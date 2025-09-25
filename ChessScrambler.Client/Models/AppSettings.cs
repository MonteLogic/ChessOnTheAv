using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        return true;
    }
}
