# 🎨 GitHub CLI Integration for Visual Regression Testing

This document explains how to use GitHub CLI (gh) with the ChessScrambler visual regression testing system for enhanced workflow management and automation.

## 🚀 Quick Setup

### 1. Run the Setup Script
```bash
./scripts/setup-gh-integration.sh
```

This will:
- ✅ Verify GitHub CLI installation and authentication
- ✅ Create necessary directories and files
- ✅ Set up git hooks for automatic testing
- ✅ Configure VS Code tasks (if applicable)
- ✅ Create helpful aliases and documentation

### 2. Generate Initial Baselines
```bash
./scripts/gh-visual-tests.sh update-baselines
```

### 3. Test the Integration
```bash
./scripts/gh-visual-tests.sh test
```

## 📋 Available Commands

### Core Commands

| Command | Description | Usage |
|---------|-------------|-------|
| `test` | Run visual tests locally | `./scripts/gh-visual-tests.sh test` |
| `test-and-comment` | Run tests and comment on PR | `./scripts/gh-visual-tests.sh test-and-comment` |
| `update-baselines` | Create PR to update baseline images | `./scripts/gh-visual-tests.sh update-baselines` |
| `check-status` | Check recent test run status | `./scripts/gh-visual-tests.sh check-status` |
| `upload-artifacts` | Upload test artifacts to PR | `./scripts/gh-visual-tests.sh upload-artifacts` |

### GitHub CLI Aliases

Add these to your shell profile (`~/.bashrc`, `~/.zshrc`, etc.):

```bash
# Add to your shell profile
source /path/to/ChessScrambler.Client/.gh-aliases

# Then use these shortcuts:
gh-visual-test          # Run visual tests
gh-visual-comment       # Run tests and comment on PR
gh-visual-baseline      # Update baseline images
gh-visual-status        # Check test status
gh-visual-artifacts     # Upload artifacts
```

## 🔄 Workflow Integration

### Automatic PR Comments

When you run `test-and-comment`, the script will:

1. **Run Visual Tests** - Execute all visual regression tests
2. **Generate Screenshots** - Capture current UI state
3. **Compare with Baselines** - Check for visual changes
4. **Post PR Comment** - Update PR with results:

#### ✅ Success Comment
```markdown
## 🎨 Visual Regression Tests ✅ PASSED

All visual regression tests have passed successfully! The UI changes maintain visual consistency.

**Test Details:**
- Branch: `feature/new-ui`
- Test Framework: Avalonia Headless + XUnit
- Screenshots: Generated and verified
- Baseline Comparison: ✅ Passed

No visual regressions detected. 🎉
```

#### ❌ Failure Comment
```markdown
## 🎨 Visual Regression Tests ❌ FAILED

Visual regression tests have detected changes that may indicate UI regressions.

**Test Details:**
- Branch: `feature/new-ui`
- Test Framework: Avalonia Headless + XUnit
- Screenshots: Generated (check artifacts)
- Baseline Comparison: ❌ Failed

**Next Steps:**
1. Review the generated comparison images
2. If changes are intentional, update baseline images
3. If changes are unintentional, investigate the UI modifications
```

### Pre-push Hooks

The setup script configures git hooks that automatically run visual tests before pushing to `main` or `develop` branches:

```bash
# This will trigger visual tests automatically
git push origin main
```

If tests fail, the push is blocked with a helpful message.

### VS Code Integration

If you use VS Code, the setup script creates tasks for easy access:

1. **Open Command Palette** (`Ctrl+Shift+P`)
2. **Type "Tasks: Run Task"**
3. **Select from available visual test tasks:**
   - `Visual Tests: Run`
   - `Visual Tests: Run and Comment`
   - `Visual Tests: Update Baselines`

## 🎯 Advanced Usage

### Baseline Management

#### Update Baselines for Intentional Changes
```bash
# 1. Make your UI changes
# 2. Run tests to generate new screenshots
./scripts/gh-visual-tests.sh test

# 3. Create PR to update baselines
./scripts/gh-visual-tests.sh update-baselines
```

This creates a separate PR with updated baseline images that can be reviewed and merged.

#### Review Baseline Changes
```bash
# Check what baseline images have changed
git diff --name-only HEAD~1 ChessScrambler.VisualTests/baseline-images/

# View specific baseline image
git show HEAD:ChessScrambler.VisualTests/baseline-images/MainWindow_Baseline.png
```

### Artifact Management

#### Upload Test Artifacts
```bash
# Upload screenshots and test results to PR
./scripts/gh-visual-tests.sh upload-artifacts
```

This creates downloadable artifacts containing:
- Generated screenshots
- Comparison images (if tests failed)
- Test execution results
- Coverage reports

#### Download Artifacts from GitHub Actions
```bash
# List available artifacts
gh run list --workflow="visual-regression-tests.yml"

# Download specific artifact
gh run download <run-id>
```

