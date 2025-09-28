using ChessScrambler.Client.Models;

namespace ChessScrambler.Tests.TestUtilities;

public static class TestDataBuilder
{
    public static ChessGame CreateSampleGame()
    {
        var game = new ChessGame();
        game.AddMove(new Move(new Position(6, 4), new Position(4, 4))); // e2-e4
        game.AddMove(new Move(new Position(1, 4), new Position(3, 4))); // e7-e5
        return game;
    }

    public static ChessBoard CreateChessBoardWithPosition(string fen)
    {
        return new ChessBoard(fen);
    }

    public static AppSettings CreateTestSettings()
    {
        return new AppSettings
        {
            BoardSize = 480,
            WindowWidth = 1400,
            WindowHeight = 900,
            WindowSizeMode = "Large"
        };
    }
}
