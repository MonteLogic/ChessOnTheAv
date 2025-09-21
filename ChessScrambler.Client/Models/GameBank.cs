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

        public static List<ImportedGame> ImportedGames => _importedGames.ToList();

        public static void ImportGamesFromPgn(string pgnContent)
        {
            var games = ParsePgnContent(pgnContent);
            _importedGames.AddRange(games);
        }

        public static void ImportGamesFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"PGN file not found: {filePath}");

            var pgnContent = File.ReadAllText(filePath);
            ImportGamesFromPgn(pgnContent);
        }

        public static ImportedGame GetRandomMiddlegamePosition()
        {
            if (_importedGames.Count == 0)
                throw new InvalidOperationException("No games imported. Please import games first.");

            var game = _importedGames[_random.Next(_importedGames.Count)];
            return game;
        }

        public static ImportedGame GetGameById(string id)
        {
            return _importedGames.FirstOrDefault(g => g.Id == id);
        }

        public static List<ImportedGame> GetGamesByPlayer(string playerName)
        {
            return _importedGames.Where(g => 
                g.WhitePlayer.Contains(playerName, StringComparison.OrdinalIgnoreCase) ||
                g.BlackPlayer.Contains(playerName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public static List<ImportedGame> GetGamesByOpening(string opening)
        {
            return _importedGames.Where(g => 
                g.Opening.Contains(opening, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public static void ClearGames()
        {
            _importedGames.Clear();
        }

        private static List<ImportedGame> ParsePgnContent(string pgnContent)
        {
            var games = new List<ImportedGame>();
            var gameBlocks = SplitPgnIntoGames(pgnContent);

            foreach (var gameBlock in gameBlocks)
            {
                try
                {
                    var game = ParseSingleGame(gameBlock);
                    if (game != null)
                        games.Add(game);
                }
                catch (Exception ex)
                {
                    // Log error but continue with other games
                    Console.WriteLine($"Error parsing game: {ex.Message}");
                }
            }

            return games;
        }

        private static List<string> SplitPgnIntoGames(string pgnContent)
        {
            var games = new List<string>();
            var lines = pgnContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var currentGame = new List<string>();
            bool inGame = false;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                if (trimmedLine.StartsWith("["))
                {
                    if (inGame)
                    {
                        games.Add(string.Join("\n", currentGame));
                        currentGame.Clear();
                    }
                    inGame = true;
                    currentGame.Add(trimmedLine);
                }
                else if (inGame && !string.IsNullOrWhiteSpace(trimmedLine))
                {
                    currentGame.Add(trimmedLine);
                }
            }

            if (inGame && currentGame.Count > 0)
            {
                games.Add(string.Join("\n", currentGame));
            }

            return games;
        }

        private static ImportedGame ParseSingleGame(string gameText)
        {
            var lines = gameText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var tags = new Dictionary<string, string>();
            var movesText = "";

            // Parse tags
            foreach (var line in lines)
            {
                if (line.StartsWith("["))
                {
                    var match = Regex.Match(line, @"\[(\w+)\s+""([^""]+)""\]");
                    if (match.Success)
                    {
                        tags[match.Groups[1].Value] = match.Groups[2].Value;
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
            var moves = new List<string>();
            var movePattern = @"(\d+\.\s*)?([NBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:=[NBRQ])?[+#]?|O-O-O|O-O)";
            var matches = Regex.Matches(movesText, movePattern);

            foreach (Match match in matches)
            {
                var move = match.Groups[2].Value.Trim();
                if (!string.IsNullOrEmpty(move))
                {
                    moves.Add(move);
                }
            }

            return moves;
        }
    }

    public class ImportedGame
    {
        public string Id { get; set; }
        public string WhitePlayer { get; set; }
        public string BlackPlayer { get; set; }
        public string Event { get; set; }
        public string Site { get; set; }
        public string Date { get; set; }
        public string Round { get; set; }
        public string Result { get; set; }
        public string Opening { get; set; }
        public List<string> Moves { get; set; }
        public string FullPgn { get; set; }

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
            if (Moves.Count == 0)
                return "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

            // If no specific move number, choose a middlegame position (around move 15-25)
            if (moveNumber == -1)
            {
                var middlegameStart = Math.Max(15, Moves.Count / 3);
                var middlegameEnd = Math.Min(Moves.Count - 5, Moves.Count * 2 / 3);
                moveNumber = _random.Next(middlegameStart, middlegameEnd + 1);
            }

            // Clamp to valid range
            moveNumber = Math.Max(0, Math.Min(moveNumber, Moves.Count - 1));

            // Create a chess game and replay moves up to the specified position
            var chessGame = new ChessDotNet.ChessGame();
            
            for (int i = 0; i <= moveNumber; i++)
            {
                try
                {
                    var move = Moves[i];
                    var validMoves = chessGame.GetValidMoves(chessGame.WhoseTurn);
                    var chessMove = validMoves.FirstOrDefault(m => m.ToString() == move);
                    
                    if (chessMove != null)
                    {
                        chessGame.MakeMove(chessMove, true);
                    }
                }
                catch
                {
                    // If move parsing fails, return the current position
                    break;
                }
            }

            return chessGame.GetFen();
        }

        private static readonly Random _random = new Random();
    }
}
