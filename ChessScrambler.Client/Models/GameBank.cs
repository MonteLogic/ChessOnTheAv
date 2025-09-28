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
        private static readonly object _lock = new object();
        
        // The default game (third game from sample_games.pgn)
        private static ImportedGame? _defaultGame = null;

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
                
                // Set the third game as the default if we have at least 3 games
                if (_importedGames.Count >= 3 && _defaultGame == null)
                {
                    _defaultGame = _importedGames[2]; // Third game (index 2)
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine($"[GAME] Set default game to: {_defaultGame.GetDisplayName()}");
                    }
                }
                
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

        public static ImportedGame GetDefaultMiddlegamePosition()
        {
            lock (_lock)
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] GetDefaultMiddlegamePosition called");
                }
                
                if (_defaultGame == null)
                {
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine("[GAME] No default game set, throwing exception");
                    }
                    throw new InvalidOperationException("No default game available. Please import games first.");
                }

                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Using default game: {_defaultGame.GetDisplayName()}");
                    Console.WriteLine($"[GAME] Default game has {_defaultGame.Moves.Count} moves");
                }
                
                // Initialize the middlegame positions for this game if not already done
                _defaultGame.InitializeMiddlegamePositions();
                
                return _defaultGame;
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
                _defaultGame = null;
            }
        }

        public static int GetImportedGamesCount()
        {
            lock (_lock)
            {
                return _importedGames.Count;
            }
        }

        public static ImportedGame? GetCotaGame()
        {
            lock (_lock)
            {
                return _importedGames.FirstOrDefault(g => 
                    g.WhitePlayer.Contains("COTA Player 1", StringComparison.OrdinalIgnoreCase) ||
                    g.BlackPlayer.Contains("COTA Player 1", StringComparison.OrdinalIgnoreCase));
            }
        }

        private static List<ImportedGame> ParsePgnContent(string pgnContent)
        {
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] ParsePgnContent called with content length: {pgnContent?.Length ?? 0}");
            }
            
            var games = new List<ImportedGame>();
            var gameBlocks = SplitPgnIntoGames(pgnContent ?? string.Empty);
            
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
                
            var movePattern = @"(\d+\.\s*)?([NBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:=[NBRQ])?[+#]?|O-O-O|O-O|0-0-0|0-0)";
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
            
            // Debug: Show first few moves to verify parsing
            if (Program.EnableGameLogging && moves.Count > 0)
            {
                Console.WriteLine($"[GAME] First 10 moves: {string.Join(", ", moves.Take(10))}");
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
        
        // Pre-computed middlegame positions for this game
        public List<string> MiddlegamePositions { get; set; } = new List<string>();

        public ImportedGame()
        {
            Moves = new List<string>();
            MiddlegamePositions = new List<string>();
        }

        public void InitializeMiddlegamePositions()
        {
            if (MiddlegamePositions.Count > 0)
                return;

            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Initializing middlegame positions for game: {GetDisplayName()}");
            }

            // Generate 3-5 specific middlegame positions based on the actual game moves
            var positionsToGenerate = Math.Min(5, Math.Max(3, Moves.Count / 10));
            
            for (int i = 0; i < positionsToGenerate; i++)
            {
                try
                {
                    // Generate a middlegame position based on the actual game moves
                    var fen = GenerateMiddlegamePositionFromGame(i);
                    MiddlegamePositions.Add(fen);
                    
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine($"[GAME] Generated middlegame position {i + 1}: {fen}");
                    }
                }
                catch (Exception ex)
                {
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine($"[GAME] Error generating middlegame position {i + 1}: {ex.Message}");
                    }
                }
            }
            
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Initialized {MiddlegamePositions.Count} middlegame positions");
            }
        }

        private string GenerateMiddlegamePositionFromGame(int positionIndex)
        {
            // Create a chess game and make moves from the actual game to reach a middlegame position
            var chessGame = new ChessDotNet.ChessGame();
            
            // Calculate how many moves to make to reach a middlegame position
            // Use different move counts for different positions
            var moveCounts = new int[] { 8, 12, 16, 20, 24 };
            var moveCount = moveCounts[Math.Min(positionIndex, moveCounts.Length - 1)];
            
            // Make moves from the actual game
            for (int i = 0; i < Math.Min(moveCount, Moves.Count); i++)
            {
                try
                {
                    var move = Moves[i];
                    var validMoves = chessGame.GetValidMoves(chessGame.WhoseTurn).ToList();
                    
                    // Try to find a matching move
                    var matchingMove = validMoves.FirstOrDefault(m => 
                        m.ToString().Contains(move) || move.Contains(m.ToString()));
                    
                    if (matchingMove != null)
                    {
                        chessGame.MakeMove(matchingMove, true);
                    }
                    else
                    {
                        // If no exact match, make a random valid move
                        if (validMoves.Count > 0)
                        {
                            var randomMove = validMoves[0]; // Use first move as fallback
                            chessGame.MakeMove(randomMove, true);
                        }
                    }
                }
                catch
                {
                    // If move fails, break and return current position
                    break;
                }
            }
            
            return chessGame.GetFen();
        }

        public string GetDisplayName()
        {
            return $"{WhitePlayer} vs {BlackPlayer} ({Date})";
        }

        public string GetMiddlegamePositionFen(int positionIndex = 0)
        {
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] GetMiddlegamePositionFen called for game: {GetDisplayName()}");
            }
            
            // Initialize middlegame positions if not already done
            InitializeMiddlegamePositions();
            
            if (MiddlegamePositions.Count == 0)
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine("[GAME] No middlegame positions available, returning starting position");
                }
                return "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            }

            // Select a specific middlegame position (not random)
            var selectedIndex = positionIndex % MiddlegamePositions.Count;
            var selectedPosition = MiddlegamePositions[selectedIndex];
            
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Selected middlegame position {selectedIndex}: {selectedPosition}");
            }
            
            return selectedPosition;
        }
    }
}