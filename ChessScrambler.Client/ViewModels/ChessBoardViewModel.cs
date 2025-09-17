using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ChessScrambler.Client.Models;

namespace ChessScrambler.Client.ViewModels;

public class SquareViewModel : INotifyPropertyChanged
{
    private ChessPiece _piece;
    private bool _isSelected;
    private bool _isHighlighted;
    private bool _isLightSquare;

    public ChessPiece Piece
    {
        get => _piece;
        set => SetProperty(ref _piece, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public bool IsHighlighted
    {
        get => _isHighlighted;
        set => SetProperty(ref _isHighlighted, value);
    }

    public bool IsLightSquare
    {
        get => _isLightSquare;
        set => SetProperty(ref _isLightSquare, value);
    }

    public int Row { get; set; }
    public int Column { get; set; }
    public string Position => $"{(char)('a' + Column)}{8 - Row}";

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public class ChessBoardViewModel : INotifyPropertyChanged
{
    private ChessBoard _chessBoard;
    private SquareViewModel _selectedSquare;
    private string _currentPlayerText;
    private string _moveHistoryText;
    private bool _isGameOver;
    private string _gameStatusText;

    public ObservableCollection<SquareViewModel> Squares { get; } = new ObservableCollection<SquareViewModel>();

    public SquareViewModel SelectedSquare
    {
        get => _selectedSquare;
        set
        {
            if (_selectedSquare != null)
                _selectedSquare.IsSelected = false;
            
            _selectedSquare = value;
            if (_selectedSquare != null)
                _selectedSquare.IsSelected = true;
            
            OnPropertyChanged();
        }
    }

    public string CurrentPlayerText
    {
        get => _currentPlayerText;
        set => SetProperty(ref _currentPlayerText, value);
    }

    public string MoveHistoryText
    {
        get => _moveHistoryText;
        set => SetProperty(ref _moveHistoryText, value);
    }

    public bool IsGameOver
    {
        get => _isGameOver;
        set => SetProperty(ref _isGameOver, value);
    }

    public string GameStatusText
    {
        get => _gameStatusText;
        set => SetProperty(ref _gameStatusText, value);
    }

    public ChessBoardViewModel()
    {
        InitializeBoard();
        LoadMiddlegamePosition();
    }

    private void InitializeBoard()
    {
        Squares.Clear();
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var square = new SquareViewModel
                {
                    Row = row,
                    Column = col,
                    IsLightSquare = (row + col) % 2 == 0
                };
                Squares.Add(square);
            }
        }
    }

    public void LoadMiddlegamePosition()
    {
        // Load a typical middlegame position
        var position = MiddlegamePositionDatabase.GetPositionById("pos001");
        _chessBoard = new ChessBoard(position.Fen);
        UpdateBoard();
        UpdateGameStatus();
    }

    public void LoadNewPosition()
    {
        // Load a random middlegame position
        var position = MiddlegamePositionDatabase.GetRandomPosition();
        _chessBoard = new ChessBoard(position.Fen);
        UpdateBoard();
        UpdateGameStatus();
    }

    private void UpdateBoard()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var square = Squares.FirstOrDefault(s => s.Row == row && s.Column == col);
                if (square != null)
                {
                    square.Piece = _chessBoard.GetPiece(row, col);
                    square.IsHighlighted = false;
                }
            }
        }
    }

    private void UpdateGameStatus()
    {
        CurrentPlayerText = $"Current Player: {(_chessBoard.CurrentPlayer == PieceColor.White ? "White" : "Black")}";
        
        var moves = _chessBoard.MoveHistory.Select(m => m.GetNotation()).ToList();
        MoveHistoryText = $"Moves: {string.Join(", ", moves)}";
        
        IsGameOver = _chessBoard.IsGameOver;
        GameStatusText = IsGameOver ? $"Game Over - {(_chessBoard.Winner == PieceColor.White ? "White" : "Black")} Wins!" : "Game in Progress";
    }

    public void OnSquareClicked(SquareViewModel square)
    {
        if (IsGameOver) return;

        if (SelectedSquare == null)
        {
            // Select a piece
            if (square.Piece != null && square.Piece.Color == _chessBoard.CurrentPlayer)
            {
                SelectedSquare = square;
                HighlightValidMoves(square);
            }
        }
        else
        {
            if (square == SelectedSquare)
            {
                // Deselect
                SelectedSquare = null;
                ClearHighlights();
            }
            else if (square.Piece != null && square.Piece.Color == _chessBoard.CurrentPlayer)
            {
                // Select different piece
                SelectedSquare = square;
                ClearHighlights();
                HighlightValidMoves(square);
            }
            else
            {
                // Try to make a move
                var move = new Move(new Position(SelectedSquare.Row, SelectedSquare.Column), new Position(square.Row, square.Column));
                if (_chessBoard.MakeMove(move))
                {
                    UpdateBoard();
                    UpdateGameStatus();
                    SelectedSquare = null;
                    ClearHighlights();
                }
            }
        }
    }

    private void HighlightValidMoves(SquareViewModel fromSquare)
    {
        // This is a simplified version - in a real implementation, you'd calculate all valid moves
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var square = Squares.FirstOrDefault(s => s.Row == row && s.Column == col);
                if (square != null)
                {
                var move = new Move(new Position(fromSquare.Row, fromSquare.Column), new Position(row, col));
                square.IsHighlighted = _chessBoard.IsValidMove(move);
                }
            }
        }
    }

    private void ClearHighlights()
    {
        foreach (var square in Squares)
        {
            square.IsHighlighted = false;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
