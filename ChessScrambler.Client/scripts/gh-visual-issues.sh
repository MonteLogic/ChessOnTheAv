#!/bin/bash

# GitHub CLI Visual Regression Testing - Issue Management
# This script helps manage issues related to visual regression testing

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

echo -e "${BLUE}🎨 GitHub CLI Visual Regression - Issue Management${NC}"
echo -e "${BLUE}Repository: ${REPO_OWNER}/${REPO_NAME}${NC}"
echo "================================================"

# Function to create visual regression issue
create_visual_regression_issue() {
    local test_name=$1
    local baseline_path=$2
    local current_path=$3
    local comparison_path=$4
    
    echo -e "${YELLOW}📝 Creating visual regression issue...${NC}"
    
    # Create issue body
    local issue_body="## 🎨 Visual Regression Detected

**Test:** \`${test_name}\`
**Branch:** \`${BRANCH_NAME}\`
**PR:** #${PR_NUMBER:-N/A}
**Date:** $(date -u)

### Description
A visual regression has been detected in the \`${test_name}\` test. The current screenshot does not match the expected baseline image.

### Files
- **Baseline:** \`${baseline_path}\`
- **Current:** \`${current_path}\`
- **Comparison:** \`${comparison_path}\`

### Screenshots
Please review the attached comparison image to see the differences.

### Next Steps
1. **If intentional change:** Update baseline images and close this issue
2. **If unintentional change:** Investigate and fix the UI regression
3. **If false positive:** Adjust comparison threshold and close this issue

### Commands
\`\`\`bash
# Update baselines (if change is intentional)
./scripts/gh-visual-tests.sh update-baselines

# Re-run tests
./scripts/gh-visual-tests.sh test

# Check test status
./scripts/gh-visual-tests.sh check-status
\`\`\`

### Labels
- \`visual-regression\`
- \`bug\`
- \`ui\`
- \`testing\`"

    # Create the issue
    local issue_number=$(gh issue create \
        --title "Visual Regression: ${test_name}" \
        --body "$issue_body" \
        --label "visual-regression,bug,ui,testing" \
        --assignee "@me" \
        --json number -q .number)
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Issue #${issue_number} created${NC}"
        
        # Upload comparison image if it exists
        if [ -f "$comparison_path" ]; then
            echo -e "${YELLOW}📎 Uploading comparison image...${NC}"
            gh issue comment "$issue_number" --body "📎 **Comparison Image**

\`\`\`bash
# Download comparison image
curl -L -o comparison.png \"[Download Link]\"
\`\`\`" 2>/dev/null || true
        fi
        
        return $issue_number
    else
        echo -e "${RED}❌ Failed to create issue${NC}"
        return 1
    fi
}

# Function to list visual regression issues
list_visual_issues() {
    echo -e "${YELLOW}📋 Listing visual regression issues...${NC}"
    
    # Get issues with visual-regression label
    gh issue list --label "visual-regression" --json number,title,state,createdAt,assignees --jq '.[] | "\(.number): \(.title) [\(.state)] - \(.createdAt)"'
}

# Function to close resolved visual issues
close_resolved_issues() {
    echo -e "${YELLOW}🔒 Closing resolved visual regression issues...${NC}"
    
    # Get open visual regression issues
    local issues=$(gh issue list --label "visual-regression" --state open --json number,title)
    
    if [ -n "$issues" ]; then
        echo "Found open visual regression issues:"
        echo "$issues" | jq -r '.[] | "\(.number): \(.title)"'
        echo ""
        read -p "Do you want to close all open visual regression issues? (y/N): " -n 1 -r
        echo
        
        if [[ $REPLY =~ ^[Yy]$ ]]; then
            echo "$issues" | jq -r '.[].number' | while read -r issue_number; do
                echo "Closing issue #$issue_number..."
                gh issue close "$issue_number" --comment "Closing as resolved. Visual regression has been addressed."
            done
            echo -e "${GREEN}✅ All visual regression issues closed${NC}"
        else
            echo "Cancelled."
        fi
    else
        echo -e "${GREEN}✅ No open visual regression issues found${NC}"
    fi
}

# Function to create baseline update issue
create_baseline_update_issue() {
    local baseline_files=$1
    
    echo -e "${YELLOW}📝 Creating baseline update issue...${NC}"
    
    local issue_body="## 🎨 Baseline Images Need Update

**Branch:** \`${BRANCH_NAME}\`
**PR:** #${PR_NUMBER:-N/A}
**Date:** $(date -u)

### Description
The visual regression tests have generated new baseline images that need to be reviewed and potentially updated.

### Files to Review
${baseline_files}

### Next Steps
1. **Review the generated baseline images**
2. **Verify they represent the correct expected visual state**
3. **If correct, merge the baseline update PR**
4. **If incorrect, investigate and fix the UI issues**

### Commands
\`\`\`bash
# Review baseline images
ls -la ChessScrambler.VisualTests/baseline-images/

# Compare with previous baselines
git diff HEAD~1 ChessScrambler.VisualTests/baseline-images/

# Update baselines if correct
./scripts/gh-visual-tests.sh update-baselines
\`\`\`

### Labels
- \`baseline-update\`
- \`maintenance\`
- \`testing\`"

    local issue_number=$(gh issue create \
        --title "Baseline Images Need Update" \
        --body "$issue_body" \
        --label "baseline-update,maintenance,testing" \
        --assignee "@me" \
        --json number -q .number)
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Baseline update issue #${issue_number} created${NC}"
        return $issue_number
    else
        echo -e "${RED}❌ Failed to create baseline update issue${NC}"
        return 1
    fi
}

# Function to create test failure issue
create_test_failure_issue() {
    local test_name=$1
    local error_message=$2
    
    echo -e "${YELLOW}📝 Creating test failure issue...${NC}"
    
    local issue_body="## 🧪 Visual Test Failure

**Test:** \`${test_name}\`
**Branch:** \`${BRANCH_NAME}\`
**PR:** #${PR_NUMBER:-N/A}
**Date:** $(date -u)

### Description
A visual regression test has failed with an error.

### Error Details
\`\`\`
${error_message}
\`\`\`

### Next Steps
1. **Check the test logs for more details**
2. **Verify the test environment is set up correctly**
3. **Fix any issues preventing the test from running**
4. **Re-run the tests once fixed**

### Commands
\`\`\`bash
# Re-run the specific test
dotnet test ChessScrambler.VisualTests/ChessScrambler.VisualTests.csproj --filter \"${test_name}\"

# Run all visual tests
./scripts/gh-visual-tests.sh test

# Check test environment
./scripts/gh-visual-tests.sh check-status
\`\`\`

### Labels
- \`test-failure\`
- \`bug\`
- \`testing\`"

    local issue_number=$(gh issue create \
        --title "Visual Test Failure: ${test_name}" \
        --body "$issue_body" \
        --label "test-failure,bug,testing" \
        --assignee "@me" \
        --json number -q .number)
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Test failure issue #${issue_number} created${NC}"
        return $issue_number
    else
        echo -e "${RED}❌ Failed to create test failure issue${NC}"
        return 1
    fi
}

# Function to create maintenance issue
create_maintenance_issue() {
    local maintenance_type=$1
    local description=$2
    
    echo -e "${YELLOW}📝 Creating maintenance issue...${NC}"
    
    local issue_body="## 🔧 Visual Testing Maintenance

**Type:** ${maintenance_type}
**Branch:** \`${BRANCH_NAME}\`
**Date:** $(date -u)

### Description
${description}

### Maintenance Tasks
- [ ] Review and update baseline images
- [ ] Check test thresholds and settings
- [ ] Verify test environment setup
- [ ] Update documentation if needed
- [ ] Clean up old test artifacts

### Commands
\`\`\`bash
# Run maintenance checks
./scripts/gh-visual-tests.sh check-status

# Update baselines if needed
./scripts/gh-visual-tests.sh update-baselines

# Clean up old artifacts
./scripts/gh-visual-tests.sh clean
\`\`\`

### Labels
- \`maintenance\`
- \`testing\`"

    local issue_number=$(gh issue create \
        --title "Visual Testing Maintenance: ${maintenance_type}" \
        --body "$issue_body" \
        --label "maintenance,testing" \
        --assignee "@me" \
        --json number -q .number)
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Maintenance issue #${issue_number} created${NC}"
        return $issue_number
    else
        echo -e "${RED}❌ Failed to create maintenance issue${NC}"
        return 1
    fi
}

# Function to show help
show_help() {
    echo "GitHub CLI Visual Regression - Issue Management"
    echo ""
    echo "Usage: $0 [command] [options]"
    echo ""
    echo "Commands:"
    echo "  create-regression    Create visual regression issue"
    echo "  create-baseline      Create baseline update issue"
    echo "  create-failure       Create test failure issue"
    echo "  create-maintenance   Create maintenance issue"
    echo "  list                 List visual regression issues"
    echo "  close-resolved       Close resolved visual issues"
    echo "  help                 Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 create-regression \"MainWindow_InitialState\" \"baseline.png\" \"current.png\" \"comparison.png\""
    echo "  $0 create-baseline \"MainWindow_Baseline.png,ChessBoard_Baseline.png\""
    echo "  $0 create-failure \"MainWindow_InitialState\" \"Test failed with error...\""
    echo "  $0 create-maintenance \"Monthly Review\" \"Monthly review of visual test baselines\""
    echo "  $0 list"
    echo "  $0 close-resolved"
}

# Main script logic
case "${1:-help}" in
    "create-regression")
        if [ $# -lt 4 ]; then
            echo -e "${RED}❌ Usage: $0 create-regression <test_name> <baseline_path> <current_path> <comparison_path>${NC}"
            exit 1
        fi
        create_visual_regression_issue "$2" "$3" "$4" "$5"
        ;;
    "create-baseline")
        if [ $# -lt 2 ]; then
            echo -e "${RED}❌ Usage: $0 create-baseline <baseline_files>${NC}"
            exit 1
        fi
        create_baseline_update_issue "$2"
        ;;
    "create-failure")
        if [ $# -lt 3 ]; then
            echo -e "${RED}❌ Usage: $0 create-failure <test_name> <error_message>${NC}"
            exit 1
        fi
        create_test_failure_issue "$2" "$3"
        ;;
    "create-maintenance")
        if [ $# -lt 3 ]; then
            echo -e "${RED}❌ Usage: $0 create-maintenance <type> <description>${NC}"
            exit 1
        fi
        create_maintenance_issue "$2" "$3"
        ;;
    "list")
        list_visual_issues
        ;;
    "close-resolved")
        close_resolved_issues
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
