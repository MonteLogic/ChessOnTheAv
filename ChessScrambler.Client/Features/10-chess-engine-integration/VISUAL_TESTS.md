# Chess Engine Integration - Visual Regression Tests

## Test Coverage

### 1. Move Validation Tests
```csharp
[Fact]
public async Task ChessEngine_MoveValidation_ShouldWorkCorrectly()
```
**Validates:**
- Valid moves are accepted and executed
- Invalid moves are rejected with appropriate feedback
- Move validation is accurate for all piece types
- Special moves (castling, en passant) are handled correctly

**Visual Elements Checked:**
- Valid move highlighting accuracy
- Invalid move rejection feedback
- Move execution visual feedback
- Error message display

### 2. Game State Detection Tests
```csharp
[Fact]
public async Task ChessEngine_GameStateDetection_ShouldWorkCorrectly()
```
**Validates:**
- Check detection works correctly
- Checkmate detection is accurate
- Stalemate detection works properly
- Game over states are displayed correctly

**Visual Elements Checked:**
- Check indicator display
- Checkmate celebration
- Stalemate indication
- Game over popup

### 3. Position Analysis Tests
```csharp
[Fact]
public async Task ChessEngine_PositionAnalysis_ShouldWorkCorrectly()
```
**Validates:**
- FEN string generation is accurate
- Position loading from FEN works correctly
- Position validation is proper
- Position conversion is accurate

**Visual Elements Checked:**
- FEN string display accuracy
- Position loading visual feedback
- Position validation indicators
- Position conversion results

### 4. Move History Tests
```csharp
[Fact]
public async Task ChessEngine_MoveHistory_ShouldWorkCorrectly()
```
**Validates:**
- Move history is accurately maintained
- Move notation is correct
- Move replay works properly
- Move navigation is accurate

**Visual Elements Checked:**
- Move history display accuracy
- Move notation formatting
- Move replay visual feedback
- Navigation state updates

### 5. Special Move Tests
```csharp
[Fact]
public async Task ChessEngine_SpecialMoves_ShouldWorkCorrectly()
```
**Validates:**
- Castling moves work correctly
- En passant captures work properly
- Pawn promotion is handled correctly
- Special move notation is accurate

**Visual Elements Checked:**
- Special move execution
- Special move notation display
- Special move visual feedback
- Special move validation

### 6. Engine Error Handling Tests
```csharp
[Fact]
public async Task ChessEngine_ErrorHandling_ShouldWorkCorrectly()
```
**Validates:**
- Engine errors are handled gracefully
- Invalid positions are handled properly
- Error messages are user-friendly
- Application continues to work despite errors

**Visual Elements Checked:**
- Error message display
- Error handling visual feedback
- Graceful degradation
- User-friendly error messages

## Test Scenarios

### Scenario 1: Basic Move Validation
1. Select a piece with valid moves
2. Verify valid moves are highlighted
3. Try to make an invalid move
4. Verify move is rejected
5. Make a valid move and verify execution

### Scenario 2: Check and Checkmate
1. Play moves that put king in check
2. Verify check indicator appears
3. Play moves that result in checkmate
4. Verify checkmate detection
5. Verify game over state

### Scenario 3: Special Moves
1. Set up castling position
2. Perform castling move
3. Set up en passant position
4. Perform en passant capture
5. Set up pawn promotion
6. Perform pawn promotion

### Scenario 4: Position Analysis
1. Load position from FEN string
2. Verify position is loaded correctly
3. Generate FEN string from position
4. Verify FEN string accuracy
5. Test position validation

### Scenario 5: Move History
1. Make several moves
2. Verify move history is maintained
3. Navigate through move history
4. Verify move notation is correct
5. Test move replay

## Test Thresholds
- **Move Validation**: 2% pixel difference tolerance
- **Game State Detection**: 3% pixel difference tolerance
- **Position Analysis**: 1% pixel difference tolerance
- **Move History**: 2% pixel difference tolerance

## Baseline Images
- `ChessEngine_MoveValidation_Baseline.png`
- `ChessEngine_GameStateDetection_Baseline.png`
- `ChessEngine_PositionAnalysis_Baseline.png`
- `ChessEngine_MoveHistory_Baseline.png`
- `ChessEngine_SpecialMoves_Baseline.png`
- `ChessEngine_ErrorHandling_Baseline.png`

## Comparison Images
When tests fail, comparison images are generated:
- `ChessEngine_MoveValidation_Comparison.png`
- `ChessEngine_GameStateDetection_Comparison.png`
- `ChessEngine_PositionAnalysis_Comparison.png`
- `ChessEngine_MoveHistory_Comparison.png`
- `ChessEngine_SpecialMoves_Comparison.png`
- `ChessEngine_ErrorHandling_Comparison.png`

## Test Data
- Various chess positions for testing
- Different move types for validation
- Special move scenarios
- Error conditions for testing
- Different game states
