# Customizable Settings - Visual Regression Tests

## Test Coverage

### 1. Board Size Changes Tests
```csharp
[Fact]
public async Task Settings_BoardSizeChanges_ShouldUpdateCorrectly()
```
**Validates:**
- Board resizes when size setting changes
- Square sizes adjust proportionally
- Piece sizes scale correctly
- Board remains centered and properly positioned

**Visual Elements Checked:**
- Board dimensions accuracy
- Square size consistency
- Piece size scaling
- Board positioning and centering

### 2. Window Size Changes Tests
```csharp
[Fact]
public async Task Settings_WindowSizeChanges_ShouldUpdateCorrectly()
```
**Validates:**
- Window resizes to selected dimensions
- Board scales appropriately for new window size
- UI elements remain properly positioned
- Layout maintains proportions

**Visual Elements Checked:**
- Window dimensions accuracy
- Board scaling within window
- UI element positioning
- Layout consistency

### 3. Settings Panel Display Tests
```csharp
[Fact]
public async Task Settings_SettingsPanel_ShouldDisplayCorrectly()
```
**Validates:**
- Settings panel is visible and accessible
- All controls are properly displayed
- Current settings are shown correctly
- Controls are functional and responsive

**Visual Elements Checked:**
- Settings panel visibility
- Control layout and alignment
- Current value display
- Button states and functionality

### 4. Real-time Updates Tests
```csharp
[Fact]
public async Task Settings_RealTimeUpdates_ShouldWorkCorrectly()
```
**Validates:**
- Changes apply immediately without restart
- UI updates smoothly during changes
- No visual glitches during updates
- All elements update together

**Visual Elements Checked:**
- Immediate visual feedback
- Smooth transition animations
- Consistent update behavior
- No visual artifacts

### 5. Settings Persistence Tests
```csharp
[Fact]
public async Task Settings_Persistence_ShouldMaintainCorrectly()
```
**Validates:**
- Settings are saved automatically
- Settings persist across application restarts
- Settings load correctly on startup
- Invalid settings are corrected

**Visual Elements Checked:**
- Settings restoration accuracy
- Consistent appearance after restart
- Error handling for invalid settings
- Fallback to defaults when needed

### 6. Responsive Design Tests
```csharp
[Fact]
public async Task Settings_ResponsiveDesign_ShouldWorkCorrectly()
```
**Validates:**
- Settings work across different screen sizes
- Auto Fit Screen mode works correctly
- Minimum and maximum sizes are respected
- Layout adapts to available space

**Visual Elements Checked:**
- Responsive behavior at different sizes
- Auto Fit Screen functionality
- Size constraint enforcement
- Layout adaptation

## Test Scenarios

### Scenario 1: Board Size Progression
1. Start with default board size (480px)
2. Change to each size option (320px to 800px)
3. Verify board scales correctly
4. Verify squares and pieces scale proportionally
5. Verify board remains centered

### Scenario 2: Window Size Modes
1. Test each window size mode
2. Verify window resizes correctly
3. Verify board scales appropriately
4. Verify UI elements remain positioned correctly
5. Test Auto Fit Screen mode

### Scenario 3: Settings Persistence
1. Change various settings
2. Restart application
3. Verify settings are restored
4. Verify visual appearance matches saved settings
5. Test with invalid settings file

### Scenario 4: Real-time Updates
1. Change board size setting
2. Verify immediate visual update
3. Change window size setting
4. Verify immediate window resize
5. Verify smooth transitions

### Scenario 5: Edge Cases
1. Test minimum board size (320px)
2. Test maximum board size (800px)
3. Test minimum window size
4. Test maximum window size
5. Test Auto Fit Screen on different screen sizes

## Test Thresholds
- **Board Scaling**: 2% pixel difference tolerance
- **Window Resizing**: 3% pixel difference tolerance
- **Settings Display**: 1% pixel difference tolerance
- **Real-time Updates**: 4% pixel difference tolerance

## Baseline Images
- `Settings_BoardSize_320px_Baseline.png`
- `Settings_BoardSize_480px_Baseline.png`
- `Settings_BoardSize_800px_Baseline.png`
- `Settings_WindowSize_Compact_Baseline.png`
- `Settings_WindowSize_Large_Baseline.png`
- `Settings_WindowSize_AutoFit_Baseline.png`
- `Settings_Panel_Display_Baseline.png`
- `Settings_RealTimeUpdates_Baseline.png`

## Comparison Images
When tests fail, comparison images are generated:
- `Settings_BoardSize_*_Comparison.png`
- `Settings_WindowSize_*_Comparison.png`
- `Settings_Panel_Display_Comparison.png`
- `Settings_RealTimeUpdates_Comparison.png`

## Test Data
- All 7 board size options (320px to 800px)
- All 6 window size modes
- Different screen sizes for Auto Fit Screen
- Various settings combinations
- Edge case scenarios
