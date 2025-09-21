# Chess Move Notation Documentation

## Overview

This document describes how the ChessScrambler application handles chess move notation and the various types of moves that can be made on the chessboard.

## Move Notation System

The application uses **Standard Algebraic Notation (SAN)** for representing chess moves, which is the most widely used notation system in modern chess.

### Basic Move Format

All moves follow this general pattern:
```
[Piece][Disambiguation][Capture][Target][Promotion][Check/Checkmate]
```

Where:
- **Piece**: The piece making the move (K, Q, R, B, N, or empty for pawns)
- **Disambiguation**: File, rank, or both when multiple pieces can move to the same square
- **Capture**: "x" when capturing an opponent's piece
- **Target**: The destination square in algebraic notation (e.g., e4, f7)
- **Promotion**: "=Q", "=R", "=B", "=N" when a pawn promotes
- **Check/Checkmate**: "+" for check, "#" for checkmate

## Move Types and Their Notation

### 1. Pawn Moves

#### Basic Pawn Moves
- **Format**: `[target]` (e.g., `e4`, `d5`)
- **Example**: `e4` - Pawn moves to e4
- **Implementation**: No piece letter needed for pawns

#### Pawn Captures
- **Format**: `[from_file]x[target]` (e.g., `exd5`, `fxg6`)
- **Example**: `exd5` - Pawn on e-file captures on d5
- **Implementation**: Includes source file for disambiguation

#### Pawn Promotion
- **Format**: `[target]=[piece]` (e.g., `e8=Q`, `c1=N`)
- **Example**: `e8=Q` - Pawn promotes to Queen on e8
- **Implementation**: Uses "=" followed by piece letter

#### En Passant
- **Format**: `[from_file]x[target]` (e.g., `exd6`)
- **Example**: `exd6` - En passant capture
- **Implementation**: Treated as regular pawn capture

### 2. Piece Moves

#### Basic Piece Moves
- **Format**: `[piece][target]` (e.g., `Nf3`, `Bc4`)
- **Example**: `Nf3` - Knight moves to f3
- **Implementation**: Uses piece letters (K, Q, R, B, N)

#### Piece Captures
- **Format**: `[piece]x[target]` (e.g., `Nxe5`, `Bxf7`)
- **Example**: `Nxe5` - Knight captures on e5
- **Implementation**: Uses "x" to indicate capture

### 3. Special Moves

#### Castling
- **Format**: `O-O` (kingside) or `O-O-O` (queenside)
- **Example**: `O-O` - Kingside castling
- **Implementation**: Special notation, not piece-based

#### Check and Checkmate
- **Check**: Add "+" to any move (e.g., `Qh5+`)
- **Checkmate**: Add "#" to any move (e.g., `Qh5#`)
- **Implementation**: Detected after move execution

## Disambiguation System

When multiple pieces of the same type can move to the same square, the application uses disambiguation:

### File Disambiguation
- **When**: Multiple pieces on different files can move to the same square
- **Format**: `[piece][file][target]` (e.g., `Nbd2`, `Rae1`)
- **Example**: `Nbd2` - Knight on b-file moves to d2

### Rank Disambiguation
- **When**: Multiple pieces on different ranks can move to the same square
- **Format**: `[piece][rank][target]` (e.g., `N1f3`, `R7e1`)
- **Example**: `N1f3` - Knight on rank 1 moves to f3

### Full Disambiguation
- **When**: Multiple pieces on different files AND ranks can move to the same square
- **Format**: `[piece][file][rank][target]` (e.g., `Nbd2`)
- **Example**: `Nbd2` - Knight on b2 moves to d2

## Move Detection and Validation

### Move Validation Process
1. **Bounds Check**: Verify source and destination are within board (0-7)
2. **ChessDotNet Integration**: Use ChessDotNet library for move validation
3. **Piece Type Detection**: Determine piece type from source square
4. **Capture Detection**: Check if destination square contains opponent piece
5. **Special Move Detection**: Identify castling, en passant, promotion

