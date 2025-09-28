using ChessScrambler.Client.Models;
using Xunit;
using FluentAssertions;

namespace ChessScrambler.Tests.UnitTests.Models;

public class ChessGameTests
{
    [Fact]
    public void ChessGame_Constructor_ShouldInitializeCorrectly()
    {
        // Act
        var game = new ChessGame();

        // Assert
        game.Id.Should().NotBeNullOrEmpty();
        game.Id.Should().StartWith("GAME_");
        game.Name.Should().StartWith("Game ");
        game.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
        game.LastPlayedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
        game.MoveHistory.Should().BeEmpty();
        game.CurrentMoveIndex.Should().Be(-1);
        game.CurrentPlayer.Should().Be(PieceColor.White);
        game.IsGameOver.Should().BeFalse();
        game.Winner.Should().BeNull();
        game.GameResult.Should().Be("*");
        game.Notes.Should().BeEmpty();
    }

    [Fact]
    public void ChessGame_WithInitialFen_ShouldSetCorrectFen()
    {
        // Arrange
        var initialFen = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";

        // Act
        var game = new ChessGame(initialFen);

        // Assert
        game.InitialFen.Should().Be(initialFen);
    }

    [Fact]
    public void AddMove_ShouldAddMoveToHistory()
    {
        // Arrange
        var game = new ChessGame();
        var move = new Move(new Position(6, 4), new Position(4, 4));

        // Act
        game.AddMove(move);

        // Assert
        game.MoveHistory.Should().HaveCount(1);
        game.MoveHistory.Should().Contain(move);
        game.CurrentMoveIndex.Should().Be(0);
        game.LastPlayedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void AddMove_MultipleMoves_ShouldUpdateCurrentMoveIndex()
    {
        // Arrange
        var game = new ChessGame();
        var move1 = new Move(new Position(6, 4), new Position(4, 4));
        var move2 = new Move(new Position(1, 4), new Position(3, 4));

        // Act
        game.AddMove(move1);
        game.AddMove(move2);

        // Assert
        game.MoveHistory.Should().HaveCount(2);
        game.CurrentMoveIndex.Should().Be(1);
    }

    [Fact]
    public void GoToMove_ValidIndex_ShouldUpdateCurrentMoveIndex()
    {
        // Arrange
        var game = new ChessGame();
        var move1 = new Move(new Position(6, 4), new Position(4, 4));
        var move2 = new Move(new Position(1, 4), new Position(3, 4));
        game.AddMove(move1);
        game.AddMove(move2);

        // Act
        game.GoToMove(0);

        // Assert
        game.CurrentMoveIndex.Should().Be(0);
    }

    [Fact]
    public void GoToMove_InvalidIndex_ShouldNotChangeIndex()
    {
        // Arrange
        var game = new ChessGame();
        var move = new Move(new Position(6, 4), new Position(4, 4));
        game.AddMove(move);
        var originalIndex = game.CurrentMoveIndex;

        // Act
        game.GoToMove(5); // Invalid index

        // Assert
        game.CurrentMoveIndex.Should().Be(originalIndex);
    }

    [Fact]
    public void GoToFirstMove_ShouldSetIndexToMinusOne()
    {
        // Arrange
        var game = new ChessGame();
        var move = new Move(new Position(6, 4), new Position(4, 4));
        game.AddMove(move);

        // Act
        game.GoToFirstMove();

        // Assert
        game.CurrentMoveIndex.Should().Be(-1);
    }

    [Fact]
    public void GoToLastMove_ShouldSetIndexToLastMove()
    {
        // Arrange
        var game = new ChessGame();
        var move1 = new Move(new Position(6, 4), new Position(4, 4));
        var move2 = new Move(new Position(1, 4), new Position(3, 4));
        game.AddMove(move1);
        game.AddMove(move2);

        // Act
        game.GoToLastMove();

        // Assert
        game.CurrentMoveIndex.Should().Be(1);
    }

    [Fact]
    public void GoToPreviousMove_ShouldDecrementIndex()
    {
        // Arrange
        var game = new ChessGame();
        var move1 = new Move(new Position(6, 4), new Position(4, 4));
        var move2 = new Move(new Position(1, 4), new Position(3, 4));
        game.AddMove(move1);
        game.AddMove(move2);

        // Act
        game.GoToPreviousMove();

        // Assert
        game.CurrentMoveIndex.Should().Be(0);
    }

    [Fact]
    public void GoToNextMove_ShouldIncrementIndex()
    {
        // Arrange
        var game = new ChessGame();
        var move1 = new Move(new Position(6, 4), new Position(4, 4));
        var move2 = new Move(new Position(1, 4), new Position(3, 4));
        game.AddMove(move1);
        game.GoToMove(0);

        // Act
        game.GoToNextMove();

        // Assert
        game.CurrentMoveIndex.Should().Be(1);
    }

    [Fact]
    public void CanGoBack_WithMoves_ShouldReturnTrue()
    {
        // Arrange
        var game = new ChessGame();
        var move = new Move(new Position(6, 4), new Position(4, 4));
        game.AddMove(move);

        // Act
        var canGoBack = game.CanGoBack;

        // Assert
        canGoBack.Should().BeTrue();
    }

    [Fact]
    public void CanGoBack_WithoutMoves_ShouldReturnFalse()
    {
        // Arrange
        var game = new ChessGame();

        // Act
        var canGoBack = game.CanGoBack;

        // Assert
        canGoBack.Should().BeFalse();
    }

    [Fact]
    public void CanGoForward_AtEnd_ShouldReturnFalse()
    {
        // Arrange
        var game = new ChessGame();
        var move = new Move(new Position(6, 4), new Position(4, 4));
        game.AddMove(move);

        // Act
        var canGoForward = game.CanGoForward;

        // Assert
        canGoForward.Should().BeFalse();
    }

    [Fact]
    public void CanGoForward_NotAtEnd_ShouldReturnTrue()
    {
        // Arrange
        var game = new ChessGame();
        var move1 = new Move(new Position(6, 4), new Position(4, 4));
        var move2 = new Move(new Position(1, 4), new Position(3, 4));
        game.AddMove(move1);
        game.AddMove(move2);
        game.GoToMove(0);

        // Act
        var canGoForward = game.CanGoForward;

        // Assert
        canGoForward.Should().BeTrue();
    }

    [Fact]
    public void GetMovesUpToCurrent_ShouldReturnCorrectMoves()
    {
        // Arrange
        var game = new ChessGame();
        var move1 = new Move(new Position(6, 4), new Position(4, 4));
        var move2 = new Move(new Position(1, 4), new Position(3, 4));
        game.AddMove(move1);
        game.AddMove(move2);
        game.GoToMove(0);

        // Act
        var moves = game.GetMovesUpToCurrent();

        // Assert
        moves.Should().HaveCount(1);
        moves.Should().Contain(move1);
    }

    [Fact]
    public void SetGameResult_WhiteWins_ShouldSetCorrectResult()
    {
        // Arrange
        var game = new ChessGame();

        // Act
        game.SetGameResult("1-0");

        // Assert
        game.GameResult.Should().Be("1-0");
        game.IsGameOver.Should().BeTrue();
        game.Winner.Should().Be(PieceColor.White);
    }

    [Fact]
    public void SetGameResult_BlackWins_ShouldSetCorrectResult()
    {
        // Arrange
        var game = new ChessGame();

        // Act
        game.SetGameResult("0-1");

        // Assert
        game.GameResult.Should().Be("0-1");
        game.IsGameOver.Should().BeTrue();
        game.Winner.Should().Be(PieceColor.Black);
    }

    [Fact]
    public void SetGameResult_Draw_ShouldSetCorrectResult()
    {
        // Arrange
        var game = new ChessGame();

        // Act
        game.SetGameResult("1/2-1/2");

        // Assert
        game.GameResult.Should().Be("1/2-1/2");
        game.IsGameOver.Should().BeTrue();
        game.Winner.Should().BeNull();
    }
}