### Status Monitoring

#### Check Test Status
```bash
# View recent test runs
./scripts/gh-visual-tests.sh check-status
```

#### Monitor PR Status
```bash
# Check PR status and comments
gh pr view --json statusCheckRollup,comments

# View specific PR comments
gh pr comment list
```

## ⚙️ Configuration

### Visual Test Settings

Edit `.github/gh-visual-tests-config.json`:

```json
{
  "visualTests": {
    "enabled": true,
    "autoComment": true,
    "uploadArtifacts": true,
    "thresholds": {
      "comparison": 0.02,
      "resize": 0.05
    },
    "windowSizes": [
      { "width": 800, "height": 600 },
      { "width": 1200, "height": 800 },
      { "width": 1600, "height": 1200 }
    ]
  }
}
```

### GitHub Actions Configuration

The system includes multiple workflows:

1. **`visual-regression-tests.yml`** - Cross-platform testing
2. **`visual-tests-pr-integration.yml`** - PR-specific integration

### Notification Settings

Configure notifications in the config file:

```json
{
  "notifications": {
    "slack": {
      "enabled": false,
      "webhookUrl": ""
    },
    "teams": {
      "enabled": false,
      "webhookUrl": ""
    }
  }
}
```

## 🐛 Troubleshooting

### Common Issues

#### 1. GitHub CLI Not Authenticated
```bash
# Authenticate with GitHub
gh auth login

# Check authentication status
gh auth status
```

#### 2. Permission Denied
```bash
# Make scripts executable
chmod +x scripts/*.sh

# Check file permissions
ls -la scripts/
```

#### 3. Tests Fail on First Run
```bash
# This is normal! Generate baselines first
./scripts/gh-visual-tests.sh update-baselines

# Review generated images
ls -la ChessScrambler.VisualTests/baseline-images/
```

#### 4. PR Comments Not Appearing
```bash
# Check if you're in a PR branch
gh pr view

# Verify GitHub CLI permissions
gh auth status
```

#### 5. VS Code Tasks Not Working
```bash
# Reload VS Code window
# Check that .vscode/tasks.json exists
cat .vscode/tasks.json
```

### Debug Mode

Run tests with verbose output:

```bash
# Enable debug logging
export DEBUG=1
./scripts/gh-visual-tests.sh test

# Or use dotnet directly
dotnet test ChessScrambler.VisualTests/ChessScrambler.VisualTests.csproj --configuration Release --logger "console;verbosity=detailed"
```

### Log Files

Check these locations for detailed logs:

- **Test Results**: `TestResults/`
- **Screenshots**: `visual-test-screenshots/`
- **GitHub Actions**: GitHub UI → Actions tab
- **PR Comments**: GitHub UI → PR → Comments

## 🔧 Customization

### Adding New Test Scenarios

1. **Create new test file**:
   ```bash
   # Create new test class
   touch ChessScrambler.VisualTests/NewFeatureVisualTests.cs
   ```

2. **Add to workflow**:
   ```yaml
   # Update .github/workflows/visual-regression-tests.yml
   - name: Run new feature tests
     run: dotnet test ChessScrambler.VisualTests/NewFeatureVisualTests.csproj
   ```

3. **Update scripts**:
   ```bash
   # Add new command to gh-visual-tests.sh
   "new-feature-tests")
       run_new_feature_tests
       ;;
   ```

### Custom Notification Channels

Add custom notification logic:

```bash
# In gh-visual-tests.sh, add:
send_slack_notification() {
    local status=$1
    local webhook_url=$SLACK_WEBHOOK_URL
    
    if [ -n "$webhook_url" ]; then
        curl -X POST -H 'Content-type: application/json' \
            --data "{\"text\":\"Visual tests $status for PR #$PR_NUMBER\"}" \
            "$webhook_url"
    fi
}
```

## 📚 Additional Resources

- **Main Visual Testing Guide**: `ChessScrambler.VisualTests/README.md`
- **GitHub CLI Documentation**: https://cli.github.com/
- **GitHub Actions Documentation**: https://docs.github.com/en/actions
- **Avalonia Headless Testing**: https://docs.avaloniaui.net/docs/guides/headless-testing

## 🤝 Contributing

To contribute to the GitHub CLI integration:

1. **Fork the repository**
2. **Create a feature branch**
3. **Make your changes**
4. **Test thoroughly**:
   ```bash
   ./scripts/gh-visual-tests.sh test
   ./scripts/gh-visual-tests.sh test-and-comment
   ```
5. **Submit a pull request**

## 📄 License

This integration follows the same license as the main ChessScrambler project.

---

**Happy Testing! 🎨✨**

For questions or issues, please open a GitHub issue or check the troubleshooting section above.
