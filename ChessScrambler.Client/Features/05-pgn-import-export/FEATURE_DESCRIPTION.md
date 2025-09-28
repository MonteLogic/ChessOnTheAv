# PGN Import/Export Feature

## Overview
The PGN Import/Export system allows users to load chess games from PGN files and export their current games in standard PGN format.

## Key Components

### PGN Import System
- **File Picker Dialog**: Native file selection interface
- **Multiple Game Support**: Import multiple games from single PGN file
- **Game Parsing**: Extract games, moves, and metadata from PGN format
- **Metadata Extraction**: Parse player names, event, date, result, opening
- **Error Handling**: Graceful handling of malformed PGN files

### PGN Export System
- **Current Game Export**: Export active game to PGN format
- **Standard Format**: Follows PGN specification for compatibility
- **Metadata Inclusion**: Include game headers and result
- **File Management**: Save to desktop with timestamp

### Game Bank Management
- **Game Storage**: Store imported games in memory
- **Game Selection**: Choose specific games for practice
- **Game Metadata**: Track player names, dates, results
- **Middlegame Generation**: Create practice positions from imported games

## Technical Implementation

### PGN Parsing
```csharp
private static List<ImportedGame> ParsePgnContent(string pgnContent)
{
    // Split PGN into individual games
    // Parse game headers (Event, Site, Date, etc.)
    // Extract move sequences
    // Create ImportedGame objects
}
```

### Game Bank Operations
- **ImportGamesFromFile()**: Load games from PGN file
- **ImportGamesFromPgn()**: Load games from PGN string
- **GetDefaultMiddlegamePosition()**: Get practice position
- **ClearGames()**: Remove all imported games
- **GetImportedGamesCount()**: Count available games

### ImportedGame Model
```csharp
public class ImportedGame
{
    public string Id { get; set; }
    public string WhitePlayer { get; set; }
    public string BlackPlayer { get; set; }
    public string Event { get; set; }
    public string Site { get; set; }
    public string Date { get; set; }
    public string Round { get; set; }
    public string Result { get; set; }
    public string Opening { get; set; }
    public List<string> Moves { get; set; }
    public string FullPgn { get; set; }
}
```

## User Interface

### Import Interface
- **Import PGN File Button**: Trigger file picker dialog
- **File Type Filter**: Filter for .pgn files
- **Import Status**: Show number of games imported
- **Clear Games Button**: Remove all imported games

### Export Interface
- **Export Current Game Button**: Export active game
- **File Naming**: Automatic timestamp-based naming
- **Export Status**: Confirm successful export
- **Desktop Save**: Save to user's desktop

### Game Information Display
- **Games Bank Status**: Show number of loaded games
- **Player Names**: Display white and black players
- **Game Metadata**: Show event, date, result information
- **Game Selection**: Choose specific games for practice

## Advanced Features

### COTA Game Integration
- **Special Game Recognition**: Identify COTA games by player names
- **Default Game Selection**: Use COTA game as default practice position
- **Game Context**: Maintain original game information
- **Middlegame Positions**: Generate practice positions from COTA games

### Middlegame Position Generation
- **Position Calculation**: Create middlegame positions from game moves
- **Multiple Positions**: Generate 3-5 positions per game
- **Move Replay**: Replay moves to reach middlegame positions
- **FEN Generation**: Convert positions to FEN format

### Error Handling
- **Malformed PGN**: Handle invalid PGN syntax gracefully
- **Missing Files**: Provide clear error messages
- **Parse Errors**: Continue processing other games if one fails
- **Fallback Behavior**: Use default positions if import fails

## User Experience
- **Simple Import**: One-click PGN file import
- **Automatic Processing**: Games processed automatically
- **Clear Feedback**: Status messages for all operations
- **Flexible Export**: Export current game anytime

## File Format Support

### PGN Headers Supported
- `[Event "..."]` - Tournament or event name
- `[Site "..."]` - Location of the game
- `[Date "..."]` - Date in YYYY.MM.DD format
- `[Round "..."]` - Round number
- `[White "..."]` - White player name
- `[Black "..."]` - Black player name
- `[Result "..."]` - Game result (1-0, 0-1, 1/2-1/2, *)
- `[ECO "..."]` - Opening classification

### Move Notation Support
- **Standard Algebraic Notation (SAN)**: e4, Nf3, O-O, etc.
- **Move Numbers**: 1. e4 e5 2. Nf3 Nc6
- **Special Moves**: Castling, en passant, promotion
- **Game Results**: 1-0, 0-1, 1/2-1/2, *

## Dependencies
- System.IO for file operations
- Avalonia.Platform.Storage for file picker
- System.Text.RegularExpressions for PGN parsing
- System.Collections.Generic for game storage
