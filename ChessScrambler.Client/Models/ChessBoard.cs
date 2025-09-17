using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessScrambler.Client.Models;

public class Position
{
    public int Row { get; set; }
    public int Column { get; set; }

    public Position(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public string GetAlgebraicNotation()
    {
        return $"{(char)('a' + Column)}{8 - Row}";
    }

    public static Position FromAlgebraicNotation(string notation)
    {
        if (notation.Length != 2) return null;
        var column = notation[0] - 'a';
        var row = 8 - (notation[1] - '0');
        return new Position(row, column);
    }

    public override bool Equals(object obj)
    {
        return obj is Position position && Row == position.Row && Column == position.Column;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }
}

public class Move
{
    public Position From { get; set; }
    public Position To { get; set; }
    public PieceType? PromotionPiece { get; set; }
    public bool IsCapture { get; set; }
    public bool IsCastling { get; set; }
    public bool IsEnPassant { get; set; }

    public Move(Position from, Position to)
    {
        From = from;
        To = to;
    }

    public string GetNotation()
    {
        if (IsCastling)
        {
            return To.Column == 6 ? "O-O" : "O-O-O";
        }

        var piece = From.GetAlgebraicNotation();
        var target = To.GetAlgebraicNotation();
        var capture = IsCapture ? "x" : "";
        var promotion = PromotionPiece.HasValue ? $"={GetPieceNotation(PromotionPiece.Value)}" : "";

        return $"{piece}{capture}{target}{promotion}";
    }

    private static string GetPieceNotation(PieceType pieceType)
    {
        return pieceType switch
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

public class ChessBoard
{
    private ChessPiece[,] board;
    private PieceColor currentPlayer;
    private List<Move> moveHistory;
    private Position? enPassantTarget;

    public PieceColor CurrentPlayer => currentPlayer;
    public List<Move> MoveHistory => moveHistory;
    public bool IsGameOver { get; private set; }
    public PieceColor? Winner { get; private set; }

    public ChessBoard()
    {
        board = new ChessPiece[8, 8];
        currentPlayer = PieceColor.White;
        moveHistory = new List<Move>();
        SetupInitialPosition();
    }

    public ChessBoard(string fen)
    {
        board = new ChessPiece[8, 8];
        currentPlayer = PieceColor.White;
        moveHistory = new List<Move>();
        LoadFromFen(fen);
    }

    private void SetupInitialPosition()
    {
        // Place pawns
        for (int col = 0; col < 8; col++)
        {
            board[1, col] = new ChessPiece(PieceType.Pawn, PieceColor.Black);
            board[6, col] = new ChessPiece(PieceType.Pawn, PieceColor.White);
        }

        // Place other pieces
        var pieceOrder = new[] { PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook };
        
        for (int col = 0; col < 8; col++)
        {
            board[0, col] = new ChessPiece(pieceOrder[col], PieceColor.Black);
            board[7, col] = new ChessPiece(pieceOrder[col], PieceColor.White);
        }
    }

    private void LoadFromFen(string fen)
    {
        var parts = fen.Split(' ');
        var boardFen = parts[0];
        var currentPlayerFen = parts.Length > 1 ? parts[1] : "w";
        
        currentPlayer = currentPlayerFen == "w" ? PieceColor.White : PieceColor.Black;

        var row = 0;
        var col = 0;

        foreach (char c in boardFen)
        {
            if (c == '/')
            {
                row++;
                col = 0;
            }
            else if (char.IsDigit(c))
            {
                col += int.Parse(c.ToString());
            }
            else
            {
                var pieceType = char.ToLower(c) switch
                {
                    'p' => PieceType.Pawn,
                    'r' => PieceType.Rook,
                    'n' => PieceType.Knight,
                    'b' => PieceType.Bishop,
                    'q' => PieceType.Queen,
                    'k' => PieceType.King,
                    _ => throw new ArgumentException($"Invalid piece character: {c}")
                };

                var pieceColor = char.IsUpper(c) ? PieceColor.White : PieceColor.Black;
                board[row, col] = new ChessPiece(pieceType, pieceColor);
                col++;
            }
        }
    }

    public ChessPiece GetPiece(int row, int col)
    {
        if (row < 0 || row >= 8 || col < 0 || col >= 8)
            return null;
        return board[row, col];
    }

    public ChessPiece GetPiece(Position position)
    {
        return GetPiece(position.Row, position.Column);
    }

    public void SetPiece(int row, int col, ChessPiece piece)
    {
        if (row >= 0 && row < 8 && col >= 0 && col < 8)
        {
            board[row, col] = piece;
        }
    }

    public void SetPiece(Position position, ChessPiece piece)
    {
        SetPiece(position.Row, position.Column, piece);
    }

    public bool IsValidMove(Move move)
    {
        var piece = GetPiece(move.From);
        if (piece == null || piece.Color != currentPlayer)
            return false;

        // Basic validation - can be expanded with full chess rules
        return IsValidMoveForPiece(piece, move);
    }

    private bool IsValidMoveForPiece(ChessPiece piece, Move move)
    {
        var rowDiff = Math.Abs(move.To.Row - move.From.Row);
        var colDiff = Math.Abs(move.To.Column - move.From.Column);

        return piece.Type switch
        {
            PieceType.Pawn => IsValidPawnMove(piece, move, rowDiff, colDiff),
            PieceType.Rook => (rowDiff == 0 || colDiff == 0) && IsPathClear(move.From, move.To),
            PieceType.Knight => (rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2),
            PieceType.Bishop => rowDiff == colDiff && IsPathClear(move.From, move.To),
            PieceType.Queen => ((rowDiff == 0 || colDiff == 0) || (rowDiff == colDiff)) && IsPathClear(move.From, move.To),
            PieceType.King => rowDiff <= 1 && colDiff <= 1,
            _ => false
        };
    }

    private bool IsValidPawnMove(ChessPiece piece, Move move, int rowDiff, int colDiff)
    {
        var direction = piece.Color == PieceColor.White ? -1 : 1;
        var startRow = piece.Color == PieceColor.White ? 6 : 1;

        // Forward move
        if (colDiff == 0 && GetPiece(move.To) == null)
        {
            if (rowDiff == 1)
                return true;
            if (rowDiff == 2 && move.From.Row == startRow)
                return true;
        }

        // Capture move
        if (colDiff == 1 && rowDiff == 1)
        {
            var targetPiece = GetPiece(move.To);
            return targetPiece != null && targetPiece.Color != piece.Color;
        }

        return false;
    }

    private bool IsPathClear(Position from, Position to)
    {
        var rowStep = from.Row == to.Row ? 0 : (to.Row - from.Row) / Math.Abs(to.Row - from.Row);
        var colStep = from.Column == to.Column ? 0 : (to.Column - from.Column) / Math.Abs(to.Column - from.Column);

        var current = new Position(from.Row + rowStep, from.Column + colStep);
        while (current.Row != to.Row || current.Column != to.Column)
        {
            if (GetPiece(current) != null)
                return false;
            current = new Position(current.Row + rowStep, current.Column + colStep);
        }

        return true;
    }

    public bool MakeMove(Move move)
    {
        if (!IsValidMove(move))
            return false;

        var piece = GetPiece(move.From);
        var capturedPiece = GetPiece(move.To);

        // Record the move
        move.IsCapture = capturedPiece != null;
        moveHistory.Add(move);

        // Move the piece
        SetPiece(move.To, piece);
        SetPiece(move.From, null);
        piece.HasMoved = true;

        // Switch players
        currentPlayer = currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;

        return true;
    }

    public string GetFen()
    {
        var fen = "";
        for (int row = 0; row < 8; row++)
        {
            var emptyCount = 0;
            for (int col = 0; col < 8; col++)
            {
                var piece = board[row, col];
                if (piece == null)
                {
                    emptyCount++;
                }
                else
                {
                    if (emptyCount > 0)
                    {
                        fen += emptyCount;
                        emptyCount = 0;
                    }
                    var symbol = piece.GetSymbol();
                    fen += char.IsUpper(symbol[0]) ? char.ToLower(symbol[0]) : char.ToUpper(symbol[0]);
                }
            }
            if (emptyCount > 0)
                fen += emptyCount;
            if (row < 7)
                fen += "/";
        }

        fen += $" {(currentPlayer == PieceColor.White ? "w" : "b")}";
        return fen;
    }
}
