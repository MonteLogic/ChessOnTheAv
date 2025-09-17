using System;

namespace ChessScrambler.Client.Models;

public enum PieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}

public enum PieceColor
{
    White,
    Black
}

public class ChessPiece
{
    public PieceType Type { get; set; }
    public PieceColor Color { get; set; }
    public bool HasMoved { get; set; } = false;
    public string Symbol => GetSymbol();

    public ChessPiece(PieceType type, PieceColor color)
    {
        Type = type;
        Color = color;
    }

    public string GetSymbol()
    {
        return Type switch
        {
            PieceType.Pawn => Color == PieceColor.White ? "♙" : "♟",
            PieceType.Rook => Color == PieceColor.White ? "♖" : "♜",
            PieceType.Knight => Color == PieceColor.White ? "♘" : "♞",
            PieceType.Bishop => Color == PieceColor.White ? "♗" : "♝",
            PieceType.Queen => Color == PieceColor.White ? "♕" : "♛",
            PieceType.King => Color == PieceColor.White ? "♔" : "♚",
            _ => "?"
        };
    }

    public string GetNotation()
    {
        return Type switch
        {
            PieceType.Pawn => "",
            PieceType.Rook => "R",
            PieceType.Knight => "N",
            PieceType.Bishop => "B",
            PieceType.Queen => "Q",
            PieceType.King => "K",
            _ => "?"
        };
    }
}
