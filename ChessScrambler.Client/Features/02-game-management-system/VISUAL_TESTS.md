# Game Management System - Visual Regression Tests

## Test Coverage

### 1. Game ID Display Tests
```csharp
[Fact]
public async Task GameManagement_GameIdDisplay_ShouldShowCorrectly()
```
**Validates:**
- Game ID is displayed prominently in the UI
- ID format matches expected pattern (GAME_YYYYMMDDHHMMSS_RRRR)
- ID updates when new game is created
- ID remains visible throughout gameplay

**Visual Elements Checked:**
- Game ID text visibility and positioning
- Font size and color consistency
- Text alignment and spacing
- ID format validation

### 2. Game Status Display Tests
```csharp
[Fact]
public async Task GameManagement_GameStatus_ShouldUpdateCorrectly()
```
**Validates:**
- Current player indicator shows correctly
- Game status updates (In Progress, Check, Checkmate, etc.)
- Status text is clearly visible and readable
- Status changes are reflected immediately

**Visual Elements Checked:**
- Player turn indicator text
- Game status message visibility
- Status color coding (if applicable)
- Text positioning and alignment

### 3. Game Creation Tests
```csharp
[Fact]
public async Task GameManagement_NewGame_ShouldCreateCorrectly()
```
**Validates:**
- New game button is visible and accessible
- Clicking creates new game with new ID
- UI updates to show new game state
- Previous game state is properly cleared

**Visual Elements Checked:**
- New game button visibility and styling
- Button click response and feedback
- UI state changes after game creation
- Game ID update animation (if applicable)

### 4. Game State Persistence Tests
```csharp
[Fact]
public async Task GameManagement_GamePersistence_ShouldMaintainState()
```
**Validates:**
- Game state persists across UI updates
- Move history remains intact
- Current position is maintained
- Game metadata is preserved

**Visual Elements Checked:**
- Move history display consistency
- Current position indicator accuracy
- Game metadata display persistence
- State restoration after UI refresh

### 5. Multiple Game Support Tests
```csharp
[Fact]
public async Task GameManagement_MultipleGames_ShouldHandleCorrectly()
```
**Validates:**
- Multiple games can be created
- Game switching works properly
- Each game maintains its own state
- UI updates correctly when switching games

**Visual Elements Checked:**
- Game switching interface (if applicable)
- State isolation between games
- UI consistency across different games
- Game selection indicators

## Test Scenarios

### Scenario 1: New Game Creation
1. Start with existing game
2. Click "New Position" button
3. Verify new game ID is generated
4. Verify UI resets to initial state
5. Verify old game state is cleared

### Scenario 2: Game State Updates
1. Make several moves in a game
2. Verify game status updates correctly
3. Verify move history is maintained
4. Verify current position tracking works
5. Verify game metadata is preserved

### Scenario 3: Game Result Handling
1. Play game to completion (checkmate/stalemate)
2. Verify game result is displayed
3. Verify game over state is shown
4. Verify appropriate UI changes occur
5. Verify game can be reset or new game started

## Test Thresholds
- **Text Display**: 1% pixel difference tolerance
- **Button States**: 2% pixel difference tolerance
- **UI Updates**: 3% pixel difference tolerance
- **State Changes**: 5% pixel difference tolerance

## Baseline Images
- `GameManagement_GameIdDisplay_Baseline.png`
- `GameManagement_GameStatus_Baseline.png`
- `GameManagement_NewGame_Baseline.png`
- `GameManagement_GamePersistence_Baseline.png`
- `GameManagement_MultipleGames_Baseline.png`

## Comparison Images
When tests fail, comparison images are generated:
- `GameManagement_GameIdDisplay_Comparison.png`
- `GameManagement_GameStatus_Comparison.png`
- `GameManagement_NewGame_Comparison.png`
- `GameManagement_GamePersistence_Comparison.png`
- `GameManagement_MultipleGames_Comparison.png`

## Test Data
- Sample game IDs for testing
- Various game states (in progress, check, checkmate)
- Different game types (middlegame, imported, custom)
- Multiple move scenarios for state testing
