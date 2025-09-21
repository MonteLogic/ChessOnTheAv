using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ChessScrambler.Client.ViewModels;
using System;

namespace ChessScrambler.Client;

public partial class MainWindow : Window
{
    private ChessBoardViewModel? _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new ChessBoardViewModel();
        _viewModel = DataContext as ChessBoardViewModel;
        
        // Add UI event logging
        SetupUILogging();
    }

    private void SetupUILogging()
    {
        // Log window events
        this.Opened += (s, e) => LogUIEvent("Window", "Opened", "Application started");
        this.Closing += (s, e) => LogUIEvent("Window", "Closing", "Application closing");
        
        // Log keyboard events
        this.KeyDown += (s, e) => LogUIEvent("Keyboard", "KeyDown", $"Key: {e.Key}, Modifiers: {e.KeyModifiers}");
        this.KeyUp += (s, e) => LogUIEvent("Keyboard", "KeyUp", $"Key: {e.Key}");
        
        // Log mouse events
        this.PointerMoved += (s, e) => {
            var position = e.GetPosition(this);
            if (position.X % 50 == 0 || position.Y % 50 == 0) // Log every 50 pixels to avoid spam
                LogUIEvent("Mouse", "Moved", $"Position: ({position.X:F0}, {position.Y:F0})");
        };
        
        this.PointerEntered += (s, e) => LogUIEvent("Mouse", "Entered", "Mouse entered window");
        this.PointerExited += (s, e) => LogUIEvent("Mouse", "Exited", "Mouse left window");
        
        // Log focus events
        this.GotFocus += (s, e) => LogUIEvent("Focus", "GotFocus", "Window gained focus");
        this.LostFocus += (s, e) => LogUIEvent("Focus", "LostFocus", "Window lost focus");
        
        // Log window resize events (simplified)
        this.Resized += (s, e) => LogUIEvent("Window", "Resized", "Window resized");
        
        // Log window position changes (simplified)
        this.PositionChanged += (s, e) => LogUIEvent("Window", "PositionChanged", "Window position changed");
    }

    private void LogUIEvent(string category, string action, string details)
    {
        if (Program.EnableUILogging)
        {
            Console.WriteLine($"[UI] {category}.{action}: {details}");
        }
    }

    private void Square_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        LogUIEvent("ChessBoard", "SquareClicked", $"Sender: {sender?.GetType().Name}");
        
        if (sender is Border border && border.DataContext is SquareViewModel square)
        {
            LogUIEvent("ChessBoard", "SquareClicked", $"Square: Row={square.Row}, Col={square.Column}, Piece={square.Piece?.GetType().Name ?? "Empty"}");
            _viewModel?.OnSquareClicked(square);
        }
        else
        {
            LogUIEvent("ChessBoard", "SquareClicked", "Invalid sender or data context");
        }
    }

    private void NewPosition_Click(object sender, RoutedEventArgs e)
    {
        LogUIEvent("Button", "NewPosition", "New position button clicked");
        _viewModel?.LoadNewPosition();
    }

    private void ResetBoard_Click(object sender, RoutedEventArgs e)
    {
        LogUIEvent("Button", "ResetBoard", "Reset board button clicked");
        _viewModel?.LoadMiddlegamePosition();
    }

    private void Hint_Click(object sender, RoutedEventArgs e)
    {
        LogUIEvent("Button", "Hint", "Hint button clicked");
        // TODO: Implement hint system
        // This could show the best move or highlight tactical opportunities
    }

    private void ExportDebug_Click(object sender, RoutedEventArgs e)
    {
        LogUIEvent("Button", "ExportDebug", "Export debug button clicked");
        _viewModel?.ExportDebugState();
    }

    // Text selection logging for Move History

    private void MoveHistory_GotFocus(object sender, GotFocusEventArgs e)
    {
        LogUIEvent("TextSelection", "MoveHistory", "Move history text box gained focus");
    }

    private void MoveHistory_LostFocus(object sender, RoutedEventArgs e)
    {
        LogUIEvent("TextSelection", "MoveHistory", "Move history text box lost focus");
    }

    private void MoveHistory_KeyDown(object sender, KeyEventArgs e)
    {
        LogUIEvent("TextSelection", "MoveHistory", $"Key pressed: {e.Key}, Modifiers: {e.KeyModifiers}");
        
        // Log copy operations
        if (e.Key == Key.C && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            LogUIEvent("TextSelection", "MoveHistory", "Copy operation (Ctrl+C) detected");
        }
    }

    // Text selection logging for Instructions

    private void Instructions_GotFocus(object sender, GotFocusEventArgs e)
    {
        LogUIEvent("TextSelection", "Instructions", "Instructions text box gained focus");
    }

    private void Instructions_LostFocus(object sender, RoutedEventArgs e)
    {
        LogUIEvent("TextSelection", "Instructions", "Instructions text box lost focus");
    }

    private void Instructions_KeyDown(object sender, KeyEventArgs e)
    {
        LogUIEvent("TextSelection", "Instructions", $"Key pressed: {e.Key}, Modifiers: {e.KeyModifiers}");
        
        // Log copy operations
        if (e.Key == Key.C && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            LogUIEvent("TextSelection", "Instructions", "Copy operation (Ctrl+C) detected");
        }
    }

    private void NewGame_Click(object sender, RoutedEventArgs e)
    {
        LogUIEvent("Button", "NewGame", "New game button clicked from popup");
        _viewModel?.LoadNewPosition();
        _viewModel?.CloseGameEndPopup();
    }

    private void ClosePopup_Click(object sender, RoutedEventArgs e)
    {
        LogUIEvent("Button", "ClosePopup", "Close popup button clicked");
        _viewModel?.CloseGameEndPopup();
    }
}