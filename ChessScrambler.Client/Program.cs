using Avalonia;
using System;

namespace ChessScrambler.Client;

class Program
{
    public static bool EnableUILogging { get; private set; } = true;
    public static bool EnableGameLogging { get; private set; } = true;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        ParseCommandLineArgs(args);
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
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
        Console.WriteLine("  --help, -h                   Show this help message");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  ChessScrambler.Client --game-only    # Only log game events");
        Console.WriteLine("  ChessScrambler.Client -go            # Only log game events (short form)");
        Console.WriteLine("  ChessScrambler.Client --no-ui        # Disable UI logging");
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
