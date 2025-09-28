# Customizable Settings Feature

## Overview
The Customizable Settings system allows users to personalize their ChessScrambler experience by adjusting board size, window dimensions, and other visual preferences.

## Key Components

### Board Size Customization
- **7 Size Options**: 320px to 800px in increments
  - Small (320px)
  - Medium (400px) 
  - Large (480px) - Default
  - X-Large (560px)
  - XX-Large (640px)
  - XXX-Large (720px)
  - Huge (800px)
- **Automatic Scaling**: Square and piece sizes calculated proportionally
- **Real-time Updates**: Changes apply immediately without restart

### Window Size Management
- **6 Predefined Modes**:
  - Compact (1000x700)
  - Medium (1200x800)
  - Large (1400x900) - Default
  - X-Large (1600x1000)
  - XX-Large (1800x1100)
  - Auto Fit Screen
- **Auto Fit Screen**: Automatically adjusts to 90% of screen size
- **Platform Detection**: Different defaults for Windows/Linux/macOS

### Settings Persistence
- **JSON Configuration**: Settings saved to `~/.local/share/ChessScrambler/settings.json`
- **Automatic Saving**: Settings saved immediately when changed
- **Load on Startup**: Settings restored when application starts
- **Validation**: Settings validated and corrected if invalid

## Technical Implementation

### AppSettings Model
```csharp
public class AppSettings : INotifyPropertyChanged
{
    public int BoardSize { get; set; } = 480;
    public int SquareSize { get; set; } = 60;
    public int PieceSize { get; set; } = 50;
    public int WindowWidth { get; set; } = 1400;
    public int WindowHeight { get; set; } = 900;
    public string WindowSizeMode { get; set; } = "Large";
}
```

### Automatic Calculations
- **Square Size**: `BoardSize / 8`
- **Piece Size**: `SquareSize * 0.87` (87% of square size)
- **Window Scaling**: Proportional adjustments based on board size

### Settings Management
- **LoadSettings()**: Load from JSON file on startup
- **SaveSettings()**: Save current settings to file
- **ResetToDefaults()**: Restore all settings to defaults
- **ValidateSettings()**: Ensure settings are within valid ranges

## User Interface

### Settings Panel
- **Board Size Selector**: Dropdown with 7 size options
- **Window Size Selector**: Dropdown with 6 window modes
- **Save Button**: Manual save option
- **Reset Button**: Restore defaults
- **Real-time Preview**: See changes immediately

### Visual Feedback
- **Immediate Updates**: Settings apply instantly
- **Window Resizing**: Window adjusts to new size immediately
- **Board Scaling**: Board resizes smoothly
- **Proportional Scaling**: All elements scale together

## User Experience
- **Intuitive Controls**: Easy-to-use dropdown selectors
- **Immediate Feedback**: See changes instantly
- **Persistent Settings**: Preferences remembered across sessions
- **Sensible Defaults**: Good default values for most users

## Advanced Features

### Platform-Specific Defaults
- **Windows**: Optimized for Windows display scaling
- **Linux**: Adjusted for Linux desktop environments
- **macOS**: Configured for macOS display characteristics

### Validation and Correction
- **Range Validation**: Ensure settings are within valid ranges
- **Auto-Correction**: Fix invalid settings automatically
- **Fallback Values**: Use defaults if settings are corrupted

### Settings File Management
- **Automatic Directory Creation**: Create settings directory if needed
- **Error Handling**: Graceful handling of file system errors
- **Backup and Recovery**: Fallback to defaults if loading fails

## Dependencies
- System.IO for file operations
- System.Text.Json for serialization
- System.ComponentModel for property change notifications
- Environment.GetFolderPath for user directory access
