using ChessScrambler.Client.Models;
using ChessScrambler.Client.ViewModels;
using Xunit;
using FluentAssertions;

namespace ChessScrambler.Tests.IntegrationTests;

public class GameFlowTests
{
    [Fact]
    public void GameFlow_CompleteGame_ShouldWorkEndToEnd()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();
        var initialPlayer = viewModel.CurrentPlayerText;

        // Act & Assert - Test initial state
        viewModel.Squares.Should().HaveCount(64);
        viewModel.IsGameOver.Should().BeFalse();
        viewModel.GameIdText.Should().NotBeNullOrEmpty();

        // Test that we can navigate moves (even if no moves exist yet)
        viewModel.GoToFirstMove();
        viewModel.CanGoBack.Should().BeFalse();
        viewModel.CanGoForward.Should().BeTrue();

        viewModel.GoToLastMove();
        viewModel.CanGoBack.Should().BeTrue();
        viewModel.CanGoForward.Should().BeFalse();
    }

    [Fact]
    public void GameFlow_NewPosition_ShouldLoadCorrectly()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();
        var originalGameId = viewModel.GameIdText;

        // Act
        viewModel.LoadMiddlegamePosition();

        // Assert
        viewModel.GameIdText.Should().NotBeNullOrEmpty();
        viewModel.Squares.Should().HaveCount(64);
        viewModel.IsGameOver.Should().BeFalse();
    }

    [Fact]
    public void GameFlow_ExportDebugState_ShouldContainValidInformation()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();

        // Act
        var debugState = viewModel.ExportDebugState();

        // Assert
        debugState.Should().NotBeNullOrEmpty();
        debugState.Should().Contain("CHESS BOARD DEBUG STATE");
        debugState.Should().Contain("Current Player");
        debugState.Should().Contain("FEN STRING");
        debugState.Should().Contain("BOARD STATE");
    }

    [Fact]
    public void GameFlow_ExportCurrentGamePgn_ShouldContainValidPgn()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();

        // Act
        var pgn = viewModel.GetCurrentGamePgn();

        // Assert
        pgn.Should().NotBeNullOrEmpty();
        pgn.Should().Contain("[Event");
        pgn.Should().Contain("[Site");
        pgn.Should().Contain("[Date");
        pgn.Should().Contain("[White");
        pgn.Should().Contain("[Black");
        pgn.Should().Contain("[Result");
    }

    [Fact]
    public void GameFlow_SettingsIntegration_ShouldWorkCorrectly()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();
        var originalBoardSize = viewModel.AppSettings.BoardSize;

        // Act
        viewModel.AppSettings.BoardSize = 640;

        // Assert
        viewModel.AppSettings.BoardSize.Should().Be(640);
        viewModel.AppSettings.SquareSize.Should().Be(80); // 640 / 8
        viewModel.AppSettings.PieceSize.Should().Be(69); // 80 * 0.87
    }

    [Fact]
    public void GameFlow_MoveHistoryIntegration_ShouldWorkCorrectly()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();

        // Act
        viewModel.GoToFirstMove();
        var canGoBackAfterFirst = viewModel.CanGoBack;
        var canGoForwardAfterFirst = viewModel.CanGoForward;

        viewModel.GoToLastMove();
        var canGoBackAfterLast = viewModel.CanGoBack;
        var canGoForwardAfterLast = viewModel.CanGoForward;

        // Assert
        canGoBackAfterFirst.Should().BeFalse();
        canGoForwardAfterFirst.Should().BeTrue();
        canGoBackAfterLast.Should().BeTrue();
        canGoForwardAfterLast.Should().BeFalse();
    }

    [Fact]
    public void GameFlow_GameStateIntegration_ShouldBeConsistent()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();

        // Act
        var gameId = viewModel.GameIdText;
        var currentPlayer = viewModel.CurrentPlayerText;
        var gameStatus = viewModel.GameStatusText;
        var isGameOver = viewModel.IsGameOver;

        // Assert
        gameId.Should().NotBeNullOrEmpty();
        currentPlayer.Should().NotBeNullOrEmpty();
        gameStatus.Should().NotBeNullOrEmpty();
        isGameOver.Should().BeFalse();
    }

    [Fact]
    public void GameFlow_AppSettingsIntegration_ShouldLoadCorrectly()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();

        // Act
        var settings = viewModel.AppSettings;

        // Assert
        settings.Should().NotBeNull();
        settings.BoardSize.Should().BeGreaterThan(0);
        settings.WindowWidth.Should().BeGreaterThan(0);
        settings.WindowHeight.Should().BeGreaterThan(0);
        settings.WindowSizeMode.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GameFlow_VisualFeedbackIntegration_ShouldWorkCorrectly()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();
        var square = viewModel.Squares.First();

        // Act
        viewModel.SelectedSquare = square;

        // Assert
        viewModel.SelectedSquare.Should().Be(square);
        square.IsSelected.Should().BeTrue();
    }

    [Fact]
    public void GameFlow_ErrorHandling_ShouldBeGraceful()
    {
        // Arrange
        var viewModel = new ChessBoardViewModel();

        // Act & Assert - Test that operations don't throw exceptions
        var action1 = () => viewModel.GoToFirstMove();
        action1.Should().NotThrow();

        var action2 = () => viewModel.GoToLastMove();
        action2.Should().NotThrow();

        var action3 = () => viewModel.GoToPreviousMove();
        action3.Should().NotThrow();

        var action4 = () => viewModel.GoToNextMove();
        action4.Should().NotThrow();

        var action5 = () => viewModel.ExportDebugState();
        action5.Should().NotThrow();

        var action6 = () => viewModel.GetCurrentGamePgn();
        action6.Should().NotThrow();
    }
}
