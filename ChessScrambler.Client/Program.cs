using Avalonia;
using System;
using ChessScrambler.Client.Models;

namespace ChessScrambler.Client;

class Program
{
    public static bool EnableUILogging { get; private set; } = true;
    public static bool EnableGameLogging { get; private set; } = true;
    public static bool EnableFullLogging { get; private set; } = false;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        ParseCommandLineArgs(args);
        
        // Check if we should run console test instead of GUI
        if (args.Length > 0 && args[0] == "--quick-test")
        {
            Console.WriteLine("Test functionality removed. Use the GUI to import and test games.");
            return;
        }
        
        // Check if we should test COTA game loading
        if (args.Length > 0 && args[0] == "--test-cota")
        {
            TestCotaGameLoading();
            return;
        }
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    private static void TestCotaGameLoading()
    {
        Console.WriteLine("Testing COTA Game Loading...");
        
        try
        {
            // Load sample games from PGN file
            var sampleGamesPath = "sample_games.pgn";
            if (System.IO.File.Exists(sampleGamesPath))
            {
                Console.WriteLine($"Loading games from: {sampleGamesPath}");
                GameBank.ImportGamesFromFile(sampleGamesPath);
                
                var totalGames = GameBank.GetImportedGamesCount();
                Console.WriteLine($"Total games loaded: {totalGames}");
                
                // Try to get the COTA game
                var cotaGame = GameBank.GetCotaGame();
                if (cotaGame != null)
                {
                    Console.WriteLine($"COTA Game found!");
                    Console.WriteLine($"  Display Name: {cotaGame.GetDisplayName()}");
                    Console.WriteLine($"  White Player: {cotaGame.WhitePlayer}");
                    Console.WriteLine($"  Black Player: {cotaGame.BlackPlayer}");
                    Console.WriteLine($"  Number of moves: {cotaGame.Moves.Count}");
                    Console.WriteLine($"  Opening: {cotaGame.Opening}");
                    
                    // Test getting middlegame position
                    var fen = cotaGame.GetMiddlegamePositionFen();
                    Console.WriteLine($"  Middlegame FEN: {fen}");
                    
                    Console.WriteLine("COTA Game test completed successfully!");
                }
                else
                {
                    Console.WriteLine("COTA Game not found!");
                    
                    // List all games to see what we have
                    var allGames = GameBank.ImportedGames;
                    Console.WriteLine("Available games:");
                    foreach (var game in allGames)
                    {
                        Console.WriteLine($"  - {game.GetDisplayName()} (White: {game.WhitePlayer}, Black: {game.BlackPlayer})");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Sample games file not found: {sampleGamesPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private static void ParseCommandLineArgs(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--no-ui-logging":
                case "--no-ui":
                    EnableUILogging = false;
                    break;
                case "--no-game-logging":
                case "--no-game":
                    EnableGameLogging = false;
                    break;
                case "--ui-only":
                    EnableGameLogging = false;
                    break;
                case "--game-only":
                case "-go":
                    EnableUILogging = false;
                    break;
                case "--full-logs":
                case "-fl":
                    EnableUILogging = true;
                    EnableGameLogging = true;
                    EnableFullLogging = true;
                    break;
                case "--help":
                case "-h":
                    ShowHelp();
                    Environment.Exit(0);
                    break;
            }
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Chess Middlegame Practicer");
        Console.WriteLine();
        Console.WriteLine("Usage: ChessScrambler.Client [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --no-ui-logging, --no-ui    Disable UI event logging");
        Console.WriteLine("  --no-game-logging, --no-game Disable game event logging");
        Console.WriteLine("  --ui-only                    Enable only UI logging");
        Console.WriteLine("  --game-only, -go             Enable only game logging");
        Console.WriteLine("  --full-logs, -fl             Enable all logging (UI + Game + Debug)");
        Console.WriteLine("  --help, -h                   Show this help message");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  ChessScrambler.Client --game-only    # Only log game events");
        Console.WriteLine("  ChessScrambler.Client -go            # Only log game events (short form)");
        Console.WriteLine("  ChessScrambler.Client --full-logs    # Enable all logging");
        Console.WriteLine("  ChessScrambler.Client -fl            # Enable all logging (short form)");
        Console.WriteLine("  ChessScrambler.Client --no-ui        # Disable UI logging");
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
