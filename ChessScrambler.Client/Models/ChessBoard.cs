using System;
using System.Collections.Generic;
using System.Linq;
using ChessDotNet;

namespace ChessScrambler.Client.Models
{
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
            return $"{(char)('A' + Column)}{8 - Row}";
        }

        public static Position? FromAlgebraicNotation(string notation)
        {
            if (notation?.Length != 2) return null;
            var column = notation[0] - 'A';
            var row = 8 - (notation[1] - '0');
            return new Position(row, column);
        }

        public ChessDotNet.Position ToChessDotNetPosition()
        {
            return new ChessDotNet.Position(GetAlgebraicNotation());
        }

        public static Position FromChessDotNetPosition(ChessDotNet.Position chessDotNetPos)
        {
            return FromAlgebraicNotation(chessDotNetPos.ToString()) ?? new Position(0, 0);
        }

        public override bool Equals(object? obj)
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
        public PieceType PieceType { get; set; }
        public PieceType? PromotionPiece { get; set; }
        public bool IsCapture { get; set; }
        public bool IsCastling { get; set; }
        public bool IsEnPassant { get; set; }
        public bool IsCheck { get; set; }
        public bool IsCheckmate { get; set; }

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

            // For proper chess notation, we need to know the piece type
            // This will be set by the ChessBoard when creating the move
            var pieceNotation = GetPieceNotation(PieceType);
            var target = To.GetAlgebraicNotation();
            var capture = IsCapture ? "x" : "";
            var promotion = PromotionPiece.HasValue ? $"={GetPieceNotation(PromotionPiece.Value)}" : "";
            var checkSymbol = IsCheckmate ? "#" : (IsCheck ? "+" : "");

            return $"{pieceNotation}{capture}{target}{promotion}{checkSymbol}";
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
        private ChessGame _chessGame;
        private List<Move> _moveHistory;

        public PieceColor CurrentPlayer => _chessGame.WhoseTurn == Player.White ? PieceColor.White : PieceColor.Black;
        public List<Move> MoveHistory => _moveHistory;
        public bool IsGameOver => _chessGame.IsCheckmated(Player.White) || _chessGame.IsCheckmated(Player.Black) || 
                                 _chessGame.IsStalemated(Player.White) || _chessGame.IsStalemated(Player.Black);
        public PieceColor? Winner
        {
            get
            {
                if (_chessGame.IsCheckmated(Player.White)) return PieceColor.Black;
                if (_chessGame.IsCheckmated(Player.Black)) return PieceColor.White;
                return null;
            }
        }

        public ChessBoard()
        {
            _chessGame = new ChessGame();
            _moveHistory = new List<Move>();
        }

        public ChessBoard(string fen)
        {
            _chessGame = new ChessGame(fen);
            _moveHistory = new List<Move>();
        }

        public ChessPiece? GetPiece(int row, int col)
        {
            if (row < 0 || row >= 8 || col < 0 || col >= 8)
                return null;

            var position = new Position(row, col);
            var chessDotNetPos = position.ToChessDotNetPosition();
            
            var piece = _chessGame.GetPieceAt(chessDotNetPos);
            if (piece == null) return null;

            return new ChessPiece(ConvertPieceType(piece), ConvertPieceColor(piece.Owner));
        }

        public ChessPiece? GetPiece(Position position)
        {
            return GetPiece(position.Row, position.Column);
        }

        public void SetPiece(int row, int col, ChessPiece? piece)
        {
            throw new NotSupportedException("Setting pieces directly is not supported. Use MakeMove instead.");
        }

        public void SetPiece(Position position, ChessPiece? piece)
        {
            SetPiece(position.Row, position.Column, piece);
        }

        public bool IsValidMove(Move move)
        {
            Console.WriteLine($"[LOG] IsValidMove called: {move.From.Row},{move.From.Column} -> {move.To.Row},{move.To.Column}");
            
            if (move.From.Row < 0 || move.From.Row >= 8 || move.From.Column < 0 || move.From.Column >= 8)
            {
                Console.WriteLine("[LOG] Invalid move: From position out of bounds");
                return false;
            }
            if (move.To.Row < 0 || move.To.Row >= 8 || move.To.Column < 0 || move.To.Column >= 8)
            {
                Console.WriteLine("[LOG] Invalid move: To position out of bounds");
                return false;
            }

            var fromPos = move.From.ToChessDotNetPosition();
            var toPos = move.To.ToChessDotNetPosition();
            Console.WriteLine($"[LOG] Converted to ChessDotNet positions: {fromPos} -> {toPos}");
            
            var validMoves = _chessGame.GetValidMoves(_chessGame.WhoseTurn);
            Console.WriteLine($"[LOG] ChessDotNet valid moves count: {validMoves.Count}");
            Console.WriteLine($"[LOG] Current player: {_chessGame.WhoseTurn}");
            
            var isValid = validMoves.Any(m => m.OriginalPosition.ToString() == fromPos.ToString() && 
                                      m.NewPosition.ToString() == toPos.ToString());
            Console.WriteLine($"[LOG] Move validation result: {isValid}");
            
            return isValid;
        }

        public bool MakeMove(Move move)
        {
            Console.WriteLine($"[LOG] MakeMove called: {move.From.Row},{move.From.Column} -> {move.To.Row},{move.To.Column}");
            
            if (!IsValidMove(move))
            {
                Console.WriteLine("[LOG] MakeMove failed: Move is not valid");
                return false;
            }

            var fromPos = move.From.ToChessDotNetPosition();
            var toPos = move.To.ToChessDotNetPosition();
            Console.WriteLine($"[LOG] Making move with ChessDotNet: {fromPos} -> {toPos}");
            
            var validMoves = _chessGame.GetValidMoves(_chessGame.WhoseTurn);
            var chessDotNetMove = validMoves.FirstOrDefault(m => m.OriginalPosition.ToString() == fromPos.ToString() && 
                                                                m.NewPosition.ToString() == toPos.ToString());
            
            if (chessDotNetMove == null)
            {
                Console.WriteLine("[LOG] MakeMove failed: Could not find matching ChessDotNet move");
                return false;
            }

            Console.WriteLine($"[LOG] Found ChessDotNet move: {chessDotNetMove}");
            
            // Get piece type before making the move
            var fromPiece = _chessGame.GetPieceAt(fromPos);
            move.PieceType = ConvertPieceType(fromPiece);
            
            var targetPiece = _chessGame.GetPieceAt(toPos);
            move.IsCapture = targetPiece != null;
            Console.WriteLine($"[LOG] Target piece: {targetPiece?.ToString() ?? "null"}, IsCapture: {move.IsCapture}");

            var moveResult = _chessGame.MakeMove(chessDotNetMove, true);
            Console.WriteLine($"[LOG] ChessDotNet MakeMove result: {moveResult}");
            
            // Check if the move was successful by looking at the move result
            // ChessDotNet returns a combination of move types, so we check if it contains any valid move type
            bool success = moveResult.HasFlag(ChessDotNet.MoveType.Move) || 
                          moveResult.HasFlag(ChessDotNet.MoveType.Capture) ||
                          moveResult.HasFlag(ChessDotNet.MoveType.Castling) ||
                          moveResult.HasFlag(ChessDotNet.MoveType.EnPassant);
            
            Console.WriteLine($"[LOG] Move success: {success}");
            
            if (success)
            {
                // Check for check and checkmate after the move
                var opponentColor = _chessGame.WhoseTurn == Player.White ? PieceColor.Black : PieceColor.White;
                move.IsCheck = IsInCheck(opponentColor);
                move.IsCheckmate = IsCheckmate(opponentColor);
                
                _moveHistory.Add(move);
                Console.WriteLine($"[LOG] Move added to history. Total moves: {_moveHistory.Count}");
                Console.WriteLine($"[LOG] New current player: {_chessGame.WhoseTurn}");
                Console.WriteLine($"[LOG] Move is check: {move.IsCheck}, is checkmate: {move.IsCheckmate}");
            }
            
            return success;
        }

        public string GetFen()
        {
            return _chessGame.GetFen();
        }

        public List<Move> GetValidMoves(Position from)
        {
            var moves = new List<Move>();
            var fromPos = from.ToChessDotNetPosition();
            
            // Get the piece at the from position to determine piece type
            var piece = _chessGame.GetPieceAt(fromPos);
            var pieceType = ConvertPieceType(piece);
            
            var validMoves = _chessGame.GetValidMoves(_chessGame.WhoseTurn);
            var movesFromPosition = validMoves.Where(m => m.OriginalPosition.ToString() == fromPos.ToString());
            
            foreach (var chessMove in movesFromPosition)
            {
                var toPosition = Position.FromChessDotNetPosition(chessMove.NewPosition);
                var move = new Move(from, toPosition)
                {
                    PieceType = pieceType
                };
                moves.Add(move);
            }
            
            return moves;
        }

        public bool IsInCheck(PieceColor color)
        {
            var player = color == PieceColor.White ? Player.White : Player.Black;
            return _chessGame.IsInCheck(player);
        }

        public bool IsCheckmate(PieceColor color)
        {
            var player = color == PieceColor.White ? Player.White : Player.Black;
            return _chessGame.IsCheckmated(player);
        }

        public bool IsStalemate(PieceColor color)
        {
            var player = color == PieceColor.White ? Player.White : Player.Black;
            return _chessGame.IsStalemated(player);
        }

        private PieceType ConvertPieceType(Piece piece)
        {
            if (piece == null) return PieceType.Pawn;
            
            var pieceString = piece.ToString()?.ToUpper();
            
            if (pieceString?.Contains("PAWN") == true) return PieceType.Pawn;
            if (pieceString?.Contains("ROOK") == true) return PieceType.Rook;
            if (pieceString?.Contains("KNIGHT") == true) return PieceType.Knight;
            if (pieceString?.Contains("BISHOP") == true) return PieceType.Bishop;
            if (pieceString?.Contains("QUEEN") == true) return PieceType.Queen;
            if (pieceString?.Contains("KING") == true) return PieceType.King;
            
            return PieceType.Pawn;
        }

        private PieceColor ConvertPieceColor(Player player)
        {
            return player == Player.White ? PieceColor.White : PieceColor.Black;
        }

        public string ExportDebugState()
        {
            var debug = new System.Text.StringBuilder();
            debug.AppendLine("=== CHESS BOARD DEBUG STATE ===");
            debug.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            debug.AppendLine($"Current Player: {CurrentPlayer}");
            debug.AppendLine($"Game Over: {IsGameOver}");
            debug.AppendLine($"Winner: {Winner?.ToString() ?? "None"}");
            debug.AppendLine();
            
            debug.AppendLine("=== FEN STRING ===");
            debug.AppendLine(GetFen());
            debug.AppendLine();
            
            debug.AppendLine("=== BOARD STATE ===");
            for (int row = 0; row < 8; row++)
            {
                debug.Append($"{8 - row} ");
                for (int col = 0; col < 8; col++)
                {
                    var piece = GetPiece(row, col);
                    if (piece == null)
                    {
                        debug.Append(". ");
                    }
                    else
                    {
                        var pieceChar = GetPieceChar(piece);
                        debug.Append($"{pieceChar} ");
                    }
                }
                debug.AppendLine();
            }
            debug.AppendLine("  A B C D E F G H");
            debug.AppendLine();
            
            debug.AppendLine("=== MOVE HISTORY ===");
            if (_moveHistory.Count == 0)
            {
                debug.AppendLine("No moves made yet.");
            }
            else
            {
                for (int i = 0; i < _moveHistory.Count; i++)
                {
                    var move = _moveHistory[i];
                    debug.AppendLine($"Move {i + 1}: {move.GetNotation()} (from {move.From.Row},{move.From.Column} to {move.To.Row},{move.To.Column})");
                }
            }
            debug.AppendLine();
            
            debug.AppendLine("=== VALID MOVES FOR CURRENT PLAYER ===");
            var allValidMoves = new List<Move>();
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var piece = GetPiece(row, col);
                    if (piece != null && piece.Color == CurrentPlayer)
                    {
                        var fromPos = new Position(row, col);
                        var validMoves = GetValidMoves(fromPos);
                        allValidMoves.AddRange(validMoves);
                    }
                }
            }
            
            if (allValidMoves.Count == 0)
            {
                debug.AppendLine("No valid moves available.");
            }
            else
            {
                debug.AppendLine($"Total valid moves: {allValidMoves.Count}");
                foreach (var move in allValidMoves.Take(10)) // Show first 10 moves
                {
                    debug.AppendLine($"  {move.GetNotation()}");
                }
                if (allValidMoves.Count > 10)
                {
                    debug.AppendLine($"  ... and {allValidMoves.Count - 10} more");
                }
            }
            debug.AppendLine();
            
            debug.AppendLine("=== CHESSDOTNET INTERNAL STATE ===");
            try
            {
                debug.AppendLine($"ChessDotNet FEN: {_chessGame.GetFen()}");
                debug.AppendLine($"ChessDotNet Whose Turn: {_chessGame.WhoseTurn}");
                debug.AppendLine($"ChessDotNet Is In Check (White): {_chessGame.IsInCheck(Player.White)}");
                debug.AppendLine($"ChessDotNet Is In Check (Black): {_chessGame.IsInCheck(Player.Black)}");
                debug.AppendLine($"ChessDotNet Is Checkmated (White): {_chessGame.IsCheckmated(Player.White)}");
                debug.AppendLine($"ChessDotNet Is Checkmated (Black): {_chessGame.IsCheckmated(Player.Black)}");
                debug.AppendLine($"ChessDotNet Is Stalemated (White): {_chessGame.IsStalemated(Player.White)}");
                debug.AppendLine($"ChessDotNet Is Stalemated (Black): {_chessGame.IsStalemated(Player.Black)}");
            }
            catch (Exception ex)
            {
                debug.AppendLine($"Error accessing ChessDotNet state: {ex.Message}");
            }
            
            debug.AppendLine("=== END DEBUG STATE ===");
            return debug.ToString();
        }

        private char GetPieceChar(ChessPiece piece)
        {
            char baseChar = piece.Type switch
            {
                PieceType.Pawn => 'P',
                PieceType.Rook => 'R',
                PieceType.Knight => 'N',
                PieceType.Bishop => 'B',
                PieceType.Queen => 'Q',
                PieceType.King => 'K',
                _ => '?'
            };
            
            return piece.Color == PieceColor.White ? baseChar : char.ToLower(baseChar);
        }
    }
}