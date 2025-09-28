#!/bin/bash

# GitHub CLI Integration for Visual Regression Testing
# This script provides enhanced integration with GitHub CLI for managing visual tests

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
REPO_OWNER=$(gh repo view --json owner -q .owner.login)
REPO_NAME=$(gh repo view --json name -q .name)
BRANCH_NAME=$(git branch --show-current)
PR_NUMBER=$(gh pr view --json number -q .number 2>/dev/null || echo "")

echo -e "${BLUE}🎯 GitHub CLI Visual Regression Testing${NC}"
echo -e "${BLUE}Repository: ${REPO_OWNER}/${REPO_NAME}${NC}"
echo -e "${BLUE}Branch: ${BRANCH_NAME}${NC}"
if [ -n "$PR_NUMBER" ]; then
    echo -e "${BLUE}PR: #${PR_NUMBER}${NC}"
fi
echo "=========================================="

# Function to run visual tests and get results
run_visual_tests() {
    echo -e "${YELLOW}🔧 Running visual regression tests...${NC}"
    
    # Check if we're in the right directory
    if [ ! -f "ChessScrambler.Client.csproj" ]; then
        echo -e "${RED}❌ Error: Please run this script from the ChessScrambler.Client directory${NC}"
        exit 1
    fi
    
    # Install dependencies if needed
    if ! command -v xvfb-run &> /dev/null; then
        echo -e "${YELLOW}📦 Installing Xvfb for headless testing...${NC}"
        sudo apt-get update
        sudo apt-get install -y xvfb libx11-6 libx11-xcb1 libxcb1 libxss1 libgconf-2-4 libxrandr2 libasound2 libpangocairo-1.0-0 libatk1.0-0 libcairo-gobject2 libgtk-3-0 libgdk-pixbuf2.0-0
    fi
    
    # Build and test
    echo -e "${YELLOW}🏗️  Building solution...${NC}"
    dotnet build --configuration Release
    
    echo -e "${YELLOW}🧪 Running visual tests...${NC}"
    xvfb-run -a dotnet test ChessScrambler.VisualTests/ChessScrambler.VisualTests.csproj --configuration Release --logger "console;verbosity=detailed" --collect:"XPlat Code Coverage" --results-directory TestResults
    
    # Check test results
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ All visual tests passed!${NC}"
        return 0
    else
        echo -e "${RED}❌ Visual tests failed!${NC}"
        return 1
    fi
}

