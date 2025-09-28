using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ChessScrambler.Client.Models;

namespace ChessScrambler.Tests.UnitTests.Models;

public class ChessGameMoveHistoryTests : IDisposable
{
    private readonly StringWriter _consoleOutput;
    private readonly TextWriter _originalConsoleOut;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public ChessGameMoveHistoryTests()
    {
        // Capture console output
        _consoleOutput = new StringWriter();
        _originalConsoleOut = Console.Out;
        Console.SetOut(_consoleOutput);

        // Set up cancellation token for timeouts
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Dispose()
    {
        // Restore console output
        Console.SetOut(_originalConsoleOut);
        _consoleOutput?.Dispose();
        _cancellationTokenSource?.Dispose();
    }

    private string GetConsoleOutput()
    {
        return _consoleOutput.ToString();
    }

    private void ClearConsoleOutput()
    {
        _consoleOutput.GetStringBuilder().Clear();
    }

    [Fact]
    public void ChessGame_InitialState_ShouldHaveEmptyMoveHistory()
    {
        // Arrange & Act
        var game = new ChessGame();

        // Assert
        Assert.Empty(game.MoveHistory);
        Assert.Equal(-1, game.CurrentMoveIndex);
        Assert.False(game.CanGoBack);
        Assert.False(game.CanGoForward);
    }

    [Fact]
    public void AddMove_ShouldUpdateMoveHistory()
    {
        // Arrange
        var game = new ChessGame();
        var move = new Move(new Position(1, 4), new Position(3, 4)); // e2-e4

        // Act
        game.AddMove(move);

        // Assert
        Assert.Single(game.MoveHistory);
        Assert.Equal(0, game.CurrentMoveIndex);
        Assert.True(game.CanGoBack);  // Can go back from index 0
        Assert.False(game.CanGoForward); // Cannot go forward from last move
    }

    [Fact]
    public void AddMultipleMoves_ShouldUpdateCorrectly()
    {
        // Arrange
        var game = new ChessGame();
        var move1 = new Move(new Position(1, 4), new Position(3, 4)); // e2-e4
        var move2 = new Move(new Position(6, 4), new Position(4, 4)); // e7-e5

        // Act
        game.AddMove(move1);
        game.AddMove(move2);

        // Assert
        Assert.Equal(2, game.MoveHistory.Count);
        Assert.Equal(1, game.CurrentMoveIndex);
        Assert.True(game.CanGoBack);
        Assert.False(game.CanGoForward);
    }

    [Fact]
    public void GoToFirstMove_ShouldSetCorrectIndex()
    {
        // Arrange
        var game = new ChessGame();
        game.AddMove(new Move(new Position(1, 4), new Position(3, 4)));
        game.AddMove(new Move(new Position(6, 4), new Position(4, 4)));

        // Act
        game.GoToFirstMove();

        // Assert
        Assert.Equal(-1, game.CurrentMoveIndex);
        Assert.False(game.CanGoBack);
        Assert.True(game.CanGoForward);
    }

    [Fact]
    public void GoToLastMove_ShouldSetCorrectIndex()
    {
        // Arrange
        var game = new ChessGame();
        game.AddMove(new Move(new Position(1, 4), new Position(3, 4)));
        game.AddMove(new Move(new Position(6, 4), new Position(4, 4)));

        // Act
        game.GoToLastMove();

        // Assert
        Assert.Equal(1, game.CurrentMoveIndex);
        Assert.True(game.CanGoBack);
        Assert.False(game.CanGoForward);
    }

    [Fact]
    public void GoToPreviousMove_ShouldDecrementIndex()
    {
        // Arrange
        var game = new ChessGame();
        game.AddMove(new Move(new Position(1, 4), new Position(3, 4)));
        game.AddMove(new Move(new Position(6, 4), new Position(4, 4)));

        // Act
        game.GoToPreviousMove();

        // Assert
        Assert.Equal(0, game.CurrentMoveIndex);
        Assert.True(game.CanGoBack);  // Can go back from index 0
        Assert.True(game.CanGoForward); // Can go forward from index 0
    }

    [Fact]
    public void GoToNextMove_ShouldIncrementIndex()
    {
        // Arrange
        var game = new ChessGame();
        game.AddMove(new Move(new Position(1, 4), new Position(3, 4)));
        game.AddMove(new Move(new Position(6, 4), new Position(4, 4)));
        game.GoToFirstMove(); // Go to index -1

        // Act
        game.GoToNextMove();

        // Assert
        Assert.Equal(0, game.CurrentMoveIndex);
        Assert.True(game.CanGoBack);  // Can go back from index 0
        Assert.True(game.CanGoForward); // Can go forward from index 0
    }

    [Fact]
    public void GoToMove_WithValidIndex_ShouldSetCorrectIndex()
    {
        // Arrange
        var game = new ChessGame();
        game.AddMove(new Move(new Position(1, 4), new Position(3, 4)));
        game.AddMove(new Move(new Position(6, 4), new Position(4, 4)));

        // Act
        game.GoToMove(0);

        // Assert
        Assert.Equal(0, game.CurrentMoveIndex);
        Assert.True(game.CanGoBack);  // Can go back from index 0
        Assert.True(game.CanGoForward); // Can go forward from index 0
    }

    [Fact]
    public void GoToMove_WithInvalidIndex_ShouldNotChangeIndex()
    {
        // Arrange
        var game = new ChessGame();
        game.AddMove(new Move(new Position(1, 4), new Position(3, 4)));
        var originalIndex = game.CurrentMoveIndex;

        // Act
        game.GoToMove(5); // Invalid index

        // Assert
        Assert.Equal(originalIndex, game.CurrentMoveIndex);
    }

    [Fact]
    public void GetMovesUpToCurrent_ShouldReturnCorrectMoves()
    {
        // Arrange
        var game = new ChessGame();
        var move1 = new Move(new Position(1, 4), new Position(3, 4));
        var move2 = new Move(new Position(6, 4), new Position(4, 4));
        game.AddMove(move1);
        game.AddMove(move2);
        game.GoToMove(0); // Go to first move

        // Act
        var moves = game.GetMovesUpToCurrent();

        // Assert
        Assert.Single(moves);
        Assert.Equal(move1, moves[0]);
    }

    [Fact]
    public void GetMovesUpToCurrent_AtBeginning_ShouldReturnEmpty()
    {
        // Arrange
        var game = new ChessGame();
        game.AddMove(new Move(new Position(1, 4), new Position(3, 4)));
        game.GoToFirstMove(); // Go to index -1

        // Act
        var moves = game.GetMovesUpToCurrent();

        // Assert
        Assert.Empty(moves);
    }

    [Fact]
    public void GetMoveHistoryText_ShouldNotBeEmpty()
    {
        // Arrange
        var game = new ChessGame();
        game.AddMove(new Move(new Position(1, 4), new Position(3, 4)));
        game.AddMove(new Move(new Position(6, 4), new Position(4, 4)));

        // Act
        var historyText = game.GetMoveHistoryText();

        // Assert
        Assert.NotNull(historyText);
        Assert.NotEmpty(historyText);
    }

    [Fact]
    public void AddMove_WhenNavigatingBack_ShouldRemoveFutureMoves()
    {
        // Arrange
        var game = new ChessGame();
        game.AddMove(new Move(new Position(1, 4), new Position(3, 4))); // Move 1
        game.AddMove(new Move(new Position(6, 4), new Position(4, 4))); // Move 2
        game.AddMove(new Move(new Position(0, 6), new Position(2, 5))); // Move 3
        game.GoToMove(0); // Go back to move 1

        // Act
        game.AddMove(new Move(new Position(0, 1), new Position(2, 2))); // New move 2

        // Assert
        Assert.Equal(2, game.MoveHistory.Count);
        Assert.Equal(1, game.CurrentMoveIndex);
    }

    [Fact]
    public async Task NavigationOperations_ShouldCompleteQuickly()
    {
        // Arrange
        var game = new ChessGame();
        // Add some moves
        for (int i = 0; i < 10; i++)
        {
            game.AddMove(new Move(new Position(1, i % 8), new Position(3, i % 8)));
        }

        // Act
        var startTime = DateTime.Now;
        
        // Perform multiple navigation operations
        for (int i = 0; i < 100; i++)
        {
            game.GoToFirstMove();
            game.GoToLastMove();
            game.GoToPreviousMove();
            game.GoToNextMove();
        }
        
        var endTime = DateTime.Now;
        var duration = endTime - startTime;

        // Assert
        Assert.True(duration.TotalSeconds < 1, $"Navigation operations took too long: {duration.TotalSeconds} seconds");
    }

    [Fact]
    public async Task NavigationWithTimeout_ShouldNotHang()
    {
        // Arrange
        _cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(3)); // 3 second timeout
        var game = new ChessGame();
        
        // Add some moves
        for (int i = 0; i < 5; i++)
        {
            game.AddMove(new Move(new Position(1, i % 8), new Position(3, i % 8)));
        }

        // Act & Assert
        try
        {
            await Task.Run(() =>
            {
                // Perform many navigation operations
                for (int i = 0; i < 1000; i++)
                {
                    game.GoToFirstMove();
                    game.GoToLastMove();
                    game.GoToPreviousMove();
                    game.GoToNextMove();
                }
            }, _cancellationTokenSource.Token);

            // If we get here, the operations completed within timeout
            Assert.True(true);
        }
        catch (OperationCanceledException)
        {
            Assert.True(false, "Navigation operations timed out after 3 seconds");
        }
    }

    [Fact]
    public void BoundaryNavigation_ShouldHandleCorrectly()
    {
        // Arrange
        var game = new ChessGame();
        game.AddMove(new Move(new Position(1, 4), new Position(3, 4)));

        // Act & Assert
        // Go to first move multiple times (should not cause issues)
        game.GoToFirstMove();
        game.GoToFirstMove();
        game.GoToFirstMove();
        Assert.Equal(-1, game.CurrentMoveIndex);

        // Go to last move multiple times (should not cause issues)
        game.GoToLastMove();
        game.GoToLastMove();
        game.GoToLastMove();
        Assert.Equal(0, game.CurrentMoveIndex);

        // Try to go beyond boundaries
        game.GoToPreviousMove();
        Assert.Equal(-1, game.CurrentMoveIndex); // Should stay at first move

        game.GoToNextMove();
        game.GoToNextMove();
        Assert.Equal(0, game.CurrentMoveIndex); // Should stay at last move
    }

    [Fact]
    public void NavigationStateConsistency_ShouldBeMaintained()
    {
        // Arrange
        var game = new ChessGame();
        game.AddMove(new Move(new Position(1, 4), new Position(3, 4)));
        game.AddMove(new Move(new Position(6, 4), new Position(4, 4)));

        // Act & Assert
        // Test that navigation state is consistent
        game.GoToFirstMove();
        Assert.False(game.CanGoBack);
        Assert.True(game.CanGoForward);

        game.GoToLastMove();
        Assert.True(game.CanGoBack);
        Assert.False(game.CanGoForward);

        game.GoToMove(0); // Go to middle
        Assert.True(game.CanGoBack);  // Can go back from index 0
        Assert.True(game.CanGoForward); // Can go forward from index 0
    }
}
