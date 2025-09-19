using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ChessScrambler.Client.ViewModels;

namespace ChessScrambler.Client;

public partial class MainWindow : Window
{
    private ChessBoardViewModel? _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new ChessBoardViewModel();
        _viewModel = DataContext as ChessBoardViewModel;
    }

    private void Square_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.DataContext is SquareViewModel square)
        {
            _viewModel?.OnSquareClicked(square);
        }
    }

    private void NewPosition_Click(object sender, RoutedEventArgs e)
    {
        _viewModel?.LoadNewPosition();
    }

    private void ResetBoard_Click(object sender, RoutedEventArgs e)
    {
        _viewModel?.LoadMiddlegamePosition();
    }

    private void Hint_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Implement hint system
        // This could show the best move or highlight tactical opportunities
    }

    private void ExportDebug_Click(object sender, RoutedEventArgs e)
    {
        _viewModel?.ExportDebugState();
    }
}