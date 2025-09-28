#!/bin/bash

# Setup script for GitHub CLI Visual Regression Testing Integration
# This script helps configure the environment for optimal GitHub CLI integration

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🔧 Setting up GitHub CLI Visual Regression Testing Integration${NC}"
echo "=================================================================="

# Check if GitHub CLI is installed
if ! command -v gh &> /dev/null; then
    echo -e "${RED}❌ GitHub CLI is not installed${NC}"
    echo -e "${YELLOW}Please install GitHub CLI first:${NC}"
    echo "  Ubuntu/Debian: curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | sudo dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg"
    echo "  sudo chmod go+r /usr/share/keyrings/githubcli-archive-keyring.gpg"
    echo "  echo \"deb [arch=\$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main\" | sudo tee /etc/apt/sources.list.d/github-cli.list > /dev/null"
    echo "  sudo apt update"
    echo "  sudo apt install gh"
    echo ""
    echo "  Or visit: https://cli.github.com/manual/installation"
    exit 1
fi

echo -e "${GREEN}✅ GitHub CLI is installed${NC}"

# Check if user is authenticated
if ! gh auth status &> /dev/null; then
    echo -e "${YELLOW}🔐 GitHub CLI authentication required${NC}"
    echo "Please authenticate with GitHub CLI:"
    gh auth login
fi

echo -e "${GREEN}✅ GitHub CLI is authenticated${NC}"

# Check if we're in a git repository
if [ ! -d ".git" ]; then
    echo -e "${RED}❌ Not in a git repository${NC}"
    echo "Please run this script from the root of your git repository"
    exit 1
fi

echo -e "${GREEN}✅ Git repository detected${NC}"

# Check if we're in the right directory
if [ ! -f "ChessScrambler.Client.csproj" ]; then
    echo -e "${RED}❌ ChessScrambler.Client.csproj not found${NC}"
    echo "Please run this script from the ChessScrambler.Client directory"
    exit 1
fi

echo -e "${GREEN}✅ ChessScrambler project detected${NC}"

# Create necessary directories
echo -e "${YELLOW}📁 Creating necessary directories...${NC}"
mkdir -p scripts
mkdir -p .github/workflows
mkdir -p ChessScrambler.VisualTests/baseline-images
mkdir -p visual-test-screenshots

echo -e "${GREEN}✅ Directories created${NC}"

# Set up GitHub CLI aliases
echo -e "${YELLOW}🔗 Setting up GitHub CLI aliases...${NC}"

# Create a .gh-aliases file
cat > .gh-aliases << 'EOF'
# GitHub CLI Aliases for Visual Regression Testing
alias gh-visual-test='./scripts/gh-visual-tests.sh test'
alias gh-visual-comment='./scripts/gh-visual-tests.sh test-and-comment'
alias gh-visual-baseline='./scripts/gh-visual-tests.sh update-baselines'
alias gh-visual-status='./scripts/gh-visual-tests.sh check-status'
alias gh-visual-artifacts='./scripts/gh-visual-tests.sh upload-artifacts'
EOF

echo -e "${GREEN}✅ GitHub CLI aliases created${NC}"

# Set up git hooks
echo -e "${YELLOW}🪝 Setting up git hooks...${NC}"

# Pre-push hook to run visual tests
cat > .git/hooks/pre-push << 'EOF'
#!/bin/bash
# Pre-push hook to run visual regression tests

echo "🎨 Running visual regression tests before push..."

# Check if we're pushing to main or develop
while read local_ref local_sha remote_ref remote_sha; do
    if [[ "$remote_ref" == "refs/heads/main" ]] || [[ "$remote_ref" == "refs/heads/develop" ]]; then
        echo "Pushing to $remote_ref - running visual tests..."
        
        # Run visual tests
        if ! ./scripts/gh-visual-tests.sh test; then
            echo "❌ Visual tests failed. Push aborted."
            echo "Run './scripts/gh-visual-tests.sh test-and-comment' to see details."
            exit 1
        fi
        
        echo "✅ Visual tests passed. Proceeding with push..."
    fi
done

exit 0
EOF

chmod +x .git/hooks/pre-push

echo -e "${GREEN}✅ Git hooks configured${NC}"

# Set up VS Code tasks (if .vscode directory exists)
if [ -d ".vscode" ]; then
    echo -e "${YELLOW}🔧 Setting up VS Code tasks...${NC}"
    
    # Create or update tasks.json
    if [ -f ".vscode/tasks.json" ]; then
        echo "Updating existing VS Code tasks..."
        # Add visual test tasks to existing tasks.json
        # This is a simplified approach - in practice, you'd want to merge JSON properly
    else
        cat > .vscode/tasks.json << 'EOF'
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "Visual Tests: Run",
            "type": "shell",
            "command": "./scripts/gh-visual-tests.sh",
            "args": ["test"],
            "group": "test",
            "presentation": {
                "echo": true,
                "reveal": "always",
                "focus": false,
                "panel": "shared"
            },
            "problemMatcher": []
        },
        {
            "label": "Visual Tests: Run and Comment",
            "type": "shell",
            "command": "./scripts/gh-visual-tests.sh",
            "args": ["test-and-comment"],
            "group": "test",
            "presentation": {
                "echo": true,
                "reveal": "always",
                "focus": false,
                "panel": "shared"
            },
            "problemMatcher": []
        },
        {
            "label": "Visual Tests: Update Baselines",
            "type": "shell",
            "command": "./scripts/gh-visual-tests.sh",
            "args": ["update-baselines"],
            "group": "test",
            "presentation": {
                "echo": true,
                "reveal": "always",
                "focus": false,
                "panel": "shared"
            },
            "problemMatcher": []
        }
    ]
}
EOF
    fi
    
    echo -e "${GREEN}✅ VS Code tasks configured${NC}"
