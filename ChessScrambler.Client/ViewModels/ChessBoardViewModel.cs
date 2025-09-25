using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ChessScrambler.Client.Models;

namespace ChessScrambler.Client.ViewModels;

/**
 * <summary>
 * Represents a single square on the chess board with its visual state and properties.
 * </summary>
 */
public class SquareViewModel : INotifyPropertyChanged
{
    private ChessPiece _piece;
    private bool _isSelected;
    private bool _isHighlighted;
    private bool _isLightSquare;

    /**
     * <summary>
     * Gets or sets the chess piece occupying this square.
     * </summary>
     */
    public ChessPiece Piece
    {
        get => _piece;
        set => SetProperty(ref _piece, value);
    }

    /**
     * <summary>
     * Gets or sets a value indicating whether this square is currently selected.
     * </summary>
     */
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    /**
     * <summary>
     * Gets or sets a value indicating whether this square is highlighted (e.g., for valid moves).
     * </summary>
     */
    public bool IsHighlighted
    {
        get => _isHighlighted;
        set => SetProperty(ref _isHighlighted, value);
    }

    /**
     * <summary>
     * Gets or sets a value indicating whether this square is a light-colored square.
     * </summary>
     */
    public bool IsLightSquare
    {
        get => _isLightSquare;
        set => SetProperty(ref _isLightSquare, value);
    }

    /**
     * <summary>
     * Gets or sets the row index of this square on the chess board (0-7).
     * </summary>
     */
    public int Row { get; set; }
    
    /**
     * <summary>
     * Gets or sets the column index of this square on the chess board (0-7).
     * </summary>
     */
    public int Column { get; set; }
    
    /**
     * <summary>
     * Gets the algebraic notation position of this square (e.g., "e4", "a1").
     * </summary>
     */
    public string Position => $"{(char)('a' + Column)}{8 - Row}";

    /**
     * <summary>
     * Occurs when a property value changes.
     * </summary>
     */
    public event PropertyChangedEventHandler PropertyChanged;

    /**
     * <summary>
     * Raises the PropertyChanged event for the specified property.
     * </summary>
     * <param name="propertyName">The name of the property that changed. If null, the caller member name is used.</param>
     */
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

/**
 * <summary>
 * ViewModel for the chess board that manages the game state, UI bindings, and user interactions.
 * </summary>
 */
public class ChessBoardViewModel : INotifyPropertyChanged
{
    private ChessBoard _chessBoard;
    private SquareViewModel _selectedSquare;
    private string _currentPlayerText;
    private string _moveHistoryText;
    private bool _isGameOver;
    private string _gameStatusText;
    private string _whitePlayerText;
    private string _blackPlayerText;
    private List<string> _currentGameMoves;
    private bool _showGameEndPopup;
    private string _gameEndMessage;
    private string _gameIdText;
    private bool _canGoBack;
    private bool _canGoForward;
    private string _moveNavigationText;
    private string _gamesBankStatus;
    private string _currentFenPosition;
    private AppSettings _appSettings;

    /**
     * <summary>
     * Gets the collection of squares that make up the chess board.
     * </summary>
     */
    public ObservableCollection<SquareViewModel> Squares { get; } = new ObservableCollection<SquareViewModel>();

    /**
     * <summary>
     * Gets or sets the currently selected square on the chess board.
     * </summary>
     */
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

    /**
     * <summary>
     * Gets or sets the text displaying the current player's turn.
     * </summary>
     */
    public string CurrentPlayerText
    {
        get => _currentPlayerText;
        set => SetProperty(ref _currentPlayerText, value);
    }

    /**
     * <summary>
     * Gets or sets the text displaying the move history.
     * </summary>
     */
    public string MoveHistoryText
    {
        get => _moveHistoryText;
        set => SetProperty(ref _moveHistoryText, value);
    }

    /**
     * <summary>
     * Gets or sets a value indicating whether the game is over.
     * </summary>
     */
    public bool IsGameOver
    {
        get => _isGameOver;
        set => SetProperty(ref _isGameOver, value);
    }

    /**
     * <summary>
     * Gets or sets the text displaying the current game status.
     * </summary>
     */
    public string GameStatusText
    {
        get => _gameStatusText;
        set => SetProperty(ref _gameStatusText, value);
    }

    /**
     * <summary>
     * Gets or sets the text displaying the white player's name.
     * </summary>
     */
    public string WhitePlayerText
    {
        get => _whitePlayerText;
        set => SetProperty(ref _whitePlayerText, value);
    }

