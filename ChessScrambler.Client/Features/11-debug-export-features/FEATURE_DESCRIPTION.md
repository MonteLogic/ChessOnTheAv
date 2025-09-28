# Debug & Export Features

## Overview
The Debug & Export system provides comprehensive debugging tools and export capabilities for analyzing chess games and application state.

## Key Components

### Debug State Export
- **Comprehensive Debug Information**: Complete application state dump
- **Chess Board State**: Current position, FEN string, piece placement
- **Game Information**: Move history, current player, game status
- **ChessDotNet State**: Internal engine state and validation
- **Valid Moves Analysis**: All available moves for current position

### PGN Export System
- **Current Game Export**: Export active game to PGN format
- **Standard PGN Headers**: Event, Site, Date, Round, Players, Result
- **Move Notation**: Standard Algebraic Notation (SAN)
- **FEN Position**: Starting position in FEN format
- **File Management**: Automatic timestamp-based naming

### Debug Information Categories

#### Game State Information
- **Timestamp**: When debug was generated
- **Current Player**: Whose turn it is
- **Game Over Status**: Whether game is finished
- **Winner**: Who won (if applicable)
- **Game Result**: 1-0, 0-1, 1/2-1/2, *

#### Board State Information
- **FEN String**: Current position in FEN format
- **Piece Placement**: Visual representation of board
- **Square Coordinates**: A1-H8 notation for each square
- **Piece Types**: All pieces with their positions

#### Move History Information
- **Complete Move List**: All moves from start to current position
- **Move Notation**: Algebraic notation for each move
- **Move Details**: From/to positions, piece types, special moves
- **Move Validation**: Whether each move was valid

#### Valid Moves Analysis
- **All Valid Moves**: Complete list of legal moves for current player
- **Move Count**: Total number of available moves
- **Move Details**: From/to positions for each valid move
- **Piece Analysis**: Which pieces can move where

#### ChessDotNet Internal State
- **Engine FEN**: ChessDotNet's internal position representation
- **Player Turn**: Whose turn according to engine
- **Check Status**: Check status for both players
- **Checkmate Status**: Checkmate status for both players
- **Stalemate Status**: Stalemate status for both players

## Technical Implementation

### Debug Export Method
```csharp
public string ExportDebugState()
{
    var debug = new StringBuilder();
    debug.AppendLine("=== CHESS BOARD DEBUG STATE ===");
    debug.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    debug.AppendLine($"Current Player: {CurrentPlayer}");
    debug.AppendLine($"Game Over: {IsGameOver}");
    debug.AppendLine($"Winner: {Winner?.ToString() ?? "None"}");
    // ... additional debug information
    return debug.ToString();
}
```

### PGN Export Method
```csharp
public string GetCurrentGamePgn()
{
    var pgn = new StringBuilder();
    pgn.AppendLine("[Event \"ChessScrambler Game\"]");
    pgn.AppendLine($"[Site \"ChessScrambler\"]");
    pgn.AppendLine($"[Date \"{DateTime.Now:yyyy.MM.dd}\"]");
    // ... additional PGN headers and moves
    return pgn.ToString();
}
```

### File Management
- **Desktop Save**: Save files to user's desktop
- **Timestamp Naming**: Automatic naming with date/time
- **File Format**: Text files for debug, PGN files for games
- **Error Handling**: Graceful handling of file system errors

## User Interface

### Debug Export Button
- **Export Debug State**: Button to generate debug information
- **Status Feedback**: Confirmation when debug is exported
- **File Location**: Show where debug file was saved
- **Error Handling**: Display errors if export fails

### PGN Export Button
- **Export Current Game**: Button to export game to PGN
- **Status Feedback**: Confirmation when PGN is exported
- **File Location**: Show where PGN file was saved
- **Error Handling**: Display errors if export fails

### Status Messages
- **Success Messages**: Confirmation of successful exports
- **Error Messages**: Clear error messages for failures
- **File Paths**: Show where files were saved
- **Operation Status**: Real-time feedback during operations

## Debug Information Format

### Header Section
```
=== CHESS BOARD DEBUG STATE ===
Timestamp: 2024-01-15 14:30:25
Current Player: White
Game Over: False
Winner: None
```

### FEN Section
```
=== FEN STRING ===
rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
```

### Board State Section
```
=== BOARD STATE ===
8 r n b q k b n r
7 p p p p p p p p
6 . . . . . . . .
5 . . . . . . . .
4 . . . . . . . .
3 . . . . . . . .
2 P P P P P P P P
1 R N B Q K B N R
  A B C D E F G H
```

### Move History Section
```
=== MOVE HISTORY ===
Move 1: e4 (from 6,4 to 4,4)
Move 2: e5 (from 1,4 to 3,4)
Move 3: Nf3 (from 7,6 to 5,5)
```

### Valid Moves Section
```
=== VALID MOVES FOR CURRENT PLAYER ===
Total valid moves: 20
  e4, e3, d4, d3, c4, c3, b4, b3, a4, a3
  Nf3, Nc3, Nh3, Na3, Bc4, Bf4, Bd3, Be2
  Qd3, Qe2
```

### ChessDotNet State Section
```
=== CHESSDOTNET INTERNAL STATE ===
ChessDotNet FEN: rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
ChessDotNet Whose Turn: White
ChessDotNet Is In Check (White): False
ChessDotNet Is In Check (Black): False
ChessDotNet Is Checkmated (White): False
ChessDotNet Is Checkmated (Black): False
ChessDotNet Is Stalemated (White): False
ChessDotNet Is Stalemated (Black): False
```

## User Experience
- **One-Click Export**: Simple button to export debug information
- **Clear Feedback**: Status messages confirm successful operations
- **Desktop Access**: Files saved to easily accessible location
- **Comprehensive Information**: All relevant state information included

## Use Cases
- **Debugging**: Analyze application state when issues occur
- **Game Analysis**: Study chess positions and moves
- **Development**: Help developers understand application behavior
- **Support**: Provide detailed information for troubleshooting

## Dependencies
- System.IO for file operations
- System.Text for string building
- System.DateTime for timestamps
- Environment.GetFolderPath for desktop access
