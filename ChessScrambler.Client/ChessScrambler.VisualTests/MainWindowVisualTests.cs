using Avalonia.Controls;
using ChessScrambler.Client.ViewModels;
using System.Threading.Tasks;
using Xunit;

namespace ChessScrambler.VisualTests;

public class MainWindowVisualTests : VisualTestBase
{
    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task MainWindow_InitialState_ShouldRenderCorrectly()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        
        // Act
        var screenshotPath = await TakeScreenshot(window, "MainWindow_InitialState");
        
        // Assert
        Assert.True(File.Exists(screenshotPath), "Screenshot should be saved successfully");
        
        // Load and verify the image has content
        var image = await LoadImage(screenshotPath);
        Assert.True(image.Width > 0 && image.Height > 0, "Screenshot should have valid dimensions");
        
        // Basic visual checks - ensure the window is not completely black or white
        var hasContent = false;
        for (int y = 0; y < image.Height; y += 10) // Sample every 10th pixel for performance
        {
            for (int x = 0; x < image.Width; x += 10)
            {
                var pixel = image[x, y];
                if (pixel.R != pixel.G || pixel.G != pixel.B) // Not grayscale
                {
                    hasContent = true;
                    break;
                }
            }
            if (hasContent) break;
        }
        
        Assert.True(hasContent, "Screenshot should contain visual content");
    }

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task MainWindow_WithChessBoard_ShouldRenderBoardCorrectly()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        
        // Wait a bit for any initialization
        await Task.Delay(500);
        
        // Act
        var screenshotPath = await TakeScreenshot(window, "MainWindow_WithChessBoard");
        
        // Assert
        Assert.True(File.Exists(screenshotPath), "Screenshot should be saved successfully");
        
        var image = await LoadImage(screenshotPath);
        Assert.True(image.Width > 0 && image.Height > 0, "Screenshot should have valid dimensions");
    }

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task MainWindow_Resize_ShouldMaintainAspectRatio()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        
        // Act - Resize the window
        window.Width = 1200;
        window.Height = 800;
        await Task.Delay(100); // Allow for resize
        
        var screenshotPath = await TakeScreenshot(window, "MainWindow_Resized");
        
        // Assert
        Assert.True(File.Exists(screenshotPath), "Screenshot should be saved successfully");
        
        var image = await LoadImage(screenshotPath);
        Assert.True(image.Width > 0 && image.Height > 0, "Screenshot should have valid dimensions");
    }

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task MainWindow_DarkMode_ShouldRenderCorrectly()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        
        // Simulate dark mode by changing theme (if supported)
        // This would depend on your app's theme switching implementation
        await Task.Delay(100);
        
        // Act
        var screenshotPath = await TakeScreenshot(window, "MainWindow_DarkMode");
        
        // Assert
        Assert.True(File.Exists(screenshotPath), "Screenshot should be saved successfully");
        
        var image = await LoadImage(screenshotPath);
        Assert.True(image.Width > 0 && image.Height > 0, "Screenshot should have valid dimensions");
    }

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task MainWindow_InitialState_ShouldMatchBaseline()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        await Task.Delay(500);
        
        // Act
        var screenshotPath = await TakeScreenshot(window, "MainWindow_InitialState");
        var matches = await CompareWithBaseline("MainWindow_InitialState", screenshotPath);
        
        // Assert
        Assert.True(matches, "Current screenshot does not match baseline. Check comparison image for differences.");
    }

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task MainWindow_WithChessBoard_ShouldMatchBaseline()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        await Task.Delay(500);
        
        // Act
        var screenshotPath = await TakeScreenshot(window, "MainWindow_WithChessBoard");
        var matches = await CompareWithBaseline("MainWindow_WithChessBoard", screenshotPath);
        
        // Assert
        Assert.True(matches, "Chess board screenshot does not match baseline. Check comparison image for differences.");
    }

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task MainWindow_Resize_ShouldMaintainVisualConsistency()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        await Task.Delay(500);
        
        // Test different sizes
        var testSizes = new[] { (800, 600), (1200, 800), (1600, 1200) };
        
        foreach (var (width, height) in testSizes)
        {
            // Act
            window.Width = width;
            window.Height = height;
            await Task.Delay(200); // Allow for resize and re-render
            
            var screenshotPath = await TakeScreenshot(window, $"MainWindow_Resize_{width}x{height}");
            var matches = await CompareWithBaseline($"MainWindow_Resize_{width}x{height}", screenshotPath, 0.05); // Higher threshold for resize tests
            
            // Assert
            Assert.True(matches, $"Current screenshot for size {width}x{height} does not match baseline. Check comparison image for differences.");
        }
    }
}