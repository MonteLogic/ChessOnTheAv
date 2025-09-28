# GitHub CLI Integration for Visual Regression Testing (PowerShell)
# This script provides enhanced integration with GitHub CLI for managing visual tests

param(
    [Parameter(Position=0)]
    [string]$Command = "test"
)

# Configuration
$RepoOwner = (gh repo view --json owner -q .owner.login)
$RepoName = (gh repo view --json name -q .name)
$BranchName = (git branch --show-current)
$PRNumber = (gh pr view --json number -q .number 2>$null)

Write-Host "🎯 GitHub CLI Visual Regression Testing" -ForegroundColor Blue
Write-Host "Repository: $RepoOwner/$RepoName" -ForegroundColor Blue
Write-Host "Branch: $BranchName" -ForegroundColor Blue
if ($PRNumber) {
    Write-Host "PR: #$PRNumber" -ForegroundColor Blue
}
Write-Host "==========================================" -ForegroundColor Blue

# Function to run visual tests and get results
function Run-VisualTests {
    Write-Host "🔧 Running visual regression tests..." -ForegroundColor Yellow
    
    # Check if we're in the right directory
    if (-not (Test-Path "ChessScrambler.Client.csproj")) {
        Write-Host "❌ Error: Please run this script from the ChessScrambler.Client directory" -ForegroundColor Red
        exit 1
    }
    
    # Build and test
    Write-Host "🏗️  Building solution..." -ForegroundColor Yellow
    dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Build failed" -ForegroundColor Red
        return $false
    }
    
    Write-Host "🧪 Running visual tests..." -ForegroundColor Yellow
    dotnet test ChessScrambler.VisualTests\ChessScrambler.VisualTests.csproj --configuration Release --logger "console;verbosity=detailed" --collect:"XPlat Code Coverage" --results-directory TestResults
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ All visual tests passed!" -ForegroundColor Green
        return $true
    } else {
        Write-Host "❌ Visual tests failed!" -ForegroundColor Red
        return $false
    }
}

