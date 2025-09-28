# Debug & Export Features - Visual Regression Tests

## Test Coverage

### 1. Debug Export Button Tests
```csharp
[Fact]
public async Task DebugExport_ExportButton_ShouldDisplayCorrectly()
```
**Validates:**
- Export Debug State button is visible and accessible
- Button styling is consistent with UI theme
- Button responds to clicks correctly
- Button state changes appropriately

**Visual Elements Checked:**
- Button visibility and positioning
- Button styling and appearance
- Click response and feedback
- Button state consistency

### 2. PGN Export Button Tests
```csharp
[Fact]
public async Task DebugExport_PgnExportButton_ShouldDisplayCorrectly()
```
**Validates:**
- Export Current Game PGN button is visible
- Button styling matches other export buttons
- Button functionality works correctly
- Button provides appropriate feedback

**Visual Elements Checked:**
- Button visibility and positioning
- Button styling consistency
- Click response and feedback
- Button state management

### 3. Export Status Messages Tests
```csharp
[Fact]
public async Task DebugExport_StatusMessages_ShouldDisplayCorrectly()
```
**Validates:**
- Success messages are displayed after export
- Error messages appear when export fails
- Status messages are clearly visible
- Messages provide useful information

**Visual Elements Checked:**
- Status message visibility
- Message formatting and styling
- Success/error message distinction
- Message positioning and layout

### 4. Export Process Tests
```csharp
[Fact]
public async Task DebugExport_ExportProcess_ShouldWorkCorrectly()
```
**Validates:**
- Debug export generates file successfully
- PGN export creates valid PGN file
- File naming includes timestamp
- Export process provides feedback

**Visual Elements Checked:**
- Export process feedback
- File naming display
- Success confirmation
- Error handling display

### 5. Debug Information Display Tests
```csharp
[Fact]
public async Task DebugExport_DebugInfo_ShouldDisplayCorrectly()
```
**Validates:**
- Debug information is comprehensive
- Information is well-formatted
- All sections are included
- Information is accurate

**Visual Elements Checked:**
- Debug information formatting
- Section headers and organization
- Data accuracy and completeness
- Text readability and layout

### 6. Error Handling Tests
```csharp
[Fact]
public async Task DebugExport_ErrorHandling_ShouldWorkCorrectly()
```
**Validates:**
- File system errors are handled gracefully
- Permission errors are reported clearly
- Invalid states are handled properly
- Error messages are user-friendly

**Visual Elements Checked:**
- Error message display
- Error handling UI feedback
- Graceful degradation
- User-friendly error messages

## Test Scenarios

### Scenario 1: Debug Export Process
1. Click "Export Debug State" button
2. Verify debug file is created
3. Check status message confirms success
4. Verify file is saved to desktop
5. Verify file contains comprehensive information

### Scenario 2: PGN Export Process
1. Play a game with several moves
2. Click "Export Current Game PGN" button
3. Verify PGN file is created
4. Check status message confirms success
5. Verify PGN file contains valid game data

### Scenario 3: Error Handling
1. Simulate file system error
2. Verify error message is displayed
3. Check error message is user-friendly
4. Verify application continues to work
5. Test recovery from error state

### Scenario 4: Multiple Exports
1. Export debug state multiple times
2. Export PGN multiple times
3. Verify each export works correctly
4. Check file naming includes timestamps
5. Verify no conflicts between exports

### Scenario 5: Export with Different Game States
1. Export debug with game in progress
2. Export debug with checkmate
3. Export debug with stalemate
4. Verify debug information reflects state
5. Check PGN export works for all states

## Test Thresholds
- **Button Display**: 1% pixel difference tolerance
- **Status Messages**: 2% pixel difference tolerance
- **Export Process**: 3% pixel difference tolerance
- **Error Handling**: 4% pixel difference tolerance

## Baseline Images
- `DebugExport_ExportButton_Baseline.png`
- `DebugExport_PgnExportButton_Baseline.png`
- `DebugExport_StatusMessages_Baseline.png`
- `DebugExport_ExportProcess_Baseline.png`
- `DebugExport_DebugInfo_Baseline.png`
- `DebugExport_ErrorHandling_Baseline.png`

## Comparison Images
When tests fail, comparison images are generated:
- `DebugExport_ExportButton_Comparison.png`
- `DebugExport_PgnExportButton_Comparison.png`
- `DebugExport_StatusMessages_Comparison.png`
- `DebugExport_ExportProcess_Comparison.png`
- `DebugExport_DebugInfo_Comparison.png`
- `DebugExport_ErrorHandling_Comparison.png`

## Test Data
- Various game states for debug export
- Different move counts for PGN export
- Error conditions for testing
- Different file system states
- Various application states
