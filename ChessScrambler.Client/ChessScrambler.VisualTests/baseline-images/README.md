# Baseline Images for Visual Regression Testing

This directory contains baseline images used for visual regression testing. These images represent the expected visual state of the application at various points.

## Structure

- `MainWindow_InitialState.png` - Expected appearance of the main window on startup
- `ChessBoard_InitialPosition.png` - Expected appearance of the chess board in initial position
- `ChessBoard_WithPieces.png` - Expected appearance with all pieces visible
- `ChessBoard_AfterMove.png` - Expected appearance after a move is made

## Adding New Baselines

1. Run the visual tests locally to generate screenshots
2. Review the generated screenshots in the `visual-test-screenshots` directory
3. If the screenshots look correct, copy them to this `baseline-images` directory
4. Commit the baseline images to version control

## Updating Baselines

When intentional visual changes are made to the application:

1. Update the baseline images with the new expected appearance
2. Run the visual tests to ensure they pass with the new baselines
3. Commit the updated baseline images

## Note

Baseline images should be committed to version control to ensure consistent testing across different environments and team members.
