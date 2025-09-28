# Visual Regression Testing for ChessScrambler

This directory contains visual regression tests for the ChessScrambler Avalonia application. These tests capture screenshots of the application's UI and compare them against baseline images to detect visual regressions.

## Overview

Visual regression testing helps ensure that UI changes don't break the visual appearance of the application. The tests:

- Capture screenshots of the main window and chess board
- Compare current screenshots against baseline images
- Generate comparison images when differences are detected
- Support multiple window sizes and states

## Project Structure

```
ChessScrambler.VisualTests/
├── ChessScrambler.VisualTests.csproj    # Test project file
├── VisualTestBase.cs                    # Base class for visual tests
├── MainWindowVisualTests.cs             # Tests for main window
├── ChessBoardVisualTests.cs             # Tests for chess board
├── VisualRegressionTests.cs             # Baseline comparison tests
├── appsettings.json                     # Test configuration
├── baseline-images/                     # Baseline reference images
│   └── README.md                        # Baseline images documentation
└── README.md                            # This file
```

## Running Tests

### Local Testing

Use the provided script for easy local testing:

```bash
# Run all visual tests
./run-visual-tests.sh test

# Generate baseline images (first time setup)
./run-visual-tests.sh baseline

# Clean up test artifacts
./run-visual-tests.sh clean

# Show help
./run-visual-tests.sh help
```

### Manual Testing

You can also run tests manually:

```bash
# Build the solution
dotnet build --configuration Release

# Run visual tests with Xvfb (headless display)
xvfb-run -a dotnet test ChessScrambler.VisualTests/ChessScrambler.VisualTests.csproj --configuration Release
```

## Test Categories

### 1. Basic Visual Tests (`MainWindowVisualTests.cs`)
- Main window initial state
- Chess board rendering
- Window resizing
- Dark mode support

### 2. Chess Board Tests (`ChessBoardVisualTests.cs`)
- Initial position rendering
- Piece visibility
- Move updates
- Different window sizes

### 3. Regression Tests (`VisualRegressionTests.cs`)
- Baseline image comparison
- Visual consistency across resizes
- Automated regression detection

## Configuration

Test settings can be configured in `appsettings.json`:

```json
{
  "VisualTestSettings": {
    "ScreenshotDirectory": "visual-test-screenshots",
    "BaselineDirectory": "baseline-images",
    "ComparisonThreshold": 0.02,
    "ResizeThreshold": 0.05,
    "WindowSizes": [
      { "Width": 800, "Height": 600 },
      { "Width": 1200, "Height": 800 },
      { "Width": 1600, "Height": 1200 }
    ]
  }
}
```

## Baseline Images

Baseline images are stored in the `baseline-images/` directory and represent the expected visual state of the application. These should be:

1. **Committed to version control** - Ensures consistent testing across environments
2. **Updated when intentional changes are made** - Reflect the new expected appearance
3. **Reviewed carefully** - Ensure they represent the correct visual state

### Adding New Baselines

1. Run tests to generate current screenshots
2. Review the generated screenshots in `visual-test-screenshots/`
3. If correct, copy them to `baseline-images/` directory
4. Commit the new baseline images

### Updating Baselines

When making intentional visual changes:

1. Update the baseline images with the new expected appearance
2. Run tests to ensure they pass with the new baselines
3. Commit the updated baseline images

## GitHub Actions Integration

The visual tests are automatically run in GitHub Actions on:

- Push to `main` or `develop` branches
- Pull requests to `main` or `develop` branches
- Manual workflow dispatch

The workflow:
1. Sets up a headless Linux environment
2. Installs necessary dependencies (Xvfb, graphics libraries)
3. Builds the application
4. Runs visual regression tests
5. Uploads test results and screenshots as artifacts

## Troubleshooting

### Common Issues

1. **Tests fail with "No display" error**
   - Ensure Xvfb is installed and running
   - Use `xvfb-run` command wrapper

2. **Screenshots are blank or incorrect**
   - Check that the application initializes properly in headless mode
   - Verify window sizing and rendering settings

3. **Baseline comparison fails**
   - Review the generated comparison images
   - Check if the visual changes are intentional
   - Update baselines if changes are correct

4. **Tests are flaky or inconsistent**
   - Increase delay times in test configuration
   - Ensure proper window initialization before screenshots

### Debug Mode

For debugging, you can run tests with more verbose output:

```bash
dotnet test ChessScrambler.VisualTests/ChessScrambler.VisualTests.csproj --configuration Release --logger "console;verbosity=detailed"
```

## Best Practices

1. **Keep baselines up to date** - Update them when making intentional visual changes
2. **Review comparison images** - Always check generated comparison images when tests fail
3. **Use appropriate thresholds** - Adjust comparison thresholds based on your needs
4. **Test multiple scenarios** - Include different window sizes and states
5. **Document visual changes** - Explain why baselines were updated in commit messages

## Dependencies

The visual testing framework uses:

- **Avalonia.Headless** - Headless rendering for Avalonia applications
- **Avalonia.Headless.XUnit** - XUnit integration for Avalonia headless testing
- **SixLabors.ImageSharp** - Image processing and comparison
- **Xvfb** - Virtual display for headless testing on Linux
- **xUnit** - Testing framework

## Contributing

When adding new visual tests:

1. Follow the existing test structure and naming conventions
2. Add appropriate delays for UI initialization
3. Include both basic visual tests and regression tests
4. Update documentation if adding new test categories
5. Ensure tests are deterministic and reliable