    /**
     * <summary>
     * Gets or sets the text displaying the black player's name.
     * </summary>
     */
    public string BlackPlayerText
    {
        get => _blackPlayerText;
        set => SetProperty(ref _blackPlayerText, value);
    }

    /**
     * <summary>
     * Gets or sets a value indicating whether the game end popup should be shown.
     * </summary>
     */
    public bool ShowGameEndPopup
    {
        get => _showGameEndPopup;
        set => SetProperty(ref _showGameEndPopup, value);
    }

    /**
     * <summary>
     * Gets or sets the message to display in the game end popup.
     * </summary>
     */
    public string GameEndMessage
    {
        get => _gameEndMessage;
        set => SetProperty(ref _gameEndMessage, value);
    }

    /**
     * <summary>
     * Gets or sets the text displaying the current game ID.
     * </summary>
     */
    public string GameIdText
    {
        get => _gameIdText;
        set => SetProperty(ref _gameIdText, value);
    }

    /**
     * <summary>
     * Gets or sets a value indicating whether the user can navigate to the previous move.
     * </summary>
     */
    public bool CanGoBack
    {
        get => _canGoBack;
        set => SetProperty(ref _canGoBack, value);
    }

    /**
     * <summary>
     * Gets or sets a value indicating whether the user can navigate to the next move.
     * </summary>
     */
    public bool CanGoForward
    {
        get => _canGoForward;
        set => SetProperty(ref _canGoForward, value);
    }

    /**
     * <summary>
     * Gets or sets the text displaying the current move position in the game.
     * </summary>
     */
    public string MoveNavigationText
    {
        get => _moveNavigationText;
        set => SetProperty(ref _moveNavigationText, value);
    }

    /**
     * <summary>
     * Gets or sets the text displaying the status of the games bank.
     * </summary>
     */
    public string GamesBankStatus
    {
        get => _gamesBankStatus;
        set => SetProperty(ref _gamesBankStatus, value);
    }

    /**
     * <summary>
     * Gets or sets the current FEN position string of the chess board.
     * </summary>
     */
    public string CurrentFenPosition
    {
        get => _currentFenPosition;
        set => SetProperty(ref _currentFenPosition, value);
    }

    /**
     * <summary>
     * Gets or sets the application settings including board size preferences.
     * </summary>
     */
    public AppSettings AppSettings
    {
        get => _appSettings;
        set => SetProperty(ref _appSettings, value);
    }

    /**
     * <summary>
     * Initializes a new instance of the ChessBoardViewModel class.
     * </summary>
     */
    public ChessBoardViewModel()
    {
        _whitePlayerText = "White: Loading...";
        _blackPlayerText = "Black: Loading...";
        _currentGameMoves = new List<string>();
        _appSettings = AppSettings.LoadSettings(); // Load saved settings
        InitializeBoard();
        LoadSampleGames();
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

    private void LoadSampleGames()
    {
        try
        {
            // Try multiple possible locations for the sample games file
            var possiblePaths = new[]
            {
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample_games.pgn"),
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "sample_games.pgn"),
                "sample_games.pgn"
            };

            string? sampleGamesPath = null;
            foreach (var path in possiblePaths)
            {
                if (System.IO.File.Exists(path))
                {
                    sampleGamesPath = path;
                    break;
                }
            }

            if (sampleGamesPath != null)
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Loading sample games from: {sampleGamesPath}");
                }

                GameBank.ImportGamesFromFile(sampleGamesPath);
                var count = GameBank.GetImportedGamesCount();

                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Successfully loaded {count} sample games");
                }
            }
            else
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Sample games file not found in any of the expected locations");
                }
            }
        }
        catch (Exception ex)
        {
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Error loading sample games: {ex.Message}");
            }
        }
    }

    /**
     * <summary>
     * Loads a middlegame position from the imported games or predefined positions.
     * </summary>
     */
    public void LoadMiddlegamePosition()
    {
        // Try to load the COTA game from imported games first
        var cotaGame = GameBank.GetCotaGame();
        if (cotaGame != null)
        {
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Loading COTA game from PGN: {cotaGame.GetDisplayName()}");
            }

            // Get the middlegame position from the COTA game
            var fen = cotaGame.GetMiddlegamePositionFen();
            _chessBoard = new ChessBoard(fen);

            // Store the current game's moves for move history
            _currentGameMoves = cotaGame.Moves;

            // Update game info to show it's from the COTA game
            GameIdText = $"Game: {cotaGame.GetDisplayName()}";
            WhitePlayerText = $"White: {cotaGame.WhitePlayer}";
            BlackPlayerText = $"Black: {cotaGame.BlackPlayer}";

            // Update move history display
            UpdateMoveHistory();

            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Successfully loaded COTA game with {cotaGame.Moves.Count} moves");
            }
        }
        else
        {
            // Fallback to the old method if COTA game not found
            if (Program.EnableGameLogging)
            {
                Console.WriteLine("[GAME] COTA game not found, falling back to random position generation");
            }

            var position = MiddlegamePositionDatabase.GetPositionById("pos001");
            _chessBoard = new ChessBoard(position.Fen);

            // Generate move history for this position
            GenerateMoveHistoryForPosition(position);
        }

        UpdateBoard();
        UpdateGameStatus();
    }

    private void GenerateMoveHistoryForPosition(MiddlegamePosition position)
    {
        try
        {
            // Create a new chess game from the starting position
            var startingGame = new ChessDotNet.ChessGame();
            var moves = new List<string>();

            // Generate a reasonable number of moves to reach the middlegame position
            // We'll make 8-15 moves to simulate a typical opening to middlegame transition
            var random = new Random(Guid.NewGuid().GetHashCode());
            var moveCount = random.Next(8, 16);

            for (int i = 0; i < moveCount; i++)
            {
                try
                {
                    var validMoves = startingGame.GetValidMoves(startingGame.WhoseTurn).ToList();
                    if (validMoves.Count == 0) break;

                    // Select a random valid move
                    var randomMove = validMoves[random.Next(validMoves.Count)];
                    startingGame.MakeMove(randomMove, true);

                    // Convert to SAN notation
                    var sanMove = randomMove.ToString();
                    moves.Add(sanMove);

                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine($"[GAME] Generated move {i + 1}: {sanMove}");
                    }
                }
                catch (Exception ex)
                {
                    if (Program.EnableGameLogging)
                    {
                        Console.WriteLine($"[GAME] Error generating move {i + 1}: {ex.Message}");
                    }
                    break;
                }
            }

            // Store the generated moves
            _currentGameMoves = moves;

            // Update the game info
            GameIdText = $"Position: {position.Name}";
            WhitePlayerText = "White: COTA Player 1";
            BlackPlayerText = "Black: COTA Player 2";

            // Update move history display
            UpdateMoveHistory();

            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Generated {moves.Count} moves for position: {position.Name}");
            }
        }
        catch (Exception ex)
        {
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Error generating move history: {ex.Message}");
            }

            // Fallback to empty move history
            _currentGameMoves = new List<string>();
            UpdateMoveHistory();
        }
    }

    /**
     * <summary>
     * Loads a new chess position asynchronously from imported games or predefined positions.
     * </summary>
     * <returns>A task representing the asynchronous operation.</returns>
     */
    public async Task LoadNewPosition()
    {
        if (Program.EnableGameLogging || Program.EnableFullLogging)
        {
            Console.WriteLine("[GAME] LoadNewPosition called");
        }

        try
        {
            var importedGamesCount = GameBank.GetImportedGamesCount();
            Console.WriteLine($"[UI] LoadNewPosition - Imported games count: {importedGamesCount}");

            // Try to load from imported games first
            if (importedGamesCount > 0)
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine("[GAME] Loading from imported games");
                }

                var importedGame = GameBank.GetRandomMiddlegamePosition();
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Selected imported game: {importedGame.GetDisplayName()}");
                    Console.WriteLine($"[GAME] Game has {importedGame.Moves.Count} moves");
                }

                // Run the heavy move replay operation on a background thread
                var fen = await Task.Run(() => importedGame.GetMiddlegamePositionFen());
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Generated FEN: {fen}");
                }

                _chessBoard = new ChessBoard(fen);

                // Update game info to show it's from an imported game
                GameIdText = $"Game: {importedGame.GetDisplayName()}";
                WhitePlayerText = $"White: {importedGame.WhitePlayer}";
                BlackPlayerText = $"Black: {importedGame.BlackPlayer}";

                // Store the current game's moves for move history
                _currentGameMoves = importedGame.Moves;
                UpdateMoveHistory();

                if (Program.EnableGameLogging || Program.EnableFullLogging)
                {
                    Console.WriteLine($"[GAME] Successfully loaded imported game position");
                    Console.WriteLine($"[GAME] Current Game ID: {GameIdText}");
                }
            }
            else
            {
                if (Program.EnableGameLogging || Program.EnableFullLogging)
                {
                    Console.WriteLine("[GAME] No imported games, using predefined positions");
                }

                // Fallback to predefined positions if no games imported
                var position = MiddlegamePositionDatabase.GetRandomPosition();
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Selected predefined position: {position.Name}");
                    Console.WriteLine($"[GAME] Position FEN: {position.Fen}");
                }

                _chessBoard = new ChessBoard(position.Fen);

                // Generate move history for this position
                GenerateMoveHistoryForPosition(position);

                if (Program.EnableGameLogging || Program.EnableFullLogging)
                {
                    Console.WriteLine($"[GAME] Successfully loaded predefined position");
                    Console.WriteLine($"[GAME] Current Game ID: {GameIdText}");
                }
            }

            UpdateBoard();
            UpdateGameStatus();

            if (Program.EnableGameLogging || Program.EnableFullLogging)
            {
                Console.WriteLine("[GAME] LoadNewPosition completed successfully");
            }
        }
        catch (Exception ex)
        {
            if (Program.EnableGameLogging)
            {
                Console.WriteLine($"[GAME] Error in LoadNewPosition: {ex.Message}");
                Console.WriteLine($"[GAME] Stack trace: {ex.StackTrace}");
            }

            // Fallback to predefined positions on error
            try
            {
                var position = MiddlegamePositionDatabase.GetRandomPosition();
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Fallback to predefined position: {position.Name}");
                }

                _chessBoard = new ChessBoard(position.Fen);

                // Generate move history for this position
                GenerateMoveHistoryForPosition(position);

                UpdateBoard();
                UpdateGameStatus();

                if (Program.EnableGameLogging || Program.EnableFullLogging)
                {
                    Console.WriteLine("[GAME] Fallback completed successfully");
                    Console.WriteLine($"[GAME] Current Game ID: {GameIdText}");
                }
            }
            catch (Exception fallbackEx)
            {
                if (Program.EnableGameLogging)
                {
                    Console.WriteLine($"[GAME] Fallback also failed: {fallbackEx.Message}");
                }

                GameStatusText = $"Error loading position: {ex.Message}";
            }
        }
    }

    /**
     * <summary>
     * Exports the current debug state of the chess board to a file on the desktop.
     * </summary>
     */
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

    /**
     * <summary>
     * Closes the game end popup dialog.
     * </summary>
     */
    public void CloseGameEndPopup()
    {
        ShowGameEndPopup = false;
    }

    /**
     * <summary>
     * Imports chess games from a PGN file.
     * </summary>
     * <param name="filePath">The path to the PGN file to import.</param>
     */
    public void ImportGamesFromFile(string filePath)
    {
        Console.WriteLine($"[UI] ImportGamesFromFile called with path: {filePath}");

        try
        {
            Console.WriteLine($"[UI] File exists: {System.IO.File.Exists(filePath)}");

            // Clear existing games first
            GameBank.ClearGames();
            Console.WriteLine($"[UI] Cleared existing games, count: {GameBank.GetImportedGamesCount()}");

            GameBank.ImportGamesFromFile(filePath);
            var count = GameBank.GetImportedGamesCount();
            Console.WriteLine($"[UI] Import completed, total games: {count}");

            GameStatusText = $"Imported {count} games from file";
            UpdateGamesBankStatus();

            Console.WriteLine($"[UI] GamesBankStatus updated: {GamesBankStatus}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UI] Error importing games: {ex.Message}");
            Console.WriteLine($"[UI] Stack trace: {ex.StackTrace}");

            GameStatusText = $"Error importing games: {ex.Message}";
        }
    }

    /**
     * <summary>
     * Imports chess games from PGN content string.
     * </summary>
     * <param name="pgnContent">The PGN content string to import.</param>
     */
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

    /**
     * <summary>
     * Gets the number of imported games in the games bank.
     * </summary>
     * <returns>The number of imported games.</returns>
     */
    public int GetImportedGamesCount()
    {
        return GameBank.GetImportedGamesCount();
    }

    /**
     * <summary>
     * Clears all imported games from the games bank.
     * </summary>
     */
    public void ClearImportedGames()
    {
        GameBank.ClearGames();
        GameStatusText = "Imported games cleared";
        UpdateGamesBankStatus();
    }

    /**
     * <summary>
     * Gets the current game in PGN format.
     * </summary>
     * <returns>A string containing the current game in PGN format.</returns>
     */
    public string GetCurrentGamePgn()
    {
        if (_chessBoard == null) return "";

        var pgn = new System.Text.StringBuilder();

        // Add PGN headers
        pgn.AppendLine("[Event \"ChessScrambler Game\"]");
        pgn.AppendLine($"[Site \"ChessScrambler\"]");
        pgn.AppendLine($"[Date \"{DateTime.Now:yyyy.MM.dd}\"]");
        pgn.AppendLine("[Round \"1\"]");
        pgn.AppendLine("[White \"Player\"]");
        pgn.AppendLine("[Black \"Player\"]");
        pgn.AppendLine($"[Result \"{_chessBoard.Game.GameResult}\"]");
        pgn.AppendLine($"[FEN \"{_chessBoard.Game.InitialFen}\"]");
        pgn.AppendLine();

        // Add moves
        var moves = _chessBoard.Game.GetMovesUpToCurrent();
        var moveText = new List<string>();

        for (int i = 0; i < moves.Count; i++)
        {
            if (i % 2 == 0)
            {
                // White move - add move number
                var moveNumber = (i / 2) + 1;
                moveText.Add($"{moveNumber}. {moves[i].GetNotation()}");
            }
            else
            {
                // Black move - just add the move
                moveText.Add(moves[i].GetNotation());
            }
        }

        pgn.AppendLine(string.Join(" ", moveText));
        pgn.AppendLine($" {_chessBoard.Game.GameResult}");

        return pgn.ToString();
    }

    /**
     * <summary>
     * Exports the current game to a PGN file on the desktop.
     * </summary>
     */
    public void ExportCurrentGamePgn()
    {
        try
        {
            var pgnContent = GetCurrentGamePgn();
            var fileName = $"chess_game_{DateTime.Now:yyyyMMdd_HHmmss}.pgn";
            var filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            System.IO.File.WriteAllText(filePath, pgnContent);

            GameStatusText = $"Game exported to: {fileName}";
        }
        catch (Exception ex)
        {
            GameStatusText = $"Error exporting game: {ex.Message}";
        }
    }


    private void UpdateGamesBankStatus()
    {
        var count = GetImportedGamesCount();
        GamesBankStatus = count > 0 ? $"Games loaded: {count}" : "No games imported";
    }

    /**
     * <summary>
     * Navigates to the first move of the current game.
     * </summary>
     */
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

    /**
     * <summary>
     * Navigates to the last move of the current game.
     * </summary>
     */
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

    /**
     * <summary>
     * Navigates to the previous move of the current game.
     * </summary>
     */
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

    /**
     * <summary>
     * Navigates to the next move of the current game.
     * </summary>
     */
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

        // Use the current game moves for move history display
        UpdateMoveHistory();

        // Update the current FEN position
        CurrentFenPosition = _chessBoard.GetFen();

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

    private void UpdateMoveHistory()
    {
        if (_currentGameMoves != null && _currentGameMoves.Count > 0)
        {
            // Format the moves in a readable way with proper move numbers
            var moveHistory = new List<string>();
            for (int i = 0; i < _currentGameMoves.Count; i += 2)
            {
                var moveNumber = (i / 2) + 1;
                var whiteMove = _currentGameMoves[i];
                var blackMove = i + 1 < _currentGameMoves.Count ? _currentGameMoves[i + 1] : "";

                if (!string.IsNullOrEmpty(blackMove))
                {
                    moveHistory.Add($"{moveNumber}. {whiteMove} {blackMove}");
                }
                else
                {
                    moveHistory.Add($"{moveNumber}. {whiteMove}");
                }
            }

            MoveHistoryText = string.Join("\n", moveHistory);
        }
        else
        {
            MoveHistoryText = "No moves available";
        }
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

    /**
     * <summary>
     * Handles the click event on a chess square.
     * </summary>
     * <param name="square">The square that was clicked.</param>
     */
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

    /**
     * <summary>
     * Occurs when a property value changes.
     * </summary>
     */
    public event PropertyChangedEventHandler PropertyChanged;

    /**
     * <summary>
     * Raises the PropertyChanged event for the specified property.
     * </summary>
     * <param name="propertyName">The name of the property that changed. If null, the caller member name is used.</param>
     */
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /**
     * <summary>
     * Sets the property value and raises PropertyChanged if the value has changed.
     * </summary>
     * <typeparam name="T">The type of the property.</typeparam>
     * <param name="field">The backing field for the property.</param>
     * <param name="value">The new value for the property.</param>
     * <param name="propertyName">The name of the property. If null, the caller member name is used.</param>
     * <returns>True if the property value was changed; otherwise, false.</returns>
     */
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}