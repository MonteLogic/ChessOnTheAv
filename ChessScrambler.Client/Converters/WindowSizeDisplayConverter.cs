using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ChessScrambler.Client.Converters;

/**
 * <summary>
 * Converts window size mode values to their display names for the ComboBox.
 * </summary>
 */
public class WindowSizeDisplayConverter : IValueConverter
{
    public static readonly WindowSizeDisplayConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string mode)
        {
            return mode switch
            {
                "Compact (1000x700)" => "Compact (1000x700)",
                "Medium (1200x800)" => "Medium (1200x800)",
                "Large (1400x900)" => "Large (1400x900)",
                "X-Large (1600x1000)" => "X-Large (1600x1000)",
                "XX-Large (1800x1100)" => "XX-Large (1800x1100)",
                "Auto Fit Screen" => "Auto Fit Screen",
                _ => mode
            };
        }
        return value?.ToString() ?? "";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return str;
        }
        return "Large (1400x900)"; // Default fallback
    }
}