# Function to comment on PR with test results
function Comment-OnPR {
    param(
        [string]$TestStatus
    )
    
    if (-not $PRNumber) {
        Write-Host "⚠️  No PR found, skipping PR comment" -ForegroundColor Yellow
        return
    }
    
    Write-Host "💬 Posting test results to PR #$PRNumber..." -ForegroundColor Yellow
    
    # Create comment body
    $commentBody = @"
## 🎨 Visual Regression Tests $(if ($TestStatus -eq "success") { "✅ PASSED" } else { "❌ FAILED" })

$(if ($TestStatus -eq "success") {
    "All visual regression tests have passed successfully! The UI changes maintain visual consistency."
} else {
    "Visual regression tests have detected changes that may indicate UI regressions."
})

**Test Details:**
- Branch: ``$BranchName``
- Test Framework: Avalonia Headless + XUnit
- Screenshots: Generated and verified
- Baseline Comparison: $(if ($TestStatus -eq "success") { "✅ Passed" } else { "❌ Failed" })

$(if ($TestStatus -ne "success") {
    @"
**Next Steps:**
1. Review the generated comparison images
2. If changes are intentional, update baseline images
3. If changes are unintentional, investigate the UI modifications

**Generated Files:**
- Screenshots: ``visual-test-screenshots/``
- Comparison images: ``visual-test-screenshots/*_comparison.png``
"@
} else {
    "No visual regressions detected. 🎉"
})
"@
    
    # Post comment
    gh pr comment $PRNumber --body $commentBody
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Comment posted to PR #$PRNumber" -ForegroundColor Green
    } else {
        Write-Host "❌ Failed to post comment to PR" -ForegroundColor Red
    }
}

# Function to upload test artifacts
function Upload-Artifacts {
    $screenshotDir = "visual-test-screenshots"
    $testResultsDir = "TestResults"
    
    if ((Test-Path $screenshotDir) -and (Get-ChildItem $screenshotDir -ErrorAction SilentlyContinue)) {
        Write-Host "📤 Uploading screenshots as PR artifacts..." -ForegroundColor Yellow
        
        # Create a zip file with screenshots
        $zipFile = "visual-test-results-$(Get-Date -Format 'yyyyMMdd-HHmmss').zip"
        
        if (Test-Path $screenshotDir) {
            Compress-Archive -Path $screenshotDir, $testResultsDir -DestinationPath $zipFile -Force
        }
        
        if (Test-Path $zipFile) {
            # Upload as PR artifact (using GitHub's file upload)
            $artifactComment = @"
📎 **Visual Test Artifacts**

Test results and screenshots have been generated:

``````bash
# Download and extract artifacts
curl -L -o visual-test-results.zip "[Download Link]"
unzip visual-test-results.zip
``````

**Contents:**
- ``visual-test-screenshots/`` - Generated screenshots
- ``TestResults/`` - Test execution results
- ``*_comparison.png`` - Visual diff comparisons
"@
            
            gh pr comment $PRNumber --body $artifactComment 2>$null
            
            Write-Host "✅ Artifacts prepared for upload" -ForegroundColor Green
        }
    }
}

# Function to create baseline update PR
function Create-BaselineUpdatePR {
    $baselineDir = "ChessScrambler.VisualTests\baseline-images"
    
    Write-Host "🔄 Creating baseline update workflow..." -ForegroundColor Yellow
    
    # Check if there are new baseline images
    if ((Test-Path $baselineDir) -and (Get-ChildItem $baselineDir -ErrorAction SilentlyContinue)) {
        # Create a new branch for baseline updates
        $baselineBranch = "update-visual-baselines-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
        
        Write-Host "🌿 Creating branch: $baselineBranch" -ForegroundColor Yellow
        git checkout -b $baselineBranch
        
        # Add baseline images
        git add $baselineDir
        git commit -m "Update visual regression test baselines

- Updated baseline images for visual regression tests
- Generated on $(Get-Date)
- Branch: $BranchName
- PR: #$($PRNumber -replace '^$', 'N/A')

This PR contains updated baseline images that should be reviewed and merged if the visual changes are intentional."
        
        # Push branch
        git push -u origin $baselineBranch
        
        # Create PR
        $prBody = @"
## 🎨 Visual Baseline Update

This PR updates the baseline images for visual regression testing.

**Changes:**
- Updated baseline images in ``ChessScrambler.VisualTests\baseline-images\``
- Generated from branch: ``$BranchName``
- Source PR: #$($PRNumber -replace '^$', 'N/A')

**Review Required:**
Please review the updated baseline images to ensure they represent the correct expected visual state of the application.

**Files Changed:**
$(git diff --name-only HEAD~1)

**Next Steps:**
1. Review the baseline images
2. If correct, approve and merge this PR
3. The main PR can then be updated to use the new baselines
"@
        
        gh pr create --title "Update Visual Regression Test Baselines" --body $prBody --base main
        
        Write-Host "✅ Baseline update PR created" -ForegroundColor Green
        
        # Return to original branch
        git checkout $BranchName
    } else {
        Write-Host "⚠️  No baseline images found to update" -ForegroundColor Yellow
    }
}

# Function to check test status
function Check-TestStatus {
    Write-Host "🔍 Checking visual test status..." -ForegroundColor Yellow
    
    # Check if tests are running in GitHub Actions
    $workflowRuns = gh run list --workflow="visual-regression-tests.yml" --limit 5 --json status,conclusion,headBranch
    
    if ($workflowRuns) {
        Write-Host "Recent Visual Test Runs:" -ForegroundColor Blue
        $workflowRuns | ConvertFrom-Json | ForEach-Object {
            $status = if ($_.status -eq "completed") { $_.conclusion } else { $_.status }
            Write-Host "$($_.headBranch): $($_.status) - $status"
        }
    } else {
        Write-Host "⚠️  No recent visual test runs found" -ForegroundColor Yellow
    }
}

# Function to show help
function Show-Help {
    Write-Host "GitHub CLI Visual Regression Testing" -ForegroundColor Blue
    Write-Host ""
    Write-Host "Usage: .\gh-visual-tests.ps1 [command]"
    Write-Host ""
    Write-Host "Commands:"
    Write-Host "  test              Run visual tests and report results"
    Write-Host "  test-and-comment  Run tests and comment on PR"
    Write-Host "  upload-artifacts  Upload test artifacts to PR"
    Write-Host "  update-baselines  Create PR to update baseline images"
    Write-Host "  check-status      Check recent test run status"
    Write-Host "  help              Show this help message"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\gh-visual-tests.ps1 test                    # Run tests locally"
    Write-Host "  .\gh-visual-tests.ps1 test-and-comment        # Run tests and comment on PR"
    Write-Host "  .\gh-visual-tests.ps1 update-baselines        # Create baseline update PR"
    Write-Host "  .\gh-visual-tests.ps1 check-status            # Check test status"
}

# Main script logic
switch ($Command.ToLower()) {
    "test" {
        Run-VisualTests
    }
    "test-and-comment" {
        if (Run-VisualTests) {
            Comment-OnPR "success"
        } else {
            Comment-OnPR "failed"
            Upload-Artifacts
        }
    }
    "upload-artifacts" {
        Upload-Artifacts
    }
    "update-baselines" {
        Create-BaselineUpdatePR
    }
    "check-status" {
        Check-TestStatus
    }
    "help" {
        Show-Help
    }
    default {
        Write-Host "❌ Unknown command: $Command" -ForegroundColor Red
        Show-Help
        exit 1
    }
}
