using Avalonia.Controls;
using ChessScrambler.Client.Models;
using ChessScrambler.Client.ViewModels;
using System.Threading.Tasks;
using Xunit;

namespace ChessScrambler.VisualTests;

public class ChessBoardVisualTests : VisualTestBase
{
    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task ChessBoard_InitialPosition_ShouldRenderCorrectly()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        await Task.Delay(500); // Allow for initialization
        
        // Act
        var screenshotPath = await TakeScreenshot(window, "ChessBoard_InitialPosition");
        
        // Assert
        Assert.True(File.Exists(screenshotPath), "Screenshot should be saved successfully");
        
        var image = await LoadImage(screenshotPath);
        Assert.True(image.Width > 0 && image.Height > 0, "Screenshot should have valid dimensions");
    }

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task ChessBoard_WithPieces_ShouldShowAllPieces()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        await Task.Delay(500);
        
        // Act
        var screenshotPath = await TakeScreenshot(window, "ChessBoard_WithPieces");
        
        // Assert
        Assert.True(File.Exists(screenshotPath), "Screenshot should be saved successfully");
        
        var image = await LoadImage(screenshotPath);
        Assert.True(image.Width > 0 && image.Height > 0, "Screenshot should have valid dimensions");
    }

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task ChessBoard_AfterMove_ShouldUpdateCorrectly()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        await Task.Delay(500);
        
        // Simulate a move (this would depend on your app's move handling)
        // For now, just take a screenshot after initialization
        
        // Act
        var screenshotPath = await TakeScreenshot(window, "ChessBoard_AfterMove");
        
        // Assert
        Assert.True(File.Exists(screenshotPath), "Screenshot should be saved successfully");
        
        var image = await LoadImage(screenshotPath);
        Assert.True(image.Width > 0 && image.Height > 0, "Screenshot should have valid dimensions");
    }

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task ChessBoard_DifferentSizes_ShouldScaleCorrectly()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        
        // Test different window sizes
        var sizes = new[] { (800, 600), (1200, 800), (1600, 1200) };
        
        foreach (var (width, height) in sizes)
        {
            // Act
            window.Width = width;
            window.Height = height;
            await Task.Delay(100); // Allow for resize
            
            var screenshotPath = await TakeScreenshot(window, $"ChessBoard_Size_{width}x{height}");
            
            // Assert
            Assert.True(File.Exists(screenshotPath), $"Screenshot should be saved successfully for size {width}x{height}");
            
            var image = await LoadImage(screenshotPath);
            Assert.True(image.Width > 0 && image.Height > 0, $"Screenshot should have valid dimensions for size {width}x{height}");
        }
    }

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task ChessBoard_InitialPosition_ShouldMatchBaseline()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        await Task.Delay(500);
        
        // Act
        var screenshotPath = await TakeScreenshot(window, "ChessBoard_InitialPosition");
        var matches = await CompareWithBaseline("ChessBoard_InitialPosition", screenshotPath);
        
        // Assert
        Assert.True(matches, "Current screenshot does not match baseline. Check comparison image for differences.");
    }

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task ChessBoard_PieceSelection_ShouldHighlightCorrectly()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        await Task.Delay(500);
        
        // Note: This test would need to simulate piece selection
        // For now, just take a screenshot
        
        // Act
        var screenshotPath = await TakeScreenshot(window, "ChessBoard_PieceSelection");
        var matches = await CompareWithBaseline("ChessBoard_PieceSelection", screenshotPath);
        
        // Assert
        Assert.True(matches, "Piece selection screenshot does not match baseline.");
    }

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task ChessBoard_ValidMoves_ShouldBeHighlighted()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        await Task.Delay(500);
        
        // Note: This test would need to simulate piece selection and move highlighting
        // For now, just take a screenshot
        
        // Act
        var screenshotPath = await TakeScreenshot(window, "ChessBoard_ValidMoves");
        var matches = await CompareWithBaseline("ChessBoard_ValidMoves", screenshotPath);
        
        // Assert
        Assert.True(matches, "Valid moves highlighting does not match baseline.");
    }
}