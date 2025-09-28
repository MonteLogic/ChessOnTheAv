# Game Management System Feature

## Overview
The Game Management System handles the creation, tracking, and persistence of chess games within the ChessScrambler application.

## Key Components

### Game Identification
- **Unique Game IDs**: Format `GAME_YYYYMMDDHHMMSS_RRRR`
  - YYYYMMDDHHMMSS: Timestamp of game creation
  - RRRR: Random 4-digit number for uniqueness
- **Game Metadata**: Name, creation date, last played date
- **Game State Tracking**: Current move index, player turn, game status

### Game State Management
- **Initial Position**: FEN string for starting position
- **Move History**: Complete list of all moves made
- **Current Position**: Track which move the game is currently on
- **Game Status**: Active, check, checkmate, stalemate, draw

### Game Types
- **Middlegame Positions**: Pre-defined tactical positions for practice
- **Imported Games**: Games loaded from PGN files
- **COTA Games**: Special games from the sample_games.pgn file
- **Custom Games**: User-created games with custom starting positions

### Persistence Features
- **Automatic Saving**: Game state saved automatically
- **Session Recovery**: Games persist across application restarts
- **Multiple Game Support**: Handle multiple games simultaneously
- **Game Result Tracking**: Win/loss/draw results with proper notation

## Technical Implementation

### ChessGame Model
```csharp
public class ChessGame
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastPlayedAt { get; set; }
    public string InitialFen { get; set; }
    public List<Move> MoveHistory { get; set; }
    public int CurrentMoveIndex { get; set; }
    public PieceColor CurrentPlayer { get; set; }
    public bool IsGameOver { get; set; }
    public PieceColor? Winner { get; set; }
    public string GameResult { get; set; }
}
```

### Game Operations
- **Create Game**: Generate new game with unique ID
- **Add Move**: Add moves to game history
- **Navigate Moves**: Move through game history
- **Set Game Result**: Mark game as finished with result
- **Export Game**: Convert game to PGN format

## User Experience
- **Seamless Game Creation**: New games created automatically
- **Game Identification**: Clear display of current game ID
- **State Persistence**: Games continue where left off
- **Multiple Game Support**: Switch between different games

## Dependencies
- System.DateTime for timestamps
- System.Guid for unique ID generation
- ChessDotNet for game logic
- JSON serialization for persistence
