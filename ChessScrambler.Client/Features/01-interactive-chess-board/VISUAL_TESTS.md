# Interactive Chess Board - Visual Regression Tests

## Test Coverage

### 1. Board Rendering Tests
```csharp
[Fact]
public async Task ChessBoard_InitialPosition_ShouldRenderCorrectly()
```
**Validates:**
- 8x8 grid layout is properly rendered
- Alternating square colors (light/dark pattern)
- All 32 pieces are placed in correct starting positions
- Board dimensions and proportions are correct

**Visual Elements Checked:**
- Square color pattern (a1=dark, b1=light, etc.)
- Piece placement accuracy
- Board border and layout
- Overall visual consistency

### 2. Piece Selection Tests
```csharp
[Fact]
public async Task ChessBoard_PieceSelection_ShouldHighlightCorrectly()
```
**Validates:**
- Selected pieces are visually highlighted
- Only pieces of the current player can be selected
- Selection state is clearly visible
- Deselection works properly

**Visual Elements Checked:**
- Selection highlighting color and style
- Selection state persistence
- Visual feedback for invalid selections

### 3. Move Highlighting Tests
```csharp
[Fact]
public async Task ChessBoard_ValidMoves_ShouldBeHighlighted()
```
**Validates:**
- Valid moves are highlighted when piece is selected
- Invalid squares are not highlighted
- Highlighting disappears when piece is deselected
- Move highlighting is accurate for all piece types

**Visual Elements Checked:**
- Valid move highlighting color and style
- Highlighting accuracy for different piece types
- Clear visual distinction between valid/invalid moves

### 4. Piece Movement Tests
```csharp
[Fact]
public async Task ChessBoard_AfterMove_ShouldUpdateCorrectly()
```
**Validates:**
- Pieces move to correct positions
- Source square becomes empty
- Destination square shows the moved piece
- Board state updates immediately after move

**Visual Elements Checked:**
- Piece position accuracy after moves
- Empty square rendering
- Move animation completion (if applicable)
- Board state consistency

### 5. Responsive Board Tests
```csharp
[Fact]
public async Task ChessBoard_DifferentSizes_ShouldScaleCorrectly()
```
**Validates:**
- Board scales properly at different window sizes
- Piece graphics maintain quality at all sizes
- Square proportions remain consistent
- Board remains centered and properly positioned

**Test Sizes:**
- 800x600 (Small)
- 1200x800 (Medium)
- 1600x1200 (Large)
- 1920x1080 (Full HD)

## Test Thresholds
- **Board Layout**: 1% pixel difference tolerance
- **Piece Placement**: 2% pixel difference tolerance
- **Selection/Highlighting**: 3% pixel difference tolerance
- **Responsive Scaling**: 5% pixel difference tolerance

## Baseline Images
- `ChessBoard_InitialPosition_Baseline.png`
- `ChessBoard_PieceSelection_Baseline.png`
- `ChessBoard_ValidMoves_Baseline.png`
- `ChessBoard_AfterMove_Baseline.png`
- `ChessBoard_Resize_800x600_Baseline.png`
- `ChessBoard_Resize_1200x800_Baseline.png`
- `ChessBoard_Resize_1600x1200_Baseline.png`
- `ChessBoard_Resize_1920x1080_Baseline.png`

## Comparison Images
When tests fail, comparison images are generated:
- `ChessBoard_InitialPosition_Comparison.png`
- `ChessBoard_PieceSelection_Comparison.png`
- `ChessBoard_ValidMoves_Comparison.png`
- `ChessBoard_AfterMove_Comparison.png`
- `ChessBoard_Resize_*_Comparison.png`
