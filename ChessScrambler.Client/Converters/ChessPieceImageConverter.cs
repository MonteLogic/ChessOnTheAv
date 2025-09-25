using System;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ChessScrambler.Client.Models;

namespace ChessScrambler.Client.Converters;

public class ChessPieceImageConverter : IValueConverter
{
    public static readonly ChessPieceImageConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ChessPiece piece)
        {
            try
            {
                // Get the resource name for the piece
                var resourceName = GetResourceName(piece);
                
                // Load the Avalonia resource
                var uri = new Uri($"avares://ChessScrambler.Client/Assets/ChessPieces/{resourceName}");
                return new Bitmap(AssetLoader.Open(uri));
            }
            catch (Exception)
            {
                // Fallback to null if image loading fails
                return null;
            }
        }
        
        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static string GetResourceName(ChessPiece piece)
    {
        var colorPrefix = piece.Color == PieceColor.White ? "w" : "b";
        var pieceLetter = piece.Type switch
        {
            PieceType.Pawn => "P",
            PieceType.Rook => "R",
            PieceType.Knight => "N",
            PieceType.Bishop => "B",
            PieceType.Queen => "Q",
            PieceType.King => "K",
            _ => "P"
        };
        
        return $"{colorPrefix}{pieceLetter}.png";
    }
}
