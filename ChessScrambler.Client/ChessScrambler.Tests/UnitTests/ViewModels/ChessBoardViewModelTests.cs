using ChessScrambler.Client.ViewModels;
using ChessScrambler.Client.Models;
using Xunit;
using FluentAssertions;
using System.ComponentModel;

namespace ChessScrambler.Tests.UnitTests.ViewModels;

public class ChessBoardViewModelTests
{
    private readonly ChessBoardViewModel _viewModel;

    public ChessBoardViewModelTests()
    {
        _viewModel = new ChessBoardViewModel();
    }

    [Fact]
    public void ChessBoardViewModel_Constructor_ShouldInitializeCorrectly()
    {
        // Assert
        _viewModel.Squares.Should().HaveCount(64);
        _viewModel.CurrentPlayerText.Should().NotBeNullOrEmpty();
        _viewModel.MoveHistoryText.Should().NotBeNullOrEmpty();
        _viewModel.IsGameOver.Should().BeFalse();
        _viewModel.GameStatusText.Should().NotBeNullOrEmpty();
        _viewModel.GameIdText.Should().NotBeNullOrEmpty();
        _viewModel.AppSettings.Should().NotBeNull();
    }

    [Fact]
    public void Squares_ShouldHaveCorrectPositions()
    {
        // Act & Assert
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var square = _viewModel.Squares.FirstOrDefault(s => s.Row == row && s.Column == col);
                square.Should().NotBeNull();
                square!.Row.Should().Be(row);
                square.Column.Should().Be(col);
                square.IsLightSquare.Should().Be((row + col) % 2 == 0);
            }
        }
    }

    [Fact]
    public void SelectedSquare_WhenSet_ShouldUpdatePreviousSquare()
    {
        // Arrange
        var square1 = _viewModel.Squares[0];
        var square2 = _viewModel.Squares[1];

        // Act
        _viewModel.SelectedSquare = square1;
        _viewModel.SelectedSquare = square2;

        // Assert
        square1.IsSelected.Should().BeFalse();
        square2.IsSelected.Should().BeTrue();
        _viewModel.SelectedSquare.Should().Be(square2);
    }

    [Fact]
    public void SelectedSquare_WhenSetToNull_ShouldDeselectCurrentSquare()
    {
        // Arrange
        var square = _viewModel.Squares[0];
        _viewModel.SelectedSquare = square;

        // Act
        _viewModel.SelectedSquare = null;

        // Assert
        square.IsSelected.Should().BeFalse();
        _viewModel.SelectedSquare.Should().BeNull();
    }

    [Fact]
    public void OnSquareClicked_WithValidPiece_ShouldSelectPiece()
    {
        // Arrange
        var square = _viewModel.Squares.First(s => s.Piece != null && s.Piece.Color == PieceColor.White);

        // Act
        _viewModel.OnSquareClicked(square);

        // Assert
        _viewModel.SelectedSquare.Should().Be(square);
        square.IsSelected.Should().BeTrue();
    }

    [Fact]
    public void OnSquareClicked_WithInvalidPiece_ShouldNotSelectPiece()
    {
        // Arrange
        var square = _viewModel.Squares.First(s => s.Piece != null && s.Piece.Color == PieceColor.Black);

        // Act
        _viewModel.OnSquareClicked(square);

        // Assert
        _viewModel.SelectedSquare.Should().BeNull();
        square.IsSelected.Should().BeFalse();
    }

    [Fact]
    public void OnSquareClicked_WhenGameOver_ShouldNotSelectPiece()
    {
        // Arrange
        _viewModel.IsGameOver = true;
        var square = _viewModel.Squares.First(s => s.Piece != null && s.Piece.Color == PieceColor.White);

        // Act
        _viewModel.OnSquareClicked(square);

        // Assert
        _viewModel.SelectedSquare.Should().BeNull();
        square.IsSelected.Should().BeFalse();
    }

    [Fact]
    public void OnSquareClicked_WithSelectedPiece_ShouldMakeMove()
    {
        // Arrange
        var fromSquare = _viewModel.Squares.First(s => s.Piece != null && s.Piece.Color == PieceColor.White);
        var toSquare = _viewModel.Squares.First(s => s.Piece == null);
        _viewModel.SelectedSquare = fromSquare;

        // Act
        _viewModel.OnSquareClicked(toSquare);

        // Assert
        // Note: This test would need to be more specific based on the actual move validation logic
        // The exact assertion would depend on whether the move is valid or not
    }

    [Fact]
    public void PropertyChanged_WhenPropertyChanges_ShouldRaiseEvent()
    {
        // Arrange
        var eventRaised = false;
        _viewModel.PropertyChanged += (sender, args) => eventRaised = true;

        // Act
        _viewModel.CurrentPlayerText = "New Player";

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void AppSettings_ShouldBeLoaded()
    {
        // Assert
        _viewModel.AppSettings.Should().NotBeNull();
        _viewModel.AppSettings.BoardSize.Should().BeGreaterThan(0);
        _viewModel.AppSettings.WindowWidth.Should().BeGreaterThan(0);
        _viewModel.AppSettings.WindowHeight.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GoToFirstMove_ShouldNavigateToFirstMove()
    {
        // Act
        _viewModel.GoToFirstMove();

        // Assert
        _viewModel.CanGoBack.Should().BeFalse();
        _viewModel.CanGoForward.Should().BeTrue();
    }

    [Fact]
    public void GoToLastMove_ShouldNavigateToLastMove()
    {
        // Act
        _viewModel.GoToLastMove();

        // Assert
        _viewModel.CanGoBack.Should().BeTrue();
        _viewModel.CanGoForward.Should().BeFalse();
    }

    [Fact]
    public void GoToPreviousMove_ShouldNavigateBackward()
    {
        // Arrange
        _viewModel.GoToLastMove();
        var originalCanGoForward = _viewModel.CanGoForward;

        // Act
        _viewModel.GoToPreviousMove();

        // Assert
        _viewModel.CanGoBack.Should().BeTrue();
        _viewModel.CanGoForward.Should().BeTrue();
    }

    [Fact]
    public void GoToNextMove_ShouldNavigateForward()
    {
        // Arrange
        _viewModel.GoToFirstMove();
        var originalCanGoBack = _viewModel.CanGoBack;

        // Act
        _viewModel.GoToNextMove();

        // Assert
        _viewModel.CanGoBack.Should().BeTrue();
        _viewModel.CanGoForward.Should().BeTrue();
    }

    [Fact]
    public void ExportDebugState_ShouldReturnDebugInformation()
    {
        // Act
        var debugState = _viewModel.ExportDebugState();

        // Assert
        debugState.Should().NotBeNullOrEmpty();
        debugState.Should().Contain("CHESS BOARD DEBUG STATE");
        debugState.Should().Contain("Current Player");
        debugState.Should().Contain("FEN STRING");
    }

    [Fact]
    public void GetCurrentGamePgn_ShouldReturnPgnString()
    {
        // Act
        var pgn = _viewModel.GetCurrentGamePgn();

        // Assert
        pgn.Should().NotBeNullOrEmpty();
        pgn.Should().Contain("[Event");
        pgn.Should().Contain("[Site");
        pgn.Should().Contain("[Date");
        pgn.Should().Contain("[White");
        pgn.Should().Contain("[Black");
    }
}
