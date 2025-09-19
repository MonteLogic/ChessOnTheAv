using System;
using ChessDotNet;

namespace DebugTest
{
    class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("=== CHESS DEBUG EXPORT TEST ===");
                
                // Create a chess game and make some moves
                var game = new ChessGame();
                Console.WriteLine("Initial FEN: " + game.GetFen());
                
                // Get some moves and make them
                var moves = game.GetValidMoves(Player.White);
                Console.WriteLine($"Available moves: {moves.Count}");
                
                if (moves.Count > 0)
                {
                    // Make first move
                    var firstMove = moves[0];
                    Console.WriteLine($"Making move: {firstMove}");
                    var result1 = game.MakeMove(firstMove, true);
                    Console.WriteLine($"Move result: {result1}");
                    Console.WriteLine($"FEN after move: {game.GetFen()}");
                    
                    // Get moves for black
                    var blackMoves = game.GetValidMoves(Player.Black);
                    Console.WriteLine($"Black moves available: {blackMoves.Count}");
                    
                    if (blackMoves.Count > 0)
                    {
                        // Make a black move
                        var blackMove = blackMoves[0];
                        Console.WriteLine($"Making black move: {blackMove}");
                        var result2 = game.MakeMove(blackMove, true);
                        Console.WriteLine($"Black move result: {result2}");
                        Console.WriteLine($"FEN after black move: {game.GetFen()}");
                    }
                }
                
                Console.WriteLine("\n=== SIMULATING DEBUG EXPORT ===");
                
                // Simulate what the debug export would show
                Console.WriteLine("=== CHESS BOARD DEBUG STATE ===");
                Console.WriteLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"Current Player: {game.WhoseTurn}");
                Console.WriteLine($"Is In Check (White): {game.IsInCheck(Player.White)}");
                Console.WriteLine($"Is In Check (Black): {game.IsInCheck(Player.Black)}");
                Console.WriteLine($"Is Checkmated (White): {game.IsCheckmated(Player.White)}");
                Console.WriteLine($"Is Checkmated (Black): {game.IsCheckmated(Player.Black)}");
                Console.WriteLine($"Is Stalemated (White): {game.IsStalemated(Player.White)}");
                Console.WriteLine($"Is Stalemated (Black): {game.IsStalemated(Player.Black)}");
                Console.WriteLine();
                
                Console.WriteLine("=== FEN STRING ===");
                Console.WriteLine(game.GetFen());
                Console.WriteLine();
                
                Console.WriteLine("=== VALID MOVES FOR CURRENT PLAYER ===");
                var currentMoves = game.GetValidMoves(game.WhoseTurn);
                Console.WriteLine($"Total valid moves: {currentMoves.Count}");
                foreach (var move in currentMoves.Take(5))
                {
                    Console.WriteLine($"  {move.OriginalPosition} -> {move.NewPosition}");
                }
                if (currentMoves.Count > 5)
                {
                    Console.WriteLine($"  ... and {currentMoves.Count - 5} more");
                }
                
                Console.WriteLine("\n=== DEBUG EXPORT TEST COMPLETED ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
