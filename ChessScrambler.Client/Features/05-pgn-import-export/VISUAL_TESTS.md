# PGN Import/Export - Visual Regression Tests

## Test Coverage

### 1. Import Interface Tests
```csharp
[Fact]
public async Task PgnImport_ImportInterface_ShouldDisplayCorrectly()
```
**Validates:**
- Import PGN File button is visible and accessible
- File picker dialog opens correctly
- File type filter works properly
- Import status messages are displayed

**Visual Elements Checked:**
- Import button visibility and styling
- File picker dialog appearance
- Status message display
- Button states and feedback

### 2. Import Process Tests
```csharp
[Fact]
public async Task PgnImport_ImportProcess_ShouldWorkCorrectly()
```
**Validates:**
- PGN files are imported successfully
- Multiple games are parsed correctly
- Game metadata is extracted and displayed
- Import status updates correctly

**Visual Elements Checked:**
- Import progress indicators
- Status message updates
- Game count display
- Error message handling

### 3. Games Bank Display Tests
```csharp
[Fact]
public async Task PgnImport_GamesBankDisplay_ShouldShowCorrectly()
```
**Validates:**
- Games bank status is displayed
- Number of imported games is shown
- Game information is displayed correctly
- Clear games button is functional

**Visual Elements Checked:**
- Games bank status text
- Game count display
- Game information layout
- Clear button visibility

### 4. Export Interface Tests
```csharp
[Fact]
public async Task PgnExport_ExportInterface_ShouldDisplayCorrectly()
```
**Validates:**
- Export Current Game button is visible
- Export process works correctly
- Export status messages are shown
- File naming is appropriate

**Visual Elements Checked:**
- Export button visibility and styling
- Export status messages
- File naming display
- Success/error feedback

### 5. Game Metadata Display Tests
```csharp
[Fact]
public async Task PgnImport_GameMetadata_ShouldDisplayCorrectly()
```
**Validates:**
- Player names are displayed correctly
- Game event information is shown
- Date and result information is displayed
- Game selection works properly

**Visual Elements Checked:**
- Player name display
- Event information formatting
- Date and result display
- Game selection interface

### 6. COTA Game Integration Tests
```csharp
[Fact]
public async Task PgnImport_COTAGame_ShouldLoadCorrectly()
```
**Validates:**
- COTA game is recognized and loaded
- COTA game becomes default practice position
- COTA game metadata is displayed correctly
- Middlegame positions are generated

**Visual Elements Checked:**
- COTA game recognition
- Default position loading
- COTA game metadata display
- Position generation success

## Test Scenarios

### Scenario 1: Basic PGN Import
1. Click "Import PGN File" button
2. Select a PGN file with multiple games
3. Verify games are imported successfully
4. Check games bank status updates
5. Verify game metadata is displayed

### Scenario 2: Import Error Handling
1. Try to import invalid PGN file
2. Verify error message is displayed
3. Try to import non-existent file
4. Verify appropriate error handling
5. Test with empty PGN file

### Scenario 3: Game Export
1. Play a game with several moves
2. Click "Export Current Game" button
3. Verify PGN file is created
4. Check file naming includes timestamp
5. Verify export success message

### Scenario 4: Games Bank Management
1. Import multiple games
2. Verify games bank shows correct count
3. Click "Clear Games" button
4. Verify games are cleared
5. Verify status updates correctly

### Scenario 5: COTA Game Integration
1. Import sample_games.pgn file
2. Verify COTA game is recognized
3. Check COTA game becomes default
4. Verify COTA game metadata display
5. Test middlegame position generation

## Test Thresholds
- **Interface Display**: 1% pixel difference tolerance
- **Import Process**: 2% pixel difference tolerance
- **Export Process**: 2% pixel difference tolerance
- **Metadata Display**: 3% pixel difference tolerance

## Baseline Images
- `PgnImport_ImportInterface_Baseline.png`
- `PgnImport_ImportProcess_Baseline.png`
- `PgnImport_GamesBankDisplay_Baseline.png`
- `PgnExport_ExportInterface_Baseline.png`
- `PgnImport_GameMetadata_Baseline.png`
- `PgnImport_COTAGame_Baseline.png`

## Comparison Images
When tests fail, comparison images are generated:
- `PgnImport_ImportInterface_Comparison.png`
- `PgnImport_ImportProcess_Comparison.png`
- `PgnImport_GamesBankDisplay_Comparison.png`
- `PgnExport_ExportInterface_Comparison.png`
- `PgnImport_GameMetadata_Comparison.png`
- `PgnImport_COTAGame_Comparison.png`

## Test Data
- Sample PGN files with various game counts
- PGN files with different metadata formats
- Invalid PGN files for error testing
- COTA game PGN file
- Games with different move counts and results