# Function to comment on PR with test results
comment_on_pr() {
    local test_status=$1
    local screenshot_dir="visual-test-screenshots"
    
    if [ -z "$PR_NUMBER" ]; then
        echo -e "${YELLOW}⚠️  No PR found, skipping PR comment${NC}"
        return
    fi
    
    echo -e "${YELLOW}💬 Posting test results to PR #${PR_NUMBER}...${NC}"
    
    # Create comment body
    local comment_body=""
    if [ "$test_status" = "success" ]; then
        comment_body="## 🎨 Visual Regression Tests ✅ PASSED

All visual regression tests have passed successfully! The UI changes maintain visual consistency.

**Test Details:**
- Branch: \`${BRANCH_NAME}\`
- Test Framework: Avalonia Headless + XUnit
- Screenshots: Generated and verified
- Baseline Comparison: ✅ Passed

No visual regressions detected. 🎉"
    else
        comment_body="## 🎨 Visual Regression Tests ❌ FAILED

Visual regression tests have detected changes that may indicate UI regressions.

**Test Details:**
- Branch: \`${BRANCH_NAME}\`
- Test Framework: Avalonia Headless + XUnit
- Screenshots: Generated (check artifacts)
- Baseline Comparison: ❌ Failed

**Next Steps:**
1. Review the generated comparison images
2. If changes are intentional, update baseline images
3. If changes are unintentional, investigate the UI modifications

**Generated Files:**
- Screenshots: \`visual-test-screenshots/\`
- Comparison images: \`visual-test-screenshots/*_comparison.png\`"
    fi
    
    # Post comment
    gh pr comment "$PR_NUMBER" --body "$comment_body"
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Comment posted to PR #${PR_NUMBER}${NC}"
    else
        echo -e "${RED}❌ Failed to post comment to PR${NC}"
    fi
}

# Function to upload test artifacts
upload_artifacts() {
    local screenshot_dir="visual-test-screenshots"
    local test_results_dir="TestResults"
    
    if [ -d "$screenshot_dir" ] && [ "$(ls -A $screenshot_dir)" ]; then
        echo -e "${YELLOW}📤 Uploading screenshots as PR artifacts...${NC}"
        
        # Create a zip file with screenshots
        local zip_file="visual-test-results-$(date +%Y%m%d-%H%M%S).zip"
        zip -r "$zip_file" "$screenshot_dir" "$test_results_dir" 2>/dev/null || true
        
        if [ -f "$zip_file" ]; then
            # Upload as PR artifact (using GitHub's file upload)
            gh pr comment "$PR_NUMBER" --body "📎 **Visual Test Artifacts**

Test results and screenshots have been generated:

\`\`\`bash
# Download and extract artifacts
curl -L -o visual-test-results.zip \"[Download Link]\"
unzip visual-test-results.zip
\`\`\`

**Contents:**
- \`visual-test-screenshots/\` - Generated screenshots
- \`TestResults/\` - Test execution results
- \`*_comparison.png\` - Visual diff comparisons" 2>/dev/null || true
            
            echo -e "${GREEN}✅ Artifacts prepared for upload${NC}"
        fi
    fi
}

# Function to create baseline update PR
create_baseline_update_pr() {
    local baseline_dir="ChessScrambler.VisualTests/baseline-images"
    
    echo -e "${YELLOW}🔄 Creating baseline update workflow...${NC}"
    
    # Check if there are new baseline images
    if [ -d "$baseline_dir" ] && [ "$(ls -A $baseline_dir)" ]; then
        # Create a new branch for baseline updates
        local baseline_branch="update-visual-baselines-$(date +%Y%m%d-%H%M%S)"
        
        echo -e "${YELLOW}🌿 Creating branch: ${baseline_branch}${NC}"
        git checkout -b "$baseline_branch"
        
        # Add baseline images
        git add "$baseline_dir"
        git commit -m "Update visual regression test baselines

- Updated baseline images for visual regression tests
- Generated on $(date)
- Branch: ${BRANCH_NAME}
- PR: #${PR_NUMBER:-N/A}

This PR contains updated baseline images that should be reviewed and merged if the visual changes are intentional."
        
        # Push branch
        git push -u origin "$baseline_branch"
        
        # Create PR
        gh pr create --title "Update Visual Regression Test Baselines" \
            --body "## 🎨 Visual Baseline Update

This PR updates the baseline images for visual regression testing.

**Changes:**
- Updated baseline images in \`ChessScrambler.VisualTests/baseline-images/\`
- Generated from branch: \`${BRANCH_NAME}\`
- Source PR: #${PR_NUMBER:-N/A}

**Review Required:**
Please review the updated baseline images to ensure they represent the correct expected visual state of the application.

**Files Changed:**
$(git diff --name-only HEAD~1)

**Next Steps:**
1. Review the baseline images
2. If correct, approve and merge this PR
3. The main PR can then be updated to use the new baselines" \
            --base main
        
        echo -e "${GREEN}✅ Baseline update PR created${NC}"
        
        # Return to original branch
        git checkout "$BRANCH_NAME"
    else
        echo -e "${YELLOW}⚠️  No baseline images found to update${NC}"
    fi
}

# Function to check test status
check_test_status() {
    echo -e "${YELLOW}🔍 Checking visual test status...${NC}"
    
    # Check if tests are running in GitHub Actions
    local workflow_runs=$(gh run list --workflow="visual-regression-tests.yml" --limit 5 --json status,conclusion,headBranch)
    
    if [ -n "$workflow_runs" ]; then
        echo -e "${BLUE}Recent Visual Test Runs:${NC}"
        echo "$workflow_runs" | jq -r '.[] | "\(.headBranch): \(.status) - \(.conclusion // "running")"'
    else
        echo -e "${YELLOW}⚠️  No recent visual test runs found${NC}"
    fi
}

# Function to show help
show_help() {
    echo "GitHub CLI Visual Regression Testing"
    echo ""
    echo "Usage: $0 [command]"
    echo ""
    echo "Commands:"
    echo "  test              Run visual tests and report results"
    echo "  test-and-comment  Run tests and comment on PR"
    echo "  upload-artifacts  Upload test artifacts to PR"
    echo "  update-baselines  Create PR to update baseline images"
    echo "  check-status      Check recent test run status"
    echo "  help              Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 test                    # Run tests locally"
    echo "  $0 test-and-comment        # Run tests and comment on PR"
    echo "  $0 update-baselines        # Create baseline update PR"
    echo "  $0 check-status            # Check test status"
}

# Main script logic
case "${1:-test}" in
    "test")
        run_visual_tests
        ;;
    "test-and-comment")
        if run_visual_tests; then
            comment_on_pr "success"
        else
            comment_on_pr "failed"
            upload_artifacts
        fi
        ;;
    "upload-artifacts")
        upload_artifacts
        ;;
    "update-baselines")
        create_baseline_update_pr
        ;;
    "check-status")
        check_test_status
        ;;
    "help"|"-h"|"--help")
        show_help
        ;;
    *)
        echo -e "${RED}❌ Unknown command: $1${NC}"
        show_help
        exit 1
        ;;
esac
