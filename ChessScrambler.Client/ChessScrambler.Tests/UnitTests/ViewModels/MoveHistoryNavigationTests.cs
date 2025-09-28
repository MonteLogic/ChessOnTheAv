using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ChessScrambler.Client.Models;
using ChessScrambler.Client.ViewModels;

namespace ChessScrambler.Tests.UnitTests.ViewModels;

public class MoveHistoryNavigationTests : IDisposable
{
    private readonly StringWriter _consoleOutput;
    private readonly TextWriter _originalConsoleOut;
    private readonly ChessBoardViewModel _viewModel;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public MoveHistoryNavigationTests()
    {
        // Capture console output
        _consoleOutput = new StringWriter();
        _originalConsoleOut = Console.Out;
        Console.SetOut(_consoleOutput);

        // Create view model
        _viewModel = new ChessBoardViewModel();
        
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
    public async Task GoToFirstMove_ShouldLogCorrectly()
    {
        // Arrange
        _viewModel.LoadNewPosition(); // Load a position with moves
        await Task.Delay(100); // Allow initialization
        ClearConsoleOutput();

        // Act
        _viewModel.GoToFirstMove();

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("[GAME]", output);
    }

    [Fact]
    public async Task GoToLastMove_ShouldLogCorrectly()
    {
        // Arrange
        _viewModel.LoadNewPosition(); // Load a position with moves
        await Task.Delay(100); // Allow initialization
        ClearConsoleOutput();

        // Act
        _viewModel.GoToLastMove();

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("[GAME]", output);
    }

    [Fact]
    public async Task GoToPreviousMove_ShouldLogCorrectly()
    {
        // Arrange
        _viewModel.LoadNewPosition(); // Load a position with moves
        await Task.Delay(100); // Allow initialization
        ClearConsoleOutput();

        // Act
        _viewModel.GoToPreviousMove();

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("[GAME]", output);
    }

    [Fact]
    public async Task GoToNextMove_ShouldLogCorrectly()
    {
        // Arrange
        _viewModel.LoadNewPosition(); // Load a position with moves
        await Task.Delay(100); // Allow initialization
        ClearConsoleOutput();

        // Act
        _viewModel.GoToNextMove();

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("[GAME]", output);
    }

    [Fact]
    public async Task NavigationState_ShouldUpdateCorrectly()
    {
        // Arrange
        _viewModel.LoadNewPosition(); // Load a position with moves
        await Task.Delay(100); // Allow initialization

        // Act & Assert
        // Test initial state
        Assert.True(_viewModel.CanGoBack || _viewModel.CanGoForward, "Should be able to navigate in at least one direction");

        // Test after going to first move
        _viewModel.GoToFirstMove();
        Assert.False(_viewModel.CanGoBack, "Should not be able to go back from first move");

        // Test after going to last move
        _viewModel.GoToLastMove();
        Assert.False(_viewModel.CanGoForward, "Should not be able to go forward from last move");
    }

    [Fact]
    public async Task MoveHistoryText_ShouldNotBeEmpty()
    {
        // Arrange
        _viewModel.LoadNewPosition(); // Load a position with moves
        await Task.Delay(100); // Allow initialization

        // Act
        var moveHistoryText = _viewModel.MoveHistoryText;

        // Assert
        Assert.NotNull(moveHistoryText);
        Assert.NotEmpty(moveHistoryText);
    }

    [Fact]
    public async Task MoveNavigationText_ShouldNotBeEmpty()
    {
        // Arrange
        _viewModel.LoadNewPosition(); // Load a position with moves
        await Task.Delay(100); // Allow initialization

        // Act
        var moveNavigationText = _viewModel.MoveNavigationText;

        // Assert
        Assert.NotNull(moveNavigationText);
        Assert.NotEmpty(moveNavigationText);
    }

    [Fact]
    public async Task NavigationWithTimeout_ShouldNotHang()
    {
        // Arrange
        _cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(5)); // 5 second timeout
        _viewModel.LoadNewPosition(); // Load a position with moves
        await Task.Delay(100); // Allow initialization

        // Act & Assert
        try
        {
            await Task.Run(() =>
            {
                _viewModel.GoToFirstMove();
                _viewModel.GoToLastMove();
                _viewModel.GoToPreviousMove();
                _viewModel.GoToNextMove();
            }, _cancellationTokenSource.Token);

            // If we get here, the operations completed within timeout
            Assert.True(true);
        }
        catch (OperationCanceledException)
        {
            Assert.True(false, "Navigation operations timed out after 5 seconds");
        }
    }

    [Fact]
    public async Task MultipleNavigationOperations_ShouldCompleteQuickly()
    {
        // Arrange
        _viewModel.LoadNewPosition(); // Load a position with moves
        await Task.Delay(100); // Allow initialization

        // Act
        var startTime = DateTime.Now;
        
        // Perform multiple navigation operations
        for (int i = 0; i < 10; i++)
        {
            _viewModel.GoToFirstMove();
            _viewModel.GoToLastMove();
            _viewModel.GoToPreviousMove();
            _viewModel.GoToNextMove();
        }
        
        var endTime = DateTime.Now;
        var duration = endTime - startTime;

        // Assert
        Assert.True(duration.TotalSeconds < 2, $"Navigation operations took too long: {duration.TotalSeconds} seconds");
    }

    [Fact]
    public async Task ConsoleOutput_ShouldContainGameLogging()
    {
        // Arrange
        ClearConsoleOutput();

        // Act
        _viewModel.LoadNewPosition(); // This should generate console output
        await Task.Delay(200); // Allow time for async operations

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("[GAME]", output);
    }

    [Fact]
    public async Task MoveHistoryGeneration_ShouldLogMoves()
    {
        // Arrange
        ClearConsoleOutput();

        // Act
        _viewModel.LoadNewPosition(); // This should generate move history
        await Task.Delay(200); // Allow time for async operations

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("Generated", output);
        Assert.Contains("moves", output);
    }

    [Fact]
    public async Task BoundaryNavigation_ShouldHandleCorrectly()
    {
        // Arrange
        _viewModel.LoadNewPosition(); // Load a position with moves
        await Task.Delay(100); // Allow initialization
        ClearConsoleOutput();

        // Act
        // Go to first move multiple times (should not cause issues)
        _viewModel.GoToFirstMove();
        _viewModel.GoToFirstMove();
        _viewModel.GoToFirstMove();

        // Go to last move multiple times (should not cause issues)
        _viewModel.GoToLastMove();
        _viewModel.GoToLastMove();
        _viewModel.GoToLastMove();

        // Assert
        var output = GetConsoleOutput();
        // Should not contain error messages
        Assert.DoesNotContain("Error", output);
        Assert.DoesNotContain("Exception", output);
    }

    [Fact]
    public async Task NavigationStateConsistency_ShouldBeMaintained()
    {
        // Arrange
        _viewModel.LoadNewPosition(); // Load a position with moves
        await Task.Delay(100); // Allow initialization

        // Act & Assert
        // Test that navigation state is consistent
        var initialCanGoBack = _viewModel.CanGoBack;
        var initialCanGoForward = _viewModel.CanGoForward;

        // After going to first move
        _viewModel.GoToFirstMove();
        Assert.False(_viewModel.CanGoBack, "Should not be able to go back from first move");

        // After going to last move
        _viewModel.GoToLastMove();
        Assert.False(_viewModel.CanGoForward, "Should not be able to go forward from last move");

        // After going back to middle
        _viewModel.GoToPreviousMove();
        Assert.True(_viewModel.CanGoBack || _viewModel.CanGoForward, "Should be able to navigate in at least one direction from middle position");
    }
}
