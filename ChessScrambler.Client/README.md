# Chess Scrambler

A modern chess middlegame practice application built with Avalonia UI and .NET 8.

## Features

### 🎯 Middlegame Practice
- Practice chess from various middlegame positions
- Multiple pre-loaded positions with different themes and difficulties
- Real-time move validation using ChessDotNet library

### 🎮 Interactive Chess Board
- Click-to-move interface
- Visual move highlighting
- Real-time game state updates
- Check and checkmate detection

### 📚 Move History & Navigation
- **Full game history** with all prior moves displayed
- **Navigation controls** to step through moves:
  - ⏮ Go to first move
  - ⏪ Previous move
  - ⏩ Next move  
  - ⏭ Go to last move
- **Move counter** showing current position (e.g., "Move 3 of 8")

### 🆔 Game Management
- **Unique game IDs** for each game session
- Game state persistence
- Easy game identification and storage

### 🎨 Modern UI
- Clean, dark theme interface
- Responsive design
- Real-time status updates
- Move history display with proper chess notation

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Windows, macOS, or Linux

### Installation

1. Clone the repository:
```bash
git clone https://github.com/MonteLogic/ChessScrambler.git
cd ChessScrambler/ChessScrambler.Client
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the application:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run
```

## How to Use

### Basic Gameplay
1. **Select a piece** by clicking on it
2. **Move the piece** by clicking on a highlighted square
3. **Navigate moves** using the navigation controls in the sidebar
4. **View move history** in the scrollable text area

### Navigation Controls
- Use the navigation buttons to step through any game
- The move counter shows your current position
- Move history updates to show only moves up to the current position

### Game Management
- Each game has a unique ID displayed at the top
- Use "New Position" to load a random middlegame position
- Use "Reset Board" to return to the initial position

## Technical Details

### Architecture
- **Frontend**: Avalonia UI (cross-platform .NET UI framework)
- **Chess Engine**: ChessDotNet library for move validation and game logic
- **Pattern**: MVVM (Model-View-ViewModel) architecture

### Key Components
- `ChessBoard` - Core chess game logic and state management
- `ChessGame` - Game metadata and move history management
- `ChessBoardViewModel` - UI data binding and user interaction
- `MiddlegamePosition` - Pre-defined chess positions for practice

### Move Notation
- Uses standard algebraic chess notation
- Proper disambiguation for ambiguous moves
- Check (+) and checkmate (#) symbols
- Move numbers for white and black moves

## Development

### Project Structure
```
ChessScrambler.Client/
├── Models/           # Data models (ChessBoard, ChessGame, etc.)
├── ViewModels/       # MVVM view models
├── Converters/       # UI data converters
├── Controls/         # Custom UI controls
└── MainWindow.axaml  # Main application window
```

### Building from Source
```bash
# Debug build
dotnet build

# Release build
dotnet build -c Release

# Run tests (if available)
dotnet test
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- [ChessDotNet](https://github.com/ProgramFOX/Chess.NET) - Chess game logic library
- [Avalonia UI](https://avaloniaui.net/) - Cross-platform UI framework
- Chess community for middlegame position inspiration

---

**Chess Scrambler** - Practice chess middlegames with style! ♟️
