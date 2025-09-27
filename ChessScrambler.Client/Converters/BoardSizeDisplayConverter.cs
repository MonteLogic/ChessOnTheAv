using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ChessScrambler.Client.Converters;

/**
 * <summary>
 * Converts board size values to their display names for the ComboBox.
 * </summary>
 */
public class BoardSizeDisplayConverter : IValueConverter
{
    public static readonly BoardSizeDisplayConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int size)
        {
            return size switch
            {
                320 => "Small (320px)",
                400 => "Medium (400px)",
                480 => "Large (480px)",
                560 => "X-Large (560px)",
                640 => "XX-Large (640px)",
                720 => "XXX-Large (720px)",
                800 => "Huge (800px)",
                _ => $"{size}px"
            };
        }
        return value?.ToString() ?? "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            // Extract the number from the display string
            var parts = str.Split('(');
            if (parts.Length > 1)
            {
                var sizePart = parts[1].Replace("px)", "").Trim();
                if (int.TryParse(sizePart, out int size))
                {
                    return size;
                }
            }
        }
        return 480; // Default fallback
    }
}
