using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChessScrambler.Client.Models
{
    public class GameBank
    {
        private static readonly List<ImportedGame> _importedGames = new List<ImportedGame>();
        private static readonly Random _random = new Random();
        private static readonly object _lock = new object();

        public static List<ImportedGame> ImportedGames 
        { 
            get 
            { 
                lock (_lock)
                {
                    return _importedGames.ToList();
                }
            } 
        }

        public static void ImportGamesFromPgn(string pgnContent)
        {
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] ImportGamesFromPgn called with content length: {pgnContent?.Length ?? 0}");
            }
            
            if (string.IsNullOrEmpty(pgnContent))
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine("[GAME] PGN content is null or empty, skipping import");
                }
                return;
            }
            
            var games = ParsePgnContent(pgnContent);
            
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Parsed {games.Count} games from PGN content");
            }
            
            lock (_lock)
            {
                _importedGames.AddRange(games);
                
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Total imported games after import: {_importedGames.Count}");
                }
            }
        }

        public static void ImportGamesFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"PGN file not found: {filePath}");

            var pgnContent = File.ReadAllText(filePath);
            if (!string.IsNullOrEmpty(pgnContent))
            {
                ImportGamesFromPgn(pgnContent);
            }
        }

        public static ImportedGame GetRandomMiddlegamePosition()
        {
            lock (_lock)
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] GetRandomMiddlegamePosition called, total games: {_importedGames.Count}");
                }
                
                if (_importedGames.Count == 0)
                {
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine("[GAME] No games imported, throwing exception");
                    }
                    throw new InvalidOperationException("No games imported. Please import games first.");

                }

                var randomIndex = _random.Next(_importedGames.Count);
                var game = _importedGames[randomIndex];
                
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Selected game at index {randomIndex}: {game.GetDisplayName()}");
                    Console.WriteLine($"[GAME] Selected game has {game.Moves.Count} moves");
                }
                
                return game;
            }
        }

        public static ImportedGame? GetGameById(string id)
        {
            lock (_lock)
            {
                return _importedGames.FirstOrDefault(g => g.Id == id);
            }
        }

        public static List<ImportedGame> GetGamesByPlayer(string playerName)
        {
            lock (_lock)
            {
                return _importedGames.Where(g => 
                    g.WhitePlayer.Contains(playerName, StringComparison.OrdinalIgnoreCase) ||
                    g.BlackPlayer.Contains(playerName, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        public static List<ImportedGame> GetGamesByOpening(string opening)
        {
            lock (_lock)
            {
                return _importedGames.Where(g => 
                    g.Opening.Contains(opening, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        public static void ClearGames()
        {
            lock (_lock)
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] ClearGames called, clearing {_importedGames.Count} games");
                }
                _importedGames.Clear();
            }
        }

        public static int GetImportedGamesCount()
        {
            lock (_lock)
            {
                return _importedGames.Count;
            }
        }

        private static List<ImportedGame> ParsePgnContent(string pgnContent)
        {
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] ParsePgnContent called with content length: {pgnContent?.Length ?? 0}");
            }
            
            var games = new List<ImportedGame>();
            var gameBlocks = SplitPgnIntoGames(pgnContent);
            
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Split PGN into {gameBlocks.Count} game blocks");
            }

            foreach (var gameBlock in gameBlocks)
            {
                try
                {
                    var game = ParseSingleGame(gameBlock);
                    if (game != null)
                    {
                        games.Add(game);
                        if (Program.EnableGameLogging)
                        {
                            Console.WriteLine($"[GAME] Successfully parsed game: {game.GetDisplayName()} with {game.Moves.Count} moves");
                        }
                    }
                    else
                    {
                        if (Program.EnableGameLogging)
                        {
                            Console.WriteLine("[GAME] ParseSingleGame returned null for a game block");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue with other games
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine($"[GAME] Error parsing game: {ex.Message}");
                        Console.WriteLine($"[GAME] Stack trace: {ex.StackTrace}");
                    }
                }
            }

            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] ParsePgnContent completed, returning {games.Count} games");
            }
            
            return games;
        }

        private static List<string> SplitPgnIntoGames(string pgnContent)
        {
            var games = new List<string>();
            if (string.IsNullOrEmpty(pgnContent))
                return games;
                
            var lines = pgnContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var currentGame = new List<string>();
            bool inGame = false;

            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] SplitPgnIntoGames: Processing {lines.Length} lines");
            }

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                if (trimmedLine.StartsWith("["))
                {
                    // If we're already in a game and encounter a new Event tag, finish the current game
                    if (inGame && currentGame.Count > 0 && trimmedLine.StartsWith("[Event"))
                    {
                        if (Program.EnableGameLogging)
                        {
                            Console.WriteLine($"[GAME] Finishing game with {currentGame.Count} lines");
                        }
                        games.Add(string.Join("\n", currentGame));
                        currentGame.Clear();
                    }
                    
                    if (!inGame)
                    {
                        inGame = true;
                    }
                    
                    currentGame.Add(trimmedLine);
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine($"[GAME] Adding tag to current game: {trimmedLine}");
                    }
                }
                else if (inGame && !string.IsNullOrWhiteSpace(trimmedLine))
                {
                    currentGame.Add(trimmedLine);
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine($"[GAME] Adding move line to current game: {trimmedLine.Substring(0, Math.Min(50, trimmedLine.Length))}...");
                    }
                }
            }

            if (inGame && currentGame.Count > 0)
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Finishing final game with {currentGame.Count} lines");
                }
                games.Add(string.Join("\n", currentGame));
            }

            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] SplitPgnIntoGames: Created {games.Count} game blocks");
            }

            return games;
        }

        private static ImportedGame? ParseSingleGame(string gameText)
        {
            if (string.IsNullOrEmpty(gameText))
                return null;
                
            var lines = gameText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var tags = new Dictionary<string, string>();
            var movesText = "";

            // Parse tags
            foreach (var line in lines)
            {
                if (line.StartsWith("["))
                {
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine($"[GAME] Parsing tag line: {line}");
                    }
                    var match = Regex.Match(line, @"\[(\w+)\s+""([^""]+)""\]");
                    if (match.Success)
                    {
                        var key = match.Groups[1].Value;
                        var value = match.Groups[2].Value;
                        tags[key] = value;
                        if (Program.EnableGameLogging)
                        {
                            Console.WriteLine($"[GAME] Parsed tag: {key} = {value}");
                        }
                    }
                    else
                    {
                        if (Program.EnableGameLogging)
                        {
                            Console.WriteLine($"[GAME] Failed to parse tag line: {line}");
                        }
                    }
                }
                else
                {
                    movesText += line + " ";
                }
            }

            // Extract moves
            var moves = ParseMoves(movesText.Trim());
            if (moves.Count == 0)
                return null;

            // Create game
            var game = new ImportedGame
            {
                Id = Guid.NewGuid().ToString(),
                WhitePlayer = tags.GetValueOrDefault("White", "Unknown"),
                BlackPlayer = tags.GetValueOrDefault("Black", "Unknown"),
                Event = tags.GetValueOrDefault("Event", "Unknown Event"),
                Site = tags.GetValueOrDefault("Site", "Unknown Site"),
                Date = tags.GetValueOrDefault("Date", "????.??.??"),
                Round = tags.GetValueOrDefault("Round", "?"),
                Result = tags.GetValueOrDefault("Result", "*"),
                Opening = tags.GetValueOrDefault("ECO", ""),
                Moves = moves,
                FullPgn = gameText
            };

            return game;
        }

        private static List<string> ParseMoves(string movesText)
        {
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] ParseMoves called with text: {movesText?.Substring(0, Math.Min(100, movesText?.Length ?? 0))}...");
            }
            
            var moves = new List<string>();
            if (string.IsNullOrEmpty(movesText))
                return moves;
                
            var movePattern = @"(\d+\.\s*)?([NBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:=[NBRQ])?[+#]?|O-O-O|O-O)";
            var matches = Regex.Matches(movesText, movePattern);
            
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Found {matches.Count} move matches in text");
            }

            foreach (Match match in matches)
            {
                var move = match.Groups[2].Value.Trim();
                if (!string.IsNullOrEmpty(move))
                {
                    moves.Add(move);
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine($"[GAME] Added move: {move}");
                    }
                }
            }

            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] ParseMoves completed, returning {moves.Count} moves");
            }
            
            return moves;
        }
    }

    public class ImportedGame
    {
        public string Id { get; set; } = string.Empty;
        public string WhitePlayer { get; set; } = string.Empty;
        public string BlackPlayer { get; set; } = string.Empty;
        public string Event { get; set; } = string.Empty;
        public string Site { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Round { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string Opening { get; set; } = string.Empty;
        public List<string> Moves { get; set; } = new List<string>();
        public string FullPgn { get; set; } = string.Empty;

        public ImportedGame()
        {
            Moves = new List<string>();
        }

        public string GetDisplayName()
        {
            return $"{WhitePlayer} vs {BlackPlayer} ({Date})";
        }

        public string GetMiddlegamePositionFen(int moveNumber = -1)
        {
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] GetMiddlegamePositionFen called for game: {GetDisplayName()}");
                Console.WriteLine($"[GAME] Total moves available: {Moves.Count}");
            }
            
            if (Moves.Count == 0)
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine("[GAME] No moves available, returning starting position");
                }
                return "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            }

            // If no specific move number, choose a middlegame position (around move 15-25)
            if (moveNumber == -1)
            {
                var middlegameStart = Math.Max(15, Moves.Count / 3);
                var middlegameEnd = Math.Min(Moves.Count - 5, Moves.Count * 2 / 3);
                moveNumber = _random.Next(middlegameStart, middlegameEnd + 1);
                
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Selected random move number: {moveNumber} (range: {middlegameStart}-{middlegameEnd})");
                }
            }

            // Clamp to valid range
            moveNumber = Math.Max(0, Math.Min(moveNumber, Moves.Count - 1));
            
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Final move number: {moveNumber}");
            }

            // Create a chess game and replay moves up to the specified position
            var chessGame = new ChessDotNet.ChessGame();
            
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Starting position FEN: {chessGame.GetFen()}");
            }
            
            for (int i = 0; i <= moveNumber; i++)
            {
                try
                {
                    var move = Moves[i];
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine($"[GAME] Processing move {i + 1}: {move}");
                    }
                    
                    var validMoves = chessGame.GetValidMoves(chessGame.WhoseTurn);
                    var chessMove = FindMatchingMove(validMoves, move, chessGame.WhoseTurn);
                    
                    if (chessMove != null)
                    {
                        chessGame.MakeMove(chessMove, true);
                        if (Program.EnableGameLogging)
                        {
                            Console.WriteLine($"[GAME] Move {i + 1} applied successfully: {move}");
                        }
                    }
                    else
                    {
                        if (Program.EnableGameLogging)
                        {
                            Console.WriteLine($"[GAME] Move {i + 1} not found in valid moves, stopping replay");
                            Console.WriteLine($"[GAME] Available moves: {string.Join(", ", validMoves.Take(10).Select(m => m.ToString()))}");
                        }
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine($"[GAME] Error processing move {i + 1}: {ex.Message}");
                    }
                    // If move parsing fails, return the current position
                    break;
                }
            }

            var finalFen = chessGame.GetFen();
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Final FEN after {moveNumber + 1} moves: {finalFen}");
            }
            
            return finalFen;
        }

        private static readonly Random _random = new Random();
        
        private static ChessDotNet.Move? FindMatchingMove(IEnumerable<ChessDotNet.Move> validMoves, string pgnMove, ChessDotNet.Player currentPlayer)
        {
            // First try exact match
            var exactMatch = validMoves.FirstOrDefault(m => m.ToString() == pgnMove);
            if (exactMatch != null)
                return exactMatch;
            
            // Try to find by piece type and destination
            var moveStr = pgnMove.Trim();
            
            // Handle special moves
            if (moveStr == "O-O" || moveStr == "0-0")
            {
                return validMoves.FirstOrDefault(m => m.ToString().Contains("O-O"));
            }
            if (moveStr == "O-O-O" || moveStr == "0-0-0")
            {
                return validMoves.FirstOrDefault(m => m.ToString().Contains("O-O-O"));
            }
            
            // Try to match by destination square
            var destinationMatch = validMoves.FirstOrDefault(m => 
            {
                var moveStr2 = m.ToString();
                // Extract destination from ChessDotNet move (e.g., "E2-E4" -> "E4")
                var parts = moveStr2.Split('-');
                if (parts.Length == 2)
                {
                    var dest = parts[1];
                    return moveStr.EndsWith(dest, StringComparison.OrdinalIgnoreCase);
                }
                return false;
            });
            
            if (destinationMatch != null)
                return destinationMatch;
            
            // Try to match by piece type and destination
            if (moveStr.Length >= 2)
            {
                var destSquare = moveStr.Substring(moveStr.Length - 2);
                var pieceType = moveStr.Length > 2 ? moveStr[0] : 'P'; // Default to pawn
                
                return validMoves.FirstOrDefault(m =>
                {
                    var moveStr2 = m.ToString();
                    var parts = moveStr2.Split('-');
                    if (parts.Length == 2)
                    {
                        var dest = parts[1];
                        return dest.Equals(destSquare, StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                });
            }
            
            return null;
        }
    }
}
