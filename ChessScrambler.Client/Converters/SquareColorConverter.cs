using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ChessScrambler.Client.Converters;

public class SquareColorConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 3) return Colors.White;
        
        var isLightSquare = values[0] is bool light && light;
        var isSelected = values[1] is bool selected && selected;
        var isHighlighted = values[2] is bool highlighted && highlighted;
        
        if (isSelected)
        {
            return Colors.Yellow;
        }
        else if (isHighlighted)
        {
            return Colors.LightBlue;
        }
        else if (isLightSquare)
        {
            return Color.FromRgb(240, 217, 181); // Traditional light square color
        }
        else
        {
            return Color.FromRgb(181, 136, 99);  // Traditional dark square color
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