### Check and Checkmate Detection
1. **Timing**: Checked AFTER move execution
2. **Check Detection**: Verify if opponent's king is in check
3. **Checkmate Detection**: Verify if opponent has no legal moves and is in check
4. **Notation Update**: Add "+" or "#" to move notation

## Move History Management

### Move Storage
- **Format**: List of `Move` objects
- **Properties**: From, To, PieceType, IsCapture, IsCheck, IsCheckmate, etc.
- **Display**: Formatted with move numbers and proper spacing

### Move Display Format
```
1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 4. Ba4 Nf6 5. O-O Be7 6. Re1 b5 7. Bb3 d6 8. c3 O-O 9. h3 Nb8 10. d4 Nbd7
```

## Implementation Details

### Key Classes

#### `Move` Class
- **Location**: `Models/ChessBoard.cs`
- **Properties**: From, To, PieceType, IsCapture, IsCheck, IsCheckmate, etc.
- **Methods**: `GetNotation()`, `GetAlgebraicNotation()`

#### `ChessBoard` Class
- **Location**: `Models/ChessBoard.cs`
- **Methods**: `MakeMove()`, `IsValidMove()`, `IsInCheck()`, `IsCheckmate()`

#### `ChessBoardViewModel` Class
- **Location**: `ViewModels/ChessBoardViewModel.cs`
- **Methods**: `UpdateGameStatus()`, `OnSquareClicked()`

### Move Notation Methods

#### `GetNotation()`
- **Purpose**: Basic move notation without disambiguation
- **Usage**: Simple move display
- **Format**: `[piece][capture][target][check]`

#### `GetAlgebraicNotation(ChessBoard board)`
- **Purpose**: Full algebraic notation with disambiguation
- **Usage**: Professional move display
- **Format**: `[piece][disambiguation][capture][target][check]`

## Example Move Sequences

### Opening Sequence
```
1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 4. Ba4 Nf6 5. O-O Be7
```

### Tactical Sequence
```
1. Nxe5 Nxe4 2. Qf3 a6 3. Qxf7#
```

### Complex Disambiguation
```
1. Nbd2 Nfd7 2. Rae1 Rae8 3. R1e3 R8e6
```

## Error Handling

### Invalid Moves
- **Detection**: ChessDotNet validation
- **Response**: Move rejected, no notation generated
- **Logging**: Console output with reason for rejection

### Edge Cases
- **Promotion**: Handled automatically by ChessDotNet
- **En Passant**: Detected and notated correctly
- **Castling**: Special notation regardless of piece positions

## Testing and Validation

### Test Cases
1. **Basic Moves**: All piece types moving normally
2. **Captures**: All piece types capturing
3. **Special Moves**: Castling, en passant, promotion
4. **Disambiguation**: Multiple pieces of same type
5. **Check/Checkmate**: Proper detection and notation

### Debug Features
- **Game Logging**: Use `--game-only` or `-go` flag
- **Console Output**: Detailed move information
- **Debug Export**: Save game state to file

## Future Enhancements

### Planned Improvements
1. **PGN Support**: Import/export PGN format
2. **Move Comments**: Add annotations to moves
3. **Variation Support**: Handle multiple move lines
4. **Time Control**: Add move timestamps
5. **Engine Integration**: Connect to chess engines

### Performance Optimizations
1. **Move Caching**: Cache valid moves for performance
2. **Incremental Updates**: Update only changed squares
3. **Memory Management**: Optimize move history storage

## Conclusion

The ChessScrambler application implements a comprehensive move notation system that handles all standard chess moves with proper disambiguation, check/checkmate detection, and professional formatting. The system is built on top of the ChessDotNet library for reliable move validation and follows standard algebraic notation conventions used in professional chess.

---

*This documentation covers the move notation system as implemented in ChessScrambler.Client version 1.0*
