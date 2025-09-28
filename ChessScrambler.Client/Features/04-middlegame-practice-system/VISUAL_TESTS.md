# Middlegame Practice System - Visual Regression Tests

## Test Coverage

### 1. Position Loading Tests
```csharp
[Fact]
public async Task MiddlegamePractice_PositionLoading_ShouldDisplayCorrectly()
```
**Validates:**
- Position loads with correct piece placement
- FEN string is properly interpreted
- Board shows the exact position from database
- Position metadata is displayed correctly

**Visual Elements Checked:**
- Piece placement accuracy
- Board state consistency
- Position title and description display
- Difficulty and theme indicators

### 2. Random Position Selection Tests
```csharp
[Fact]
public async Task MiddlegamePractice_RandomPosition_ShouldLoadCorrectly()
```
**Validates:**
- "New Position" button loads random position
- Different positions are loaded on multiple clicks
- Each position displays correctly
- Position variety is maintained

**Visual Elements Checked:**
- Button functionality and response
- Position variety in loaded positions
- Consistent loading behavior
- UI updates after position change

### 3. Position Metadata Display Tests
```csharp
[Fact]
public async Task MiddlegamePractice_MetadataDisplay_ShouldShowCorrectly()
```
**Validates:**
- Position name is displayed
- Difficulty level is shown
- Tactical themes are listed
- Key moves are displayed
- Hints are provided

**Visual Elements Checked:**
- Metadata text visibility and formatting
- Information layout and organization
- Text readability and alignment
- Consistent styling across positions

### 4. Difficulty Level Tests
```csharp
[Fact]
public async Task MiddlegamePractice_DifficultyLevels_ShouldDisplayCorrectly()
```
**Validates:**
- Difficulty indicators are visible
- Different difficulty levels are distinguishable
- Difficulty affects position complexity
- Visual indicators match difficulty rating

**Visual Elements Checked:**
- Difficulty rating display
- Visual indicators for difficulty levels
- Position complexity representation
- Consistent difficulty presentation

### 5. Position Navigation Tests
```csharp
[Fact]
public async Task MiddlegamePractice_PositionNavigation_ShouldWorkCorrectly()
```
**Validates:**
- Can navigate between different positions
- Position changes update board correctly
- Move history reflects position changes
- Navigation maintains game state

**Visual Elements Checked:**
- Navigation button functionality
- Board updates during navigation
- Move history updates
- State consistency across positions

### 6. COTA Game Integration Tests
```csharp
[Fact]
public async Task MiddlegamePractice_COTAGame_ShouldLoadCorrectly()
```
**Validates:**
- COTA game loads with correct position
- Player information is displayed
- Game metadata is shown correctly
- Position matches COTA game middlegame

**Visual Elements Checked:**
- COTA game position accuracy
- Player name display
- Game information formatting
- Position authenticity

## Test Scenarios

### Scenario 1: Basic Position Loading
1. Start application
2. Verify default position loads
3. Check position metadata display
4. Verify board shows correct position
5. Confirm difficulty and theme are shown

### Scenario 2: Random Position Selection
1. Click "New Position" button
2. Verify new position loads
3. Repeat multiple times
4. Verify different positions are loaded
5. Check position variety

### Scenario 3: Position Metadata Display
1. Load different positions
2. Verify metadata updates correctly
3. Check difficulty indicators
4. Verify tactical themes display
5. Confirm key moves are shown

### Scenario 4: Difficulty Progression
1. Load positions of different difficulties
2. Verify difficulty indicators are correct
3. Check position complexity matches difficulty
4. Verify visual indicators are consistent
5. Test all difficulty levels (1-5)

### Scenario 5: COTA Game Integration
1. Load COTA game position
2. Verify player information is displayed
3. Check game metadata is correct
4. Verify position matches expected
5. Confirm move history is available

## Test Thresholds
- **Position Accuracy**: 1% pixel difference tolerance
- **Metadata Display**: 2% pixel difference tolerance
- **Navigation Updates**: 3% pixel difference tolerance
- **Difficulty Indicators**: 4% pixel difference tolerance

## Baseline Images
- `MiddlegamePractice_PositionLoading_Baseline.png`
- `MiddlegamePractice_RandomPosition_Baseline.png`
- `MiddlegamePractice_MetadataDisplay_Baseline.png`
- `MiddlegamePractice_DifficultyLevels_Baseline.png`
- `MiddlegamePractice_PositionNavigation_Baseline.png`
- `MiddlegamePractice_COTAGame_Baseline.png`

## Comparison Images
When tests fail, comparison images are generated:
- `MiddlegamePractice_PositionLoading_Comparison.png`
- `MiddlegamePractice_RandomPosition_Comparison.png`
- `MiddlegamePractice_MetadataDisplay_Comparison.png`
- `MiddlegamePractice_DifficultyLevels_Comparison.png`
- `MiddlegamePractice_PositionNavigation_Comparison.png`
- `MiddlegamePractice_COTAGame_Comparison.png`

## Test Data
- All 8 predefined positions
- Different difficulty levels (1-5)
- Various opening themes
- COTA game positions
- Random position selections
