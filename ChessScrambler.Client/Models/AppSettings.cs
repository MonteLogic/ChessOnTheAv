using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ChessScrambler.Client.Models;

/**
 * <summary>
 * Represents application settings that can be configured by the user.
 * </summary>
 */
public class AppSettings : INotifyPropertyChanged
{
    private int _boardSize = 480; // Default board size in pixels
    private int _squareSize = 60; // Default square size in pixels
    private int _pieceSize = 50; // Default piece size in pixels
    private int _windowWidth = 1400; // Default window width
    private int _windowHeight = 900; // Default window height
    private string _windowSizeMode = "Large"; // Window size mode

    public AppSettings()
    {
        // Don't auto-save in constructor to avoid overwriting loaded settings
    }

    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
        ".local", "share", "ChessScrambler");
    private static readonly string SettingsFilePath = Path.Combine(SettingsDirectory, "settings.json");

    /**
     * <summary>
     * Gets or sets the size of the chess board in pixels (width and height).
     * </summary>
     */
    public int BoardSize
    {
        get => _boardSize;
        set
        {
            if (SetProperty(ref _boardSize, value))
            {
                // Automatically calculate square size based on board size
                SquareSize = _boardSize / 8;
                // Automatically calculate piece size (slightly smaller than square)
                PieceSize = (int)(SquareSize * 0.83);
            }
        }
    }

    /**
     * <summary>
     * Gets or sets the size of each square in pixels.
     * This is automatically calculated based on BoardSize / 8.
     * </summary>
     */
    public int SquareSize
    {
        get => _squareSize;
        set => SetProperty(ref _squareSize, value);
    }

    /**
     * <summary>
     * Gets or sets the size of chess pieces in pixels.
     * This is automatically calculated as 83% of the square size.
     * </summary>
     */
    public int PieceSize
    {
        get => _pieceSize;
        set => SetProperty(ref _pieceSize, value);
    }

    /**
     * <summary>
     * Gets or sets the window width in pixels.
     * </summary>
     */
    public int WindowWidth
    {
        get => _windowWidth;
        set => SetProperty(ref _windowWidth, value);
    }

    /**
     * <summary>
     * Gets or sets the window height in pixels.
     * </summary>
     */
    public int WindowHeight
    {
        get => _windowHeight;
        set => SetProperty(ref _windowHeight, value);
    }

    /**
     * <summary>
     * Gets or sets the window size mode (Small, Medium, Large, X-Large, Full Screen).
     * </summary>
     */
    public string WindowSizeMode
    {
        get => _windowSizeMode;
        set
        {
            if (SetProperty(ref _windowSizeMode, value))
            {
                UpdateWindowSizeFromMode();
            }
        }
    }

    /**
     * <summary>
     * Gets the available board size options for the UI.
     * </summary>
     */
    public static int[] AvailableBoardSizes => new[] { 320, 400, 480, 560, 640, 720, 800 };

    /**
     * <summary>
     * Gets the display names for the available board sizes.
     * </summary>
     */
    public static string[] BoardSizeDisplayNames => new[] 
    { 
        "Small (320px)", 
        "Medium (400px)", 
        "Large (480px)", 
        "X-Large (560px)", 
        "XX-Large (640px)", 
        "XXX-Large (720px)", 
        "Huge (800px)" 
    };

    /**
     * <summary>
     * Gets the available window size modes.
     * </summary>
     */
    public static string[] AvailableWindowSizeModes => new[] 
    { 
        "Compact (1000x700)", 
        "Medium (1200x800)", 
        "Large (1400x900)", 
        "X-Large (1600x1000)", 
        "XX-Large (1800x1100)",
        "Auto Fit Screen"
    };

    /**
     * <summary>
     * Gets the display names for the available window sizes.
     * </summary>
     */
    public static string[] WindowSizeDisplayNames => new[] 
    { 
        "Compact (1000x700)", 
        "Medium (1200x800)", 
        "Large (1400x900)", 
        "X-Large (1600x1000)", 
        "XX-Large (1800x1100)",
        "Auto Fit Screen"
    };

    /**
     * <summary>
     * Updates the window size based on the selected mode.
     * </summary>
     */
    private void UpdateWindowSizeFromMode()
    {
        switch (_windowSizeMode)
        {
            case "Compact (1000x700)":
                WindowWidth = 1000;
                WindowHeight = 700;
                break;
            case "Medium (1200x800)":
                WindowWidth = 1200;
                WindowHeight = 800;
                break;
            case "Large (1400x900)":
                WindowWidth = 1400;
                WindowHeight = 900;
                break;
            case "X-Large (1600x1000)":
                WindowWidth = 1600;
                WindowHeight = 1000;
                break;
            case "XX-Large (1800x1100)":
                WindowWidth = 1800;
                WindowHeight = 1100;
                break;
            case "Auto Fit Screen":
                SetAutoFitScreenSize();
                break;
        }
    }

    /**
     * <summary>
     * Sets the window size to fit the screen with appropriate margins.
     * </summary>
     */
    private void SetAutoFitScreenSize()
    {
        try
        {
            // Get screen dimensions (this is a simplified approach)
            // In a real implementation, you'd use System.Windows.Forms.Screen or similar
            var screenWidth = 1920; // Default fallback
            var screenHeight = 1080; // Default fallback
            
            // Try to get actual screen size (this is platform-specific)
            // For now, we'll use reasonable defaults that work on most screens
            if (System.Environment.OSVersion.Platform == System.PlatformID.Win32NT)
            {
                // Windows - try to get screen size
                screenWidth = 1920;
                screenHeight = 1080;
            }
            else if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                // Linux - try to get screen size
                screenWidth = 1920;
                screenHeight = 1080;
            }
            
            // Set window size to 90% of screen size with minimum constraints
            WindowWidth = System.Math.Max(1200, (int)(screenWidth * 0.9));
            WindowHeight = System.Math.Max(800, (int)(screenHeight * 0.9));
        }
        catch
        {
            // Fallback to large size if screen detection fails
            WindowWidth = 1400;
            WindowHeight = 900;
        }
    }

    /**
     * <summary>
     * Occurs when a property value changes.
     * </summary>
     */
    public event PropertyChangedEventHandler PropertyChanged;

    /**
     * <summary>
     * Raises the PropertyChanged event for the specified property.
     * </summary>
     * <param name="propertyName">The name of the property that changed. If null, the caller member name is used.</param>
     */
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /**
     * <summary>
     * Sets the property value and raises PropertyChanged if the value has changed.
     * </summary>
     * <typeparam name="T">The type of the property.</typeparam>
     * <param name="field">The backing field for the property.</param>
     * <param name="value">The new value for the property.</param>
     * <param name="propertyName">The name of the property. If null, the caller member name is used.</param>
     * <returns>True if the property value was changed; otherwise, false.</returns>
     */
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        
        // Auto-save settings when they change
        SaveSettings();
        
        return true;
    }

    /**
     * <summary>
     * Loads settings from the settings file.
     * </summary>
     * <returns>A new AppSettings instance with loaded values, or default values if loading fails.</returns>
     */
    public static AppSettings LoadSettings()
    {
        try
        {
            Console.WriteLine($"[SETTINGS] Attempting to load settings from: {SettingsFilePath}");
            
            if (File.Exists(SettingsFilePath))
            {
                Console.WriteLine("[SETTINGS] Settings file found, loading...");
                var json = File.ReadAllText(SettingsFilePath);
                
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                
                var settings = JsonSerializer.Deserialize<AppSettings>(json, options);
                if (settings != null)
                {
                    Console.WriteLine($"[SETTINGS] Settings loaded successfully - Board: {settings.BoardSize}px, Window: {settings.WindowWidth}x{settings.WindowHeight}, Mode: {settings.WindowSizeMode}");
                    // Ensure settings are valid
                    settings.ValidateSettings();
                    return settings;
                }
            }
            else
            {
                Console.WriteLine("[SETTINGS] Settings file not found, using defaults");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SETTINGS] Error loading settings: {ex.Message}");
        }

        // Return default settings if loading fails
        var defaultSettings = new AppSettings();
        Console.WriteLine("[SETTINGS] Using default settings");
        // Save default settings for first-time users
        defaultSettings.SaveSettings();
        return defaultSettings;
    }

    /**
     * <summary>
     * Saves the current settings to the settings file.
     * </summary>
     */
    public void SaveSettings()
    {
        try
        {
            Console.WriteLine($"[SETTINGS] Saving settings to: {SettingsFilePath}");
            Console.WriteLine($"[SETTINGS] Current values - Board: {_boardSize}px, Window: {_windowWidth}x{_windowHeight}, Mode: {_windowSizeMode}");
            
            // Ensure the settings directory exists
            Directory.CreateDirectory(SettingsDirectory);
            Console.WriteLine($"[SETTINGS] Settings directory created/verified: {SettingsDirectory}");

            // Serialize settings to JSON
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(SettingsFilePath, json);
            Console.WriteLine("[SETTINGS] Settings saved successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SETTINGS] Error saving settings: {ex.Message}");
        }
    }

    /**
     * <summary>
     * Validates and corrects settings to ensure they are within acceptable ranges.
     * </summary>
     */
    private void ValidateSettings()
    {
        // Validate board size
        if (_boardSize < 320 || _boardSize > 800)
        {
            _boardSize = 480;
        }

        // Validate window size
        if (_windowWidth < 1000 || _windowWidth > 2000)
        {
            _windowWidth = 1400;
        }

        if (_windowHeight < 700 || _windowHeight > 1500)
        {
            _windowHeight = 900;
        }

        // Validate window size mode
        if (!AvailableWindowSizeModes.Contains(_windowSizeMode))
        {
            _windowSizeMode = "Large";
        }

        // Recalculate dependent values
        SquareSize = _boardSize / 8;
        PieceSize = (int)(SquareSize * 0.83);
    }

    /**
     * <summary>
     * Resets all settings to their default values.
     * </summary>
     */
    public void ResetToDefaults()
    {
        _boardSize = 480;
        _squareSize = 60;
        _pieceSize = 50;
        _windowWidth = 1400;
        _windowHeight = 900;
        _windowSizeMode = "Large";

        OnPropertyChanged(nameof(BoardSize));
        OnPropertyChanged(nameof(SquareSize));
        OnPropertyChanged(nameof(PieceSize));
        OnPropertyChanged(nameof(WindowWidth));
        OnPropertyChanged(nameof(WindowHeight));
        OnPropertyChanged(nameof(WindowSizeMode));

        SaveSettings();
    }
}
