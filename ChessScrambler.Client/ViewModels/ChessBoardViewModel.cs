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
    private bool _showGameEndPopup;
    private string _gameEndMessage;
    private string _gameIdText;
    private bool _canGoBack;
    private bool _canGoForward;
    private string _moveNavigationText;
    private string _gamesBankStatus;

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

    public bool ShowGameEndPopup
    {
        get => _showGameEndPopup;
        set => SetProperty(ref _showGameEndPopup, value);
    }

    public string GameEndMessage
    {
        get => _gameEndMessage;
        set => SetProperty(ref _gameEndMessage, value);
    }

    public string GameIdText
    {
        get => _gameIdText;
        set => SetProperty(ref _gameIdText, value);
    }

    public bool CanGoBack
    {
        get => _canGoBack;
        set => SetProperty(ref _canGoBack, value);
    }

    public bool CanGoForward
    {
        get => _canGoForward;
        set => SetProperty(ref _canGoForward, value);
    }

    public string MoveNavigationText
    {
        get => _moveNavigationText;
        set => SetProperty(ref _moveNavigationText, value);
    }

    public string GamesBankStatus
    {
        get => _gamesBankStatus;
        set => SetProperty(ref _gamesBankStatus, value);
    }

    public ChessBoardViewModel()
    {
        InitializeBoard();
        LoadMiddlegamePosition();
        UpdateGamesBankStatus();
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
        try
        {
            // Try to load from imported games first
            if (GameBank.ImportedGames.Count > 0)
            {
                var importedGame = GameBank.GetRandomMiddlegamePosition();
                var fen = importedGame.GetMiddlegamePositionFen();
                _chessBoard = new ChessBoard(fen);
                
                // Update game info to show it's from an imported game
                GameIdText = $"Game: {importedGame.GetDisplayName()}";
            }
            else
            {
                // Fallback to predefined positions if no games imported
                var position = MiddlegamePositionDatabase.GetRandomPosition();
                _chessBoard = new ChessBoard(position.Fen);
                GameIdText = $"Position: {position.Name}";
            }
            
            UpdateBoard();
            UpdateGameStatus();
        }
        catch (Exception ex)
        {
            // Fallback to predefined positions on error
            var position = MiddlegamePositionDatabase.GetRandomPosition();
            _chessBoard = new ChessBoard(position.Fen);
            GameIdText = $"Position: {position.Name}";
            UpdateBoard();
            UpdateGameStatus();
        }
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

    public void CloseGameEndPopup()
    {
        ShowGameEndPopup = false;
    }

    public void ImportGamesFromFile(string filePath)
    {
        try
        {
            GameBank.ImportGamesFromFile(filePath);
            GameStatusText = $"Imported {GameBank.ImportedGames.Count} games from file";
            UpdateGamesBankStatus();
        }
        catch (Exception ex)
        {
            GameStatusText = $"Error importing games: {ex.Message}";
        }
    }

    public void ImportGamesFromPgn(string pgnContent)
    {
        try
        {
            GameBank.ImportGamesFromPgn(pgnContent);
            GameStatusText = $"Imported {GameBank.ImportedGames.Count} games from PGN";
            UpdateGamesBankStatus();
        }
        catch (Exception ex)
        {
            GameStatusText = $"Error importing games: {ex.Message}";
        }
    }

    public int GetImportedGamesCount()
    {
        return GameBank.ImportedGames.Count;
    }

    public void ClearImportedGames()
    {
        GameBank.ClearGames();
        GameStatusText = "Imported games cleared";
        UpdateGamesBankStatus();
    }

    private void UpdateGamesBankStatus()
    {
        var count = GetImportedGamesCount();
        GamesBankStatus = count > 0 ? $"Games loaded: {count}" : "No games imported";
    }

    public void GoToFirstMove()
    {
        if (_chessBoard != null)
        {
            _chessBoard.GoToFirstMove();
            UpdateBoard();
            UpdateGameStatus();
            UpdateNavigationState();
        }
    }

    public void GoToLastMove()
    {
        if (_chessBoard != null)
        {
            _chessBoard.GoToLastMove();
            UpdateBoard();
            UpdateGameStatus();
            UpdateNavigationState();
        }
    }

    public void GoToPreviousMove()
    {
        if (_chessBoard != null)
        {
            _chessBoard.GoToPreviousMove();
            UpdateBoard();
            UpdateGameStatus();
            UpdateNavigationState();
        }
    }

    public void GoToNextMove()
    {
        if (_chessBoard != null)
        {
            _chessBoard.GoToNextMove();
            UpdateBoard();
            UpdateGameStatus();
            UpdateNavigationState();
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
        if (Program.EnableGameLogging)
        {
            Console.WriteLine($"[GAME] UpdateGameStatus called - Current player: {_chessBoard.CurrentPlayer}");
        }
        
        // Update game ID
        GameIdText = $"Game ID: {_chessBoard.Game.Id}";
        
        // Check if current player is in check
        var isInCheck = _chessBoard.IsInCheck(_chessBoard.CurrentPlayer);
        var checkText = isInCheck ? " (CHECK!)" : "";
        CurrentPlayerText = $"Current Player: {(_chessBoard.CurrentPlayer == PieceColor.White ? "White" : "Black")}{checkText}";
        
        // Use the game's move history text which shows moves up to current position
        MoveHistoryText = _chessBoard.Game.GetMoveHistoryText();
        
        var wasGameOver = IsGameOver;
        IsGameOver = _chessBoard.IsGameOver;
        
        if (IsGameOver)
        {
            if (_chessBoard.Winner.HasValue)
            {
                var winner = _chessBoard.Winner == PieceColor.White ? "White" : "Black";
                GameStatusText = $"Game Over - {winner} Wins by Checkmate!";
                GameEndMessage = $"🎉 {winner} Wins by Checkmate! 🎉";
            }
            else
            {
                // Check if it's stalemate
                var isStalemate = _chessBoard.IsStalemate(_chessBoard.CurrentPlayer);
                GameStatusText = isStalemate ? "Game Over - Stalemate (Draw)!" : "Game Over - Draw!";
                GameEndMessage = isStalemate ? "🤝 Stalemate - It's a Draw! 🤝" : "🤝 Game Over - Draw! 🤝";
            }
            
            // Show popup if game just ended
            if (!wasGameOver)
            {
                ShowGameEndPopup = true;
            }
        }
        else
        {
            GameStatusText = isInCheck ? "Check!" : "Game in Progress";
        }
        
        UpdateNavigationState();
    }

    private void UpdateNavigationState()
    {
        if (_chessBoard != null)
        {
            CanGoBack = _chessBoard.CanGoBack;
            CanGoForward = _chessBoard.CanGoForward;
            
            var currentMove = _chessBoard.Game.CurrentMoveIndex + 1;
            var totalMoves = _chessBoard.Game.MoveHistory.Count;
            MoveNavigationText = $"Move {currentMove} of {totalMoves}";
        }
    }

    public void OnSquareClicked(SquareViewModel square)
    {
        if (Program.EnableGameLogging)
        {
            Console.WriteLine($"[GAME] Square clicked: Row={square.Row}, Col={square.Column}, Piece={square.Piece?.ToString() ?? "Empty"}");
            Console.WriteLine($"[GAME] Current player: {_chessBoard.CurrentPlayer}");
            Console.WriteLine($"[GAME] Game over: {IsGameOver}");
            Console.WriteLine($"[GAME] Selected square: {SelectedSquare?.Row},{SelectedSquare?.Column ?? -1}");
        }
        
        if (IsGameOver) 
        {
            if (Program.EnableGameLogging)
            {
                Console.WriteLine("[GAME] Game is over, ignoring click");
            }
            return;
        }

        if (SelectedSquare == null)
        {
            if (Program.EnableGameLogging)
            {
                Console.WriteLine("[GAME] No piece selected, trying to select piece");
            }
            // Select a piece
            if (square.Piece != null && square.Piece.Color == _chessBoard.CurrentPlayer)
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Selecting piece: {square.Piece.Type} at {square.Row},{square.Column}");
                }
                SelectedSquare = square;
                HighlightValidMoves(square);
            }
            else
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Cannot select piece: Piece={square.Piece?.ToString() ?? "null"}, CurrentPlayer={_chessBoard.CurrentPlayer}");
                }
            }
        }
        else
        {
            if (square == SelectedSquare)
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine("[GAME] Deselecting piece");
                }
                // Deselect
                SelectedSquare = null;
                ClearHighlights();
            }
            else if (square.Piece != null && square.Piece.Color == _chessBoard.CurrentPlayer)
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Selecting different piece: {square.Piece.Type} at {square.Row},{square.Column}");
                }
                // Select different piece
                SelectedSquare = square;
                ClearHighlights();
                HighlightValidMoves(square);
            }
            else
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Attempting move from {SelectedSquare.Row},{SelectedSquare.Column} to {square.Row},{square.Column}");
                }
                // Try to make a move
                var move = new Move(new Position(SelectedSquare.Row, SelectedSquare.Column), new Position(square.Row, square.Column));
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Move object created: {move.GetNotation()}");
                }
                
                var isValid = _chessBoard.IsValidMove(move);
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Move is valid: {isValid}");
                }
                
                if (isValid)
                {
                    var moveResult = _chessBoard.MakeMove(move);
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine($"[GAME] Move result: {moveResult}");
                    }
                    
                    if (moveResult)
                    {
                        if (Program.EnableGameLogging)
                        {
                            Console.WriteLine("[GAME] Move successful, updating board and game status");
                        }
                        UpdateBoard();
                        UpdateGameStatus();
                        SelectedSquare = null;
                        ClearHighlights();
                        if (Program.EnableGameLogging)
                        {
                            Console.WriteLine($"[GAME] New current player: {_chessBoard.CurrentPlayer}");
                        }
                    }
                    else
                    {
                        if (Program.EnableGameLogging)
                        {
                            Console.WriteLine("[GAME] Move failed - keeping current player and selection");
                        }
                        // Don't change the current player or clear selection when move fails
                    }
                }
                else
                {
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine("[GAME] Move is not valid - keeping current player and selection");
                    }
                    // Don't change the current player or clear selection when move is invalid
                }
            }
        }
    }

    private void HighlightValidMoves(SquareViewModel fromSquare)
    {
        var fromPosition = new Position(fromSquare.Row, fromSquare.Column);
        if (Program.EnableGameLogging)
        {
            Console.WriteLine($"[GAME] Getting valid moves for piece at {fromPosition.Row},{fromPosition.Column}");
        }
        var validMoves = _chessBoard.GetValidMoves(fromPosition);
        if (Program.EnableGameLogging)
        {
            Console.WriteLine($"[GAME] Found {validMoves.Count} valid moves");
        }
        
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