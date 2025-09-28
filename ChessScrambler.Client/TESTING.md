# Testing Guide

This document explains how to run tests locally and troubleshoot GitHub Actions issues.

## Local Testing

### Linux (Ubuntu/Debian)

1. Make sure you're in the `ChessScrambler.Client` directory
2. Run the test script:
   ```bash
   ./run-tests-linux.sh
   ```

### Windows

1. Open PowerShell in the `ChessScrambler.Client` directory
2. Run the test script:
   ```powershell
   .\run-tests-windows.ps1
   ```

### Manual Testing

If you prefer to run tests manually:

```bash
# Build the solution
dotnet build ChessScrambler.Client.sln --configuration Release

# Run unit tests
dotnet test ChessScrambler.Tests/ChessScrambler.Tests.csproj --configuration Release --no-build

# Run visual tests (Linux)
xvfb-run -a dotnet test ChessScrambler.VisualTests/ChessScrambler.VisualTests.csproj --configuration Release --no-build

# Run visual tests (Windows)
dotnet test ChessScrambler.VisualTests\ChessScrambler.VisualTests.csproj --configuration Release --no-build
```

## GitHub Actions Issues

### Problem
The error you're seeing:
```
MSBUILD : error MSB1009: Project file does not exist.
Switch: your-solution-name
```

This happens because:
1. Environment variables are not set correctly
2. The workflow is trying to run on Windows but using Linux-style commands
3. The solution name is hardcoded as placeholder text

### Solution
I've created a new CI workflow (`.github/workflows/ci.yml`) that:
1. Properly sets environment variables for each platform
2. Uses the correct solution name (`ChessScrambler.Client.sln`)
3. Handles Windows, Linux, and macOS builds separately
4. Uses platform-appropriate commands and syntax

### Environment Variables
The new workflow uses these environment variables:
- `SOLUTION_NAME`: `ChessScrambler.Client.sln`
- `TEST_PROJECT_PATH`: `ChessScrambler.VisualTests/ChessScrambler.VisualTests.csproj`
- `CONFIGURATION`: `Release`

### Platform-Specific Commands
- **Windows**: Uses `$env:VARIABLE_NAME` syntax
- **Linux/macOS**: Uses `$VARIABLE_NAME` syntax

## Troubleshooting

### If tests fail locally:
1. Make sure you have .NET 8.0 installed
2. For Linux: Install Xvfb and other dependencies
3. For Windows: Make sure you have the latest .NET SDK

### If GitHub Actions fail:
1. Check the workflow file syntax
2. Verify environment variables are set correctly
3. Make sure the solution and project files exist
4. Check the logs for specific error messages

### Common Issues:
1. **"Project file does not exist"**: Check the solution name and path
2. **"Command not found"**: Check if .NET is installed and in PATH
3. **Visual tests fail**: Check if Xvfb is running (Linux) or if display is available

## File Structure
```
ChessScrambler.Client/
├── .github/workflows/
│   ├── ci.yml                    # New comprehensive CI workflow
│   ├── visual-regression-tests.yml
│   └── visual-tests-pr-integration.yml
├── run-tests-linux.sh           # Linux test runner
├── run-tests-windows.ps1        # Windows test runner
├── ChessScrambler.Client.sln    # Solution file
├── ChessScrambler.Tests/        # Unit tests
└── ChessScrambler.VisualTests/  # Visual tests
```

## Next Steps
1. Commit and push the new workflow file
2. Test the workflow by creating a pull request
3. Use the local test scripts for development
4. Monitor the GitHub Actions runs for any issues
