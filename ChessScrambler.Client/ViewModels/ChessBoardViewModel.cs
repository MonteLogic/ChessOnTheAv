using System;
using System.Collections.Generic;
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

    public void ExportDebugState()
    {
        try
        {
            var debugState = _chessBoard.ExportDebugState();
            var fileName = $"chess_debug_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            
            System.IO.File.WriteAllText(filePath, debugState);
            
            GameStatusText = $"Debug state exported to: {fileName}";
        }
        catch (Exception ex)
        {
            GameStatusText = $"Error exporting debug state: {ex.Message}";
        }
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
        Console.WriteLine($"[LOG] UpdateGameStatus called - Current player: {_chessBoard.CurrentPlayer}");
        CurrentPlayerText = $"Current Player: {(_chessBoard.CurrentPlayer == PieceColor.White ? "White" : "Black")}";
        
        var moves = _chessBoard.MoveHistory.Select(m => m.GetNotation()).ToList();
        
        // Format moves in a more readable way with move numbers
        var formattedMoves = new List<string>();
        for (int i = 0; i < moves.Count; i++)
        {
            if (i % 2 == 0)
            {
                // White move - add move number
                var moveNumber = (i / 2) + 1;
                formattedMoves.Add($"{moveNumber}. {moves[i]}");
            }
            else
            {
                // Black move - just add the move
                formattedMoves.Add(moves[i]);
            }
        }
        
        MoveHistoryText = string.Join(" ", formattedMoves);
        
        IsGameOver = _chessBoard.IsGameOver;
        GameStatusText = IsGameOver ? 
            (_chessBoard.Winner.HasValue ? 
                $"Game Over - {(_chessBoard.Winner == PieceColor.White ? "White" : "Black")} Wins!" : 
                "Game Over - Draw!") : 
            "Game in Progress";
    }

    public void OnSquareClicked(SquareViewModel square)
    {
        Console.WriteLine($"[LOG] Square clicked: Row={square.Row}, Col={square.Column}, Piece={square.Piece?.ToString() ?? "Empty"}");
        Console.WriteLine($"[LOG] Current player: {_chessBoard.CurrentPlayer}");
        Console.WriteLine($"[LOG] Game over: {IsGameOver}");
        Console.WriteLine($"[LOG] Selected square: {SelectedSquare?.Row},{SelectedSquare?.Column ?? -1}");
        
        if (IsGameOver) 
        {
            Console.WriteLine("[LOG] Game is over, ignoring click");
            return;
        }

        if (SelectedSquare == null)
        {
            Console.WriteLine("[LOG] No piece selected, trying to select piece");
            // Select a piece
            if (square.Piece != null && square.Piece.Color == _chessBoard.CurrentPlayer)
            {
                Console.WriteLine($"[LOG] Selecting piece: {square.Piece.Type} at {square.Row},{square.Column}");
                SelectedSquare = square;
                HighlightValidMoves(square);
            }
            else
            {
                Console.WriteLine($"[LOG] Cannot select piece: Piece={square.Piece?.ToString() ?? "null"}, CurrentPlayer={_chessBoard.CurrentPlayer}");
            }
        }
        else
        {
            if (square == SelectedSquare)
            {
                Console.WriteLine("[LOG] Deselecting piece");
                // Deselect
                SelectedSquare = null;
                ClearHighlights();
            }
            else if (square.Piece != null && square.Piece.Color == _chessBoard.CurrentPlayer)
            {
                Console.WriteLine($"[LOG] Selecting different piece: {square.Piece.Type} at {square.Row},{square.Column}");
                // Select different piece
                SelectedSquare = square;
                ClearHighlights();
                HighlightValidMoves(square);
            }
            else
            {
                Console.WriteLine($"[LOG] Attempting move from {SelectedSquare.Row},{SelectedSquare.Column} to {square.Row},{square.Column}");
                // Try to make a move
                var move = new Move(new Position(SelectedSquare.Row, SelectedSquare.Column), new Position(square.Row, square.Column));
                Console.WriteLine($"[LOG] Move object created: {move.GetNotation()}");
                
                var isValid = _chessBoard.IsValidMove(move);
                Console.WriteLine($"[LOG] Move is valid: {isValid}");
                
                if (isValid)
                {
                    var moveResult = _chessBoard.MakeMove(move);
                    Console.WriteLine($"[LOG] Move result: {moveResult}");
                    
                    if (moveResult)
                    {
                        Console.WriteLine("[LOG] Move successful, updating board and game status");
                        UpdateBoard();
                        UpdateGameStatus();
                        SelectedSquare = null;
                        ClearHighlights();
                        Console.WriteLine($"[LOG] New current player: {_chessBoard.CurrentPlayer}");
                    }
                    else
                    {
                        Console.WriteLine("[LOG] Move failed - keeping current player and selection");
                        // Don't change the current player or clear selection when move fails
                    }
                }
                else
                {
                    Console.WriteLine("[LOG] Move is not valid - keeping current player and selection");
                    // Don't change the current player or clear selection when move is invalid
                }
            }
        }
    }

    private void HighlightValidMoves(SquareViewModel fromSquare)
    {
        var fromPosition = new Position(fromSquare.Row, fromSquare.Column);
        Console.WriteLine($"[LOG] Getting valid moves for piece at {fromPosition.Row},{fromPosition.Column}");
        var validMoves = _chessBoard.GetValidMoves(fromPosition);
        Console.WriteLine($"[LOG] Found {validMoves.Count} valid moves");
        
        foreach (var square in Squares)
        {
            var move = new Move(fromPosition, new Position(square.Row, square.Column));
            square.IsHighlighted = validMoves.Any(m => m.From.Equals(move.From) && m.To.Equals(move.To));
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