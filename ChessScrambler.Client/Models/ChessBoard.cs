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
    private ChessPiece[,] _board;
    private List<Move> _moveHistory;
    private PieceColor _currentPlayer;

    public PieceColor CurrentPlayer => _currentPlayer;
    public List<Move> MoveHistory => _moveHistory;
    public bool IsGameOver { get; private set; }
    public PieceColor? Winner { get; private set; }

    public ChessBoard()
    {
        _board = new ChessPiece[8, 8];
        _moveHistory = new List<Move>();
        _currentPlayer = PieceColor.White;
        InitializeBoard();
    }

    public ChessBoard(string fen)
    {
        _board = new ChessPiece[8, 8];
        _moveHistory = new List<Move>();
        _currentPlayer = PieceColor.White;
        LoadFromFen(fen);
    }

    private void InitializeBoard()
    {
        // Initialize the board with starting position
        // White pieces
        _board[7, 0] = new ChessPiece(PieceType.Rook, PieceColor.White);
        _board[7, 1] = new ChessPiece(PieceType.Knight, PieceColor.White);
        _board[7, 2] = new ChessPiece(PieceType.Bishop, PieceColor.White);
        _board[7, 3] = new ChessPiece(PieceType.Queen, PieceColor.White);
        _board[7, 4] = new ChessPiece(PieceType.King, PieceColor.White);
        _board[7, 5] = new ChessPiece(PieceType.Bishop, PieceColor.White);
        _board[7, 6] = new ChessPiece(PieceType.Knight, PieceColor.White);
        _board[7, 7] = new ChessPiece(PieceType.Rook, PieceColor.White);

        for (int col = 0; col < 8; col++)
        {
            _board[6, col] = new ChessPiece(PieceType.Pawn, PieceColor.White);
        }

        // Black pieces
        _board[0, 0] = new ChessPiece(PieceType.Rook, PieceColor.Black);
        _board[0, 1] = new ChessPiece(PieceType.Knight, PieceColor.Black);
        _board[0, 2] = new ChessPiece(PieceType.Bishop, PieceColor.Black);
        _board[0, 3] = new ChessPiece(PieceType.Queen, PieceColor.Black);
        _board[0, 4] = new ChessPiece(PieceType.King, PieceColor.Black);
        _board[0, 5] = new ChessPiece(PieceType.Bishop, PieceColor.Black);
        _board[0, 6] = new ChessPiece(PieceType.Knight, PieceColor.Black);
        _board[0, 7] = new ChessPiece(PieceType.Rook, PieceColor.Black);

        for (int col = 0; col < 8; col++)
        {
            _board[1, col] = new ChessPiece(PieceType.Pawn, PieceColor.Black);
        }
    }

    private void LoadFromFen(string fen)
    {
        // Simple FEN parser for basic positions
        var parts = fen.Split(' ');
        var boardPart = parts[0];
        var currentPlayerPart = parts.Length > 1 ? parts[1] : "w";

        _currentPlayer = currentPlayerPart == "w" ? PieceColor.White : PieceColor.Black;

        var row = 0;
        var col = 0;

        foreach (char c in boardPart)
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
                var pieceType = GetPieceTypeFromChar(c);
                var pieceColor = char.IsUpper(c) ? PieceColor.White : PieceColor.Black;
                _board[row, col] = new ChessPiece(pieceType, pieceColor);
                col++;
            }
        }
    }

    private PieceType GetPieceTypeFromChar(char c)
    {
        var upper = char.ToUpper(c);
        return upper switch
        {
            'P' => PieceType.Pawn,
            'R' => PieceType.Rook,
            'N' => PieceType.Knight,
            'B' => PieceType.Bishop,
            'Q' => PieceType.Queen,
            'K' => PieceType.King,
            _ => throw new ArgumentException($"Unknown piece character: {c}")
        };
    }

    public ChessPiece GetPiece(int row, int col)
    {
        if (row < 0 || row >= 8 || col < 0 || col >= 8)
            return null;

        return _board[row, col];
    }

    public ChessPiece GetPiece(Position position)
    {
        return GetPiece(position.Row, position.Column);
    }

    public void SetPiece(int row, int col, ChessPiece piece)
    {
        if (row >= 0 && row < 8 && col >= 0 && col < 8)
        {
            _board[row, col] = piece;
        }
    }

    public void SetPiece(Position position, ChessPiece piece)
    {
        SetPiece(position.Row, position.Column, piece);
    }

    public bool IsValidMove(Move move)
    {
        if (move.From.Row < 0 || move.From.Row >= 8 || move.From.Column < 0 || move.From.Column >= 8)
            return false;
        if (move.To.Row < 0 || move.To.Row >= 8 || move.To.Column < 0 || move.To.Column >= 8)
            return false;

        var piece = GetPiece(move.From);
        if (piece == null || piece.Color != _currentPlayer)
            return false;

        // Basic move validation - this is a simplified version
        // In a real implementation, you'd want more sophisticated validation
        return true;
    }

    public bool MakeMove(Move move)
    {
        if (!IsValidMove(move))
            return false;

        var piece = GetPiece(move.From);
        if (piece == null)
            return false;

        // Check if it's a capture
        var targetPiece = GetPiece(move.To);
        move.IsCapture = targetPiece != null;

        // Make the move
        SetPiece(move.To, piece);
        SetPiece(move.From, null);

        // Update piece state
        piece.HasMoved = true;

        // Add to move history
        _moveHistory.Add(move);

        // Switch players
        _currentPlayer = _currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;

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
                var piece = _board[row, col];
                if (piece == null)
                {
                    emptyCount++;
                }
                else
                {
                    if (emptyCount > 0)
                    {
                        fen += emptyCount.ToString();
                        emptyCount = 0;
                    }
                    fen += GetCharFromPiece(piece);
                }
            }
            if (emptyCount > 0)
            {
                fen += emptyCount.ToString();
            }
            if (row < 7)
            {
                fen += "/";
            }
        }

        fen += " " + (_currentPlayer == PieceColor.White ? "w" : "b");
        return fen;
    }

    private char GetCharFromPiece(ChessPiece piece)
    {
        var c = piece.Type switch
        {
            PieceType.Pawn => 'p',
            PieceType.Rook => 'r',
            PieceType.Knight => 'n',
            PieceType.Bishop => 'b',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _ => '?'
        };

        return piece.Color == PieceColor.White ? char.ToUpper(c) : c;
    }

    public List<Move> GetValidMoves(Position from)
    {
        var moves = new List<Move>();
        var piece = GetPiece(from);
        if (piece == null || piece.Color != _currentPlayer)
            return moves;

        // This is a simplified version - in a real implementation,
        // you'd want to generate all valid moves for the piece
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var to = new Position(row, col);
                var move = new Move(from, to);
                if (IsValidMove(move))
                {
                    moves.Add(move);
                }
            }
        }

        return moves;
    }
}