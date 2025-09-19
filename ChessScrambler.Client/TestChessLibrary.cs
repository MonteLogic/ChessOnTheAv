using System;
using ChessDotNet;

namespace ChessScrambler.Client
{
    public class TestChessLibrary
    {
        public static void TestChessLibraryAPI()
        {
            try
            {
                // Try to create a chess board
                var board = new ChessGame();
                Console.WriteLine("ChessGame created successfully");
                
                // Try to get the current position
                var fen = board.GetFen();
                Console.WriteLine($"Initial FEN: {fen}");
                
                // Try to get legal moves for white (current player)
                var moves = board.GetValidMoves(Player.White);
                Console.WriteLine($"Number of legal moves for White: {moves.Count}");
                
                // Try to make a move
                if (moves.Count > 0)
                {
                    var firstMove = moves[0];
                    Console.WriteLine($"First legal move: {firstMove}");
                    board.MakeMove(firstMove, true);
                    Console.WriteLine($"FEN after move: {board.GetFen()}");
                }
                
                // Check game status
                var isCheckWhite = board.IsInCheck(Player.White);
                var isCheckBlack = board.IsInCheck(Player.Black);
                var isCheckmateWhite = board.IsCheckmated(Player.White);
                var isCheckmateBlack = board.IsCheckmated(Player.Black);
                var isStalemateWhite = board.IsStalemated(Player.White);
                var isStalemateBlack = board.IsStalemated(Player.Black);
                
                Console.WriteLine($"White is in check: {isCheckWhite}");
                Console.WriteLine($"Black is in check: {isCheckBlack}");
                Console.WriteLine($"White is checkmated: {isCheckmateWhite}");
                Console.WriteLine($"Black is checkmated: {isCheckmateBlack}");
                Console.WriteLine($"White is stalemated: {isStalemateWhite}");
                Console.WriteLine($"Black is stalemated: {isStalemateBlack}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error testing ChessDotNet: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
