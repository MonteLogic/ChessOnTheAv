using ChessScrambler.Client.Models;
using Xunit;
using FluentAssertions;

namespace ChessScrambler.Tests.UnitTests.Models;

public class ChessBoardTests
{
    private readonly ChessBoard _chessBoard;

    public ChessBoardTests()
    {
        _chessBoard = new ChessBoard();
    }

    [Fact]
    public void ChessBoard_InitialPosition_ShouldHaveCorrectPieces()
    {
        // Arrange & Act
        var whiteKing = _chessBoard.GetPiece(7, 4); // e1
        var blackKing = _chessBoard.GetPiece(0, 4); // e8
        var whitePawn = _chessBoard.GetPiece(6, 4); // e2
        var blackPawn = _chessBoard.GetPiece(1, 4); // e7

        // Assert
        whiteKing.Should().NotBeNull();
        whiteKing!.Type.Should().Be(PieceType.King);
        whiteKing.Color.Should().Be(PieceColor.White);

        blackKing.Should().NotBeNull();
        blackKing!.Type.Should().Be(PieceType.King);
        blackKing.Color.Should().Be(PieceColor.Black);

        whitePawn.Should().NotBeNull();
        whitePawn!.Type.Should().Be(PieceType.Pawn);
        whitePawn.Color.Should().Be(PieceColor.White);

        blackPawn.Should().NotBeNull();
        blackPawn!.Type.Should().Be(PieceType.Pawn);
        blackPawn.Color.Should().Be(PieceColor.Black);
    }

    [Fact]
    public void ChessBoard_InitialPosition_ShouldHaveWhiteToMove()
    {
        // Act
        var currentPlayer = _chessBoard.CurrentPlayer;

        // Assert
        currentPlayer.Should().Be(PieceColor.White);
    }

    [Fact]
    public void ChessBoard_InitialPosition_ShouldNotBeGameOver()
    {
        // Act
        var isGameOver = _chessBoard.IsGameOver;

        // Assert
        isGameOver.Should().BeFalse();
    }

    [Fact]
    public void MakeMove_ValidPawnMove_ShouldReturnTrue()
    {
        // Arrange
        var move = new Move(new Position(6, 4), new Position(4, 4)); // e2-e4

        // Act
        var result = _chessBoard.MakeMove(move);

        // Assert
        result.Should().BeTrue();
        _chessBoard.CurrentPlayer.Should().Be(PieceColor.Black);
    }

    [Fact]
    public void MakeMove_InvalidMove_ShouldReturnFalse()
    {
        // Arrange
        var move = new Move(new Position(7, 4), new Position(5, 4)); // e1-e3 (invalid)

        // Act
        var result = _chessBoard.MakeMove(move);

        // Assert
        result.Should().BeFalse();
        _chessBoard.CurrentPlayer.Should().Be(PieceColor.White); // Should not change
    }

    [Fact]
    public void IsValidMove_ValidMove_ShouldReturnTrue()
    {
        // Arrange
        var move = new Move(new Position(6, 4), new Position(4, 4)); // e2-e4

        // Act
        var isValid = _chessBoard.IsValidMove(move);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValidMove_InvalidMove_ShouldReturnFalse()
    {
        // Arrange
        var move = new Move(new Position(7, 4), new Position(5, 4)); // e1-e3

        // Act
        var isValid = _chessBoard.IsValidMove(move);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void GetValidMoves_FromPawnPosition_ShouldReturnValidMoves()
    {
        // Arrange
        var fromPosition = new Position(6, 4); // e2

        // Act
        var validMoves = _chessBoard.GetValidMoves(fromPosition);

        // Assert
        validMoves.Should().NotBeEmpty();
        validMoves.Should().Contain(m => m.To.Row == 4 && m.To.Column == 4); // e4
        validMoves.Should().Contain(m => m.To.Row == 5 && m.To.Column == 4); // e3
    }

    [Fact]
    public void GetFen_InitialPosition_ShouldReturnCorrectFen()
    {
        // Act
        var fen = _chessBoard.GetFen();

        // Assert
        fen.Should().Be("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
    }

    [Fact]
    public void ChessBoard_WithFen_ShouldLoadCorrectPosition()
    {
        // Arrange
        var customFen = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";
        var board = new ChessBoard(customFen);

        // Act
        var currentPlayer = board.CurrentPlayer;
        var fen = board.GetFen();

        // Assert
        currentPlayer.Should().Be(PieceColor.Black);
        fen.Should().Be(customFen);
    }
}
