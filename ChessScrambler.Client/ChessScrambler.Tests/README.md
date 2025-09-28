# ChessScrambler Tests

This directory contains the standard C# test projects for ChessScrambler, following .NET testing conventions.

## Test Projects

### ChessScrambler.Tests
Unit tests and integration tests for the main application.

**Structure:**
```
ChessScrambler.Tests/
├── UnitTests/
│   ├── Models/
│   │   ├── ChessBoardTests.cs
│   │   ├── ChessGameTests.cs
│   │   └── AppSettingsTests.cs
│   └── ViewModels/
│       └── ChessBoardViewModelTests.cs
├── IntegrationTests/
│   └── GameFlowTests.cs
├── TestUtilities/
│   ├── TestDataBuilder.cs
│   └── ChessTestHelpers.cs
└── ChessScrambler.Tests.csproj
```

### ChessScrambler.VisualTests
Visual regression tests for UI components.

**Structure:**
```
ChessScrambler.VisualTests/
├── ChessBoardVisualTests.cs
├── MainWindowVisualTests.cs
├── VisualTestBase.cs
└── ChessScrambler.VisualTests.csproj
```

## Running Tests

### Unit Tests
```bash
dotnet test ChessScrambler.Tests/ChessScrambler.Tests.csproj
```

### Visual Tests
```bash
dotnet test ChessScrambler.VisualTests/ChessScrambler.VisualTests.csproj
```

### All Tests
```bash
dotnet test
```

## Test Categories

### Unit Tests
- **Models**: Test individual model classes and their behavior
- **ViewModels**: Test ViewModel logic and data binding
- **Services**: Test service classes and business logic

### Integration Tests
- **GameFlow**: Test complete game workflows and component interactions
- **End-to-End**: Test complete user scenarios

### Visual Tests
- **UI Rendering**: Test that UI components render correctly
- **Visual Regression**: Compare current UI with baseline images
- **Responsive Design**: Test UI at different window sizes

## Test Utilities

### TestDataBuilder
Helper methods for creating test data and sample objects.

### ChessTestHelpers
Chess-specific helper methods for testing game logic.

## Dependencies

- **xUnit**: Testing framework
- **FluentAssertions**: Fluent assertion library
- **Moq**: Mocking framework
- **Avalonia.Headless**: Headless UI testing
- **SixLabors.ImageSharp**: Image processing for visual tests

## Test Data

Tests use both:
- **Synthetic data**: Generated test data for unit tests
- **Real data**: Actual game positions and scenarios for integration tests
- **Baseline images**: Reference images for visual regression tests

## Continuous Integration

Tests are automatically run in CI/CD pipelines:
- Unit tests run on every commit
- Visual tests run on pull requests
- Integration tests run on deployment
