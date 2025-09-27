using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using ChessScrambler.Client.Models;

namespace ChessScrambler.Client.Converters;

public class ChessSquareConverter : IValueConverter
{
    public static readonly ChessSquareConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isLightSquare)
        {
            return isLightSquare ? new SolidColorBrush(Color.FromRgb(240, 217, 181)) : new SolidColorBrush(Color.FromRgb(181, 136, 99));
        }
        return new SolidColorBrush(Colors.White);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class PieceColorConverter : IValueConverter
{
    public static readonly PieceColorConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is PieceColor color)
        {
            return color == PieceColor.White ? 
                new SolidColorBrush(Color.FromRgb(255, 255, 255)) : // Pure white for white pieces
                new SolidColorBrush(Color.FromRgb(50, 50, 50));     // Dark gray for black pieces
        }
        return new SolidColorBrush(Colors.Black);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
