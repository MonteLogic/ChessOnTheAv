# Move History & Navigation - Visual Regression Tests

## Test Coverage

### 1. Move History Display Tests
```csharp
[Fact]
public async Task MoveHistory_Display_ShouldShowCorrectly()
```
**Validates:**
- Move history text is displayed in proper format
- Move numbering is correct (1. e4 e5, 2. Nf3 Nc6)
- Algebraic notation is properly formatted
- Text is readable and properly aligned

**Visual Elements Checked:**
- Move history text area visibility
- Font size and color consistency
- Text alignment and spacing
- Scrollbar appearance (if applicable)

### 2. Navigation Controls Tests
```csharp
[Fact]
public async Task MoveHistory_NavigationButtons_ShouldBeVisible()
```
**Validates:**
- All navigation buttons are visible and accessible
- Button states (enabled/disabled) are correct
- Button icons are properly displayed
- Button layout is consistent

**Visual Elements Checked:**
- First Move button (⏮)
- Previous Move button (⏪)
- Next Move button (⏩)
- Last Move button (⏭)
- Button spacing and alignment

### 3. Move Counter Display Tests
```csharp
[Fact]
public async Task MoveHistory_MoveCounter_ShouldUpdateCorrectly()
```
**Validates:**
- Move counter shows current position correctly
- Format is consistent (e.g., "Move 3 of 8")
- Counter updates when navigating
- Counter reflects actual game state

**Visual Elements Checked:**
- Move counter text visibility
- Text formatting and alignment
- Update animation (if applicable)
- Position accuracy

### 4. Navigation State Tests
```csharp
[Fact]
public async Task MoveHistory_NavigationState_ShouldUpdateCorrectly()
```
**Validates:**
- Button states change based on current position
- Disabled buttons are properly grayed out
- Enabled buttons are clearly clickable
- State changes are immediate and clear

**Visual Elements Checked:**
- Button enabled/disabled states
- Visual feedback for state changes
- Consistency across all buttons
- Clear indication of available actions

### 5. Current Move Highlighting Tests
```csharp
[Fact]
public async Task MoveHistory_CurrentMoveHighlighting_ShouldWork()
```
**Validates:**
- Current move is highlighted in the move history
- Highlighting moves as user navigates
- Highlighting is visually distinct
- Highlighting accuracy

**Visual Elements Checked:**
- Current move highlighting color/style
- Highlighting visibility and contrast
- Highlighting accuracy
- Smooth highlighting transitions

### 6. Move History Scrolling Tests
```csharp
[Fact]
public async Task MoveHistory_Scrolling_ShouldWorkCorrectly()
```
**Validates:**
- Long move histories can be scrolled
- Current move remains visible when navigating
- Scrollbar appears when needed
- Scrolling is smooth and responsive

**Visual Elements Checked:**
- Scrollbar visibility and functionality
- Scroll position maintenance
- Text visibility during scrolling
- Smooth scrolling behavior

## Test Scenarios

### Scenario 1: Basic Navigation
1. Start with a game with multiple moves
2. Use Previous/Next buttons to navigate
3. Verify move counter updates correctly
4. Verify current move highlighting works
5. Verify board updates with navigation

### Scenario 2: Boundary Navigation
1. Navigate to first move
2. Verify Previous button is disabled
3. Navigate to last move
4. Verify Next button is disabled
5. Verify move counter shows correct position

### Scenario 3: Long Game Navigation
1. Load a game with many moves (20+)
2. Verify scrolling works properly
3. Navigate to different positions
4. Verify current move remains visible
5. Verify move history display is accurate

### Scenario 4: Move History Formatting
1. Make various types of moves (captures, castling, etc.)
2. Verify notation is correct for each move type
3. Verify move numbering is accurate
4. Verify special symbols (+ for check, # for checkmate)
5. Verify disambiguation works correctly

## Test Thresholds
- **Text Display**: 1% pixel difference tolerance
- **Button States**: 2% pixel difference tolerance
- **Navigation Updates**: 3% pixel difference tolerance
- **Highlighting**: 4% pixel difference tolerance

## Baseline Images
- `MoveHistory_Display_Baseline.png`
- `MoveHistory_NavigationButtons_Baseline.png`
- `MoveHistory_MoveCounter_Baseline.png`
- `MoveHistory_NavigationState_Baseline.png`
- `MoveHistory_CurrentMoveHighlighting_Baseline.png`
- `MoveHistory_Scrolling_Baseline.png`

## Comparison Images
When tests fail, comparison images are generated:
- `MoveHistory_Display_Comparison.png`
- `MoveHistory_NavigationButtons_Comparison.png`
- `MoveHistory_MoveCounter_Comparison.png`
- `MoveHistory_NavigationState_Comparison.png`
- `MoveHistory_CurrentMoveHighlighting_Comparison.png`
- `MoveHistory_Scrolling_Comparison.png`

## Test Data
- Games with various move counts (5, 10, 20, 50+ moves)
- Different move types (captures, castling, promotion, etc.)
- Games with checks and checkmates
- Games with special notation requirements