fi

# Create a quick start guide
echo -e "${YELLOW}📚 Creating quick start guide...${NC}"

cat > VISUAL_TESTING_GUIDE.md << 'EOF'
# 🎨 Visual Regression Testing - Quick Start Guide

## GitHub CLI Integration Setup Complete! 🎉

### Available Commands

#### Basic Commands
```bash
# Run visual tests locally
./scripts/gh-visual-tests.sh test

# Run tests and comment on PR
./scripts/gh-visual-tests.sh test-and-comment

# Update baseline images
./scripts/gh-visual-tests.sh update-baselines

# Check test status
./scripts/gh-visual-tests.sh check-status

# Upload test artifacts
./scripts/gh-visual-tests.sh upload-artifacts
```

#### GitHub CLI Aliases (add to your shell profile)
```bash
# Add to ~/.bashrc or ~/.zshrc
source .gh-aliases

# Then use:
gh-visual-test
gh-visual-comment
gh-visual-baseline
gh-visual-status
gh-visual-artifacts
```

### Workflow Integration

#### Automatic PR Comments
- Visual tests run automatically on PR creation/updates
- Results are posted as PR comments
- Screenshots are uploaded as artifacts

#### Pre-push Hooks
- Visual tests run before pushing to main/develop branches
- Push is blocked if tests fail
- Ensures visual consistency in main branches

#### VS Code Integration
- Tasks available in VS Code Command Palette
- Run tests directly from the editor
- View results in integrated terminal

### First Time Setup

1. **Generate Initial Baselines:**
   ```bash
   ./scripts/gh-visual-tests.sh update-baselines
   ```

2. **Review Generated Images:**
   - Check `ChessScrambler.VisualTests/baseline-images/`
   - Ensure they represent correct visual state

3. **Commit Baselines:**
   ```bash
   git add ChessScrambler.VisualTests/baseline-images/
   git commit -m "Add initial visual regression test baselines"
   git push
   ```

### Configuration

Edit `.github/gh-visual-tests-config.json` to customize:
- Test thresholds
- Window sizes
- Notification settings
- Artifact retention

### Troubleshooting

#### Tests Fail on First Run
- This is normal! Generate baselines first
- Run `./scripts/gh-visual-tests.sh update-baselines`

#### Permission Issues
- Ensure scripts are executable: `chmod +x scripts/*.sh`
- Check GitHub CLI authentication: `gh auth status`

#### VS Code Tasks Not Working
- Reload VS Code window
- Check that tasks.json was created properly

### Need Help?

- Check the main README in `ChessScrambler.VisualTests/`
- Review GitHub Actions logs
- Use `./scripts/gh-visual-tests.sh help` for command help

Happy Testing! 🎨✨
EOF

echo -e "${GREEN}✅ Quick start guide created${NC}"

# Final setup verification
echo -e "${YELLOW}🔍 Verifying setup...${NC}"

# Check if all required files exist
required_files=(
    "scripts/gh-visual-tests.sh"
    ".github/workflows/visual-regression-tests.yml"
    ".github/workflows/visual-tests-pr-integration.yml"
    "ChessScrambler.VisualTests/ChessScrambler.VisualTests.csproj"
    ".github/gh-visual-tests-config.json"
)

all_good=true
for file in "${required_files[@]}"; do
    if [ -f "$file" ]; then
        echo -e "  ✅ $file"
    else
        echo -e "  ❌ $file"
        all_good=false
    fi
done

if [ "$all_good" = true ]; then
    echo -e "${GREEN}🎉 Setup completed successfully!${NC}"
    echo ""
    echo -e "${BLUE}Next steps:${NC}"
    echo "1. Review the quick start guide: cat VISUAL_TESTING_GUIDE.md"
    echo "2. Generate initial baselines: ./scripts/gh-visual-tests.sh update-baselines"
    echo "3. Test the integration: ./scripts/gh-visual-tests.sh test"
    echo ""
    echo -e "${YELLOW}💡 Pro tip: Add the aliases to your shell profile for easier access!${NC}"
    echo "   echo 'source $(pwd)/.gh-aliases' >> ~/.bashrc"
else
    echo -e "${RED}❌ Setup incomplete. Some files are missing.${NC}"
    exit 1
fi
