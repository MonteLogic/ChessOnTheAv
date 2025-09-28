using ChessScrambler.Client.Models;

namespace ChessScrambler.Tests.TestUtilities;

public static class ChessTestHelpers
{
    public static Move CreateMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        return new Move(new Position(fromRow, fromCol), new Position(toRow, toCol));
    }

    public static bool IsValidPawnMove(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol)
    {
        var move = CreateMove(fromRow, fromCol, toRow, toCol);
        return board.IsValidMove(move);
    }

    public static ChessPiece? GetPieceAt(ChessBoard board, int row, int col)
    {
        return board.GetPiece(row, col);
    }

    public static bool IsPieceAt(ChessBoard board, int row, int col, PieceType expectedType, PieceColor expectedColor)
    {
        var piece = board.GetPiece(row, col);
        return piece != null && piece.Type == expectedType && piece.Color == expectedColor;
    }

    public static string GetFenForPosition(ChessBoard board)
    {
        return board.GetFen();
    }
}
