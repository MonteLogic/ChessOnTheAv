using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessScrambler.Client.Models
{
    public class ChessGame
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastPlayedAt { get; set; }
        public string InitialFen { get; set; }
        public List<Move> MoveHistory { get; set; }
        public int CurrentMoveIndex { get; set; }
        public PieceColor CurrentPlayer { get; set; }
        public bool IsGameOver { get; set; }
        public PieceColor? Winner { get; set; }
        public string GameResult { get; set; } // "1-0", "0-1", "1/2-1/2", "*"
        public string Notes { get; set; }

        public ChessGame()
        {
            Id = GenerateGameId();
            Name = $"Game {Id}";
            CreatedAt = DateTime.Now;
            LastPlayedAt = DateTime.Now;
            MoveHistory = new List<Move>();
            CurrentMoveIndex = -1;
            CurrentPlayer = PieceColor.White;
            IsGameOver = false;
            Winner = null;
            GameResult = "*";
            Notes = "";
        }

        public ChessGame(string initialFen) : this()
        {
            InitialFen = initialFen;
        }

        public ChessGame(string id, string name, string initialFen, List<Move> moves) : this()
        {
            Id = id;
            Name = name;
            InitialFen = initialFen;
            MoveHistory = moves ?? new List<Move>();
            CurrentMoveIndex = MoveHistory.Count - 1;
        }

        private static string GenerateGameId()
        {
            // Generate a unique game ID using timestamp and random component
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"GAME_{timestamp}_{random}";
        }

        public void AddMove(Move move)
        {
            // Remove any moves after the current index (when navigating back and adding new moves)
            if (CurrentMoveIndex < MoveHistory.Count - 1)
            {
                MoveHistory.RemoveRange(CurrentMoveIndex + 1, MoveHistory.Count - CurrentMoveIndex - 1);
            }

            MoveHistory.Add(move);
            CurrentMoveIndex = MoveHistory.Count - 1;
            LastPlayedAt = DateTime.Now;
        }

        public void GoToMove(int moveIndex)
        {
            if (moveIndex >= -1 && moveIndex < MoveHistory.Count)
            {
                CurrentMoveIndex = moveIndex;
                LastPlayedAt = DateTime.Now;
            }
        }

        public void GoToFirstMove()
        {
            GoToMove(-1);
        }

        public void GoToLastMove()
        {
            GoToMove(MoveHistory.Count - 1);
        }

        public void GoToPreviousMove()
        {
            if (CurrentMoveIndex > -1)
            {
                GoToMove(CurrentMoveIndex - 1);
            }
        }

        public void GoToNextMove()
        {
            if (CurrentMoveIndex < MoveHistory.Count - 1)
            {
                GoToMove(CurrentMoveIndex + 1);
            }
        }

        public bool CanGoBack => CurrentMoveIndex > -1;
        public bool CanGoForward => CurrentMoveIndex < MoveHistory.Count - 1;

        public List<Move> GetMovesUpToCurrent()
        {
            if (CurrentMoveIndex == -1)
                return new List<Move>();
            
            return MoveHistory.Take(CurrentMoveIndex + 1).ToList();
        }

        public string GetMoveHistoryText()
        {
            var moves = GetMovesUpToCurrent();
            var formattedMoves = new List<string>();
            
            for (int i = 0; i < moves.Count; i++)
            {
                if (i % 2 == 0)
                {
                    // White move - add move number
                    var moveNumber = (i / 2) + 1;
                    formattedMoves.Add($"{moveNumber}. {moves[i].GetNotation()}");
                }
                else
                {
                    // Black move - just add the move
                    formattedMoves.Add(moves[i].GetNotation());
                }
            }
            
            return string.Join(" ", formattedMoves);
        }

        public string GetCurrentPositionFen(ChessBoard board)
        {
            // This would need to be implemented to reconstruct the FEN from the initial position and moves
            // For now, we'll use the board's current FEN
            return board.GetFen();
        }

        public void SetGameResult(string result)
        {
            GameResult = result;
            IsGameOver = true;
            
            switch (result)
            {
                case "1-0":
                    Winner = PieceColor.White;
                    break;
                case "0-1":
                    Winner = PieceColor.Black;
                    break;
                case "1/2-1/2":
                    Winner = null;
                    break;
                default:
                    Winner = null;
                    break;
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Id}) - {MoveHistory.Count} moves - {GameResult}";
        }
    }
}
