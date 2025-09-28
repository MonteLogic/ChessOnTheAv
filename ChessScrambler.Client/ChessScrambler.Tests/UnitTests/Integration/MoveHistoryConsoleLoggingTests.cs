using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ChessScrambler.Client.Models;
using ChessScrambler.Client.ViewModels;

namespace ChessScrambler.Tests.UnitTests.Integration;

public class MoveHistoryConsoleLoggingTests : IDisposable
{
    private readonly StringWriter _consoleOutput;
    private readonly TextWriter _originalConsoleOut;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public MoveHistoryConsoleLoggingTests()
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
    public async Task LoadNewPosition_ShouldLogGameInformation()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();
        ClearConsoleOutput();

        // Act
        viewModel.LoadNewPosition();
        await Task.Delay(500); // Allow time for async operations

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("[GAME]", output);
        Assert.Contains("moves", output);
    }

    [Fact]
    public async Task NavigationOperations_ShouldLogCorrectly()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();
        viewModel.LoadNewPosition();
        await Task.Delay(300); // Allow initialization
        ClearConsoleOutput();

        // Act
        viewModel.GoToFirstMove();
        viewModel.GoToLastMove();
        viewModel.GoToPreviousMove();
        viewModel.GoToNextMove();

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("[GAME]", output);
    }

    [Fact]
    public async Task MoveHistoryGeneration_ShouldLogMoveDetails()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();
        ClearConsoleOutput();

        // Act
        viewModel.LoadNewPosition();
        await Task.Delay(500); // Allow time for move generation

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("[GAME]", output);
        Assert.Contains("Generated", output);
    }

    [Fact]
    public async Task ConsoleLogging_ShouldNotHang()
    {
        // Arrange
        _cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(5));
        var viewModel = new ChessBoardViewModel();
        ClearConsoleOutput();

        // Act & Assert
        try
        {
            await Task.Run(async () =>
            {
                viewModel.LoadNewPosition();
                await Task.Delay(100);
                
                // Perform multiple operations that should log
                for (int i = 0; i < 10; i++)
                {
                    viewModel.GoToFirstMove();
                    viewModel.GoToLastMove();
                    viewModel.GoToPreviousMove();
                    viewModel.GoToNextMove();
                }
            }, _cancellationTokenSource.Token);

            // If we get here, operations completed within timeout
            var output = GetConsoleOutput();
            Assert.True(output.Length > 0, "Console output should not be empty");
        }
        catch (OperationCanceledException)
        {
            Assert.True(false, "Console logging operations timed out after 5 seconds");
        }
    }

    [Fact]
    public async Task MultipleLoadOperations_ShouldLogConsistently()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();
        ClearConsoleOutput();

        // Act
        for (int i = 0; i < 3; i++)
        {
            viewModel.LoadNewPosition();
            await Task.Delay(200);
        }

        // Assert
        var output = GetConsoleOutput();
        var gameLogCount = CountOccurrences(output, "[GAME]");
        Assert.True(gameLogCount > 0, "Should have logged game information");
    }

    [Fact]
    public async Task NavigationStateChanges_ShouldBeLogged()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();
        viewModel.LoadNewPosition();
        await Task.Delay(300);
        ClearConsoleOutput();

        // Act
        // Perform navigation that should trigger state changes
        viewModel.GoToFirstMove();
        await Task.Delay(50);
        viewModel.GoToLastMove();
        await Task.Delay(50);
        viewModel.GoToPreviousMove();
        await Task.Delay(50);

        // Assert
        var output = GetConsoleOutput();
        Assert.Contains("[GAME]", output);
    }

    [Fact]
    public async Task MoveHistoryText_ShouldBeGenerated()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();
        viewModel.LoadNewPosition();
        await Task.Delay(300);

        // Act
        var moveHistoryText = viewModel.MoveHistoryText;
        var moveNavigationText = viewModel.MoveNavigationText;

        // Assert
        Assert.NotNull(moveHistoryText);
        Assert.NotNull(moveNavigationText);
        Assert.NotEmpty(moveHistoryText);
        Assert.NotEmpty(moveNavigationText);
    }

    [Fact]
    public async Task ConsoleOutput_ShouldContainExpectedPatterns()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();
        ClearConsoleOutput();

        // Act
        viewModel.LoadNewPosition();
        await Task.Delay(500);

        // Assert
        var output = GetConsoleOutput();
        
        // Check for common logging patterns
        Assert.Contains("[GAME]", output);
        
        // Should contain some form of move information
        bool hasMoveInfo = output.Contains("move") || output.Contains("Move") || 
                          output.Contains("generated") || output.Contains("Generated");
        Assert.True(hasMoveInfo, "Console output should contain move-related information");
    }

    [Fact]
    public async Task RapidNavigation_ShouldNotCauseErrors()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();
        viewModel.LoadNewPosition();
        await Task.Delay(300);
        ClearConsoleOutput();

        // Act
        // Perform rapid navigation
        for (int i = 0; i < 20; i++)
        {
            viewModel.GoToFirstMove();
            viewModel.GoToLastMove();
            viewModel.GoToPreviousMove();
            viewModel.GoToNextMove();
        }

        // Assert
        var output = GetConsoleOutput();
        Assert.DoesNotContain("Error", output);
        Assert.DoesNotContain("Exception", output);
        Assert.DoesNotContain("Failed", output);
    }

    [Fact]
    public async Task ConsoleLoggingPerformance_ShouldBeAcceptable()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();
        ClearConsoleOutput();

        // Act
        var startTime = DateTime.Now;
        
        viewModel.LoadNewPosition();
        await Task.Delay(200);
        
        // Perform multiple operations
        for (int i = 0; i < 50; i++)
        {
            viewModel.GoToFirstMove();
            viewModel.GoToLastMove();
            viewModel.GoToPreviousMove();
            viewModel.GoToNextMove();
        }
        
        var endTime = DateTime.Now;
        var duration = endTime - startTime;

        // Assert
        Assert.True(duration.TotalSeconds < 6, $"Console logging operations took too long: {duration.TotalSeconds} seconds");
        
        var output = GetConsoleOutput();
        Assert.True(output.Length > 0, "Should have generated console output");
    }

    private int CountOccurrences(string text, string pattern)
    {
        int count = 0;
        int index = 0;
        while ((index = text.IndexOf(pattern, index)) != -1)
        {
            count++;
            index += pattern.Length;
        }
        return count;
    }
}
