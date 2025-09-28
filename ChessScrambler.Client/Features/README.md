# ChessScrambler Features Documentation

This directory contains organized documentation and tests for each major feature of the ChessScrambler application.

## Feature Organization

Each feature has its own folder containing:
- `FEATURE_DESCRIPTION.md` - Detailed description of the feature
- `VISUAL_TESTS.md` - Visual regression tests specific to this feature
- `test_files/` - Any additional test files or examples

## Features Covered

### Core Chess Features
- [Interactive Chess Board](./01-interactive-chess-board/) - Click-to-move interface with visual feedback
- [Game Management System](./02-game-management-system/) - Unique game IDs and state persistence
- [Move History & Navigation](./03-move-history-navigation/) - Complete move tracking and navigation
- [Middlegame Practice System](./04-middlegame-practice-system/) - 8 predefined tactical positions
- [PGN Import/Export](./05-pgn-import-export/) - Load and save games in standard PGN format

### User Interface Features
- [Modern UI Design](./06-modern-ui-design/) - Dark theme with responsive design
- [Customizable Settings](./07-customizable-settings/) - Board size and window customization
- [Visual Feedback System](./08-visual-feedback-system/) - Real-time visual cues and indicators

### Technical Features
- [MVVM Architecture](./09-mvvm-architecture/) - Clean separation of concerns with data binding
- [Chess Engine Integration](./10-chess-engine-integration/) - ChessDotNet library integration
- [Debug & Export Features](./11-debug-export-features/) - Comprehensive debugging and export tools

## Visual Regression Testing

Each feature includes specific visual regression tests that validate:
- UI rendering correctness
- Feature-specific visual elements
- Responsive behavior
- State changes and transitions

## Usage

Navigate to any feature folder to see detailed documentation and tests for that specific feature.
