using Avalonia.Controls;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ChessScrambler.VisualTests;

public class VisualRegressionTests : VisualTestBase
{
    private new static readonly string BaselineImagesDirectory = Path.Combine("baseline-images");

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task MainWindow_ShouldMatchBaseline()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        await Task.Delay(500); // Allow for initialization
        
        // Act
        var currentScreenshot = await TakeScreenshot(window, "MainWindow_Current");
        var baselinePath = Path.Combine(BaselineImagesDirectory, "MainWindow_Baseline.png");
        
        // Assert
        if (File.Exists(baselinePath))
        {
            var baselineImage = await LoadImage(baselinePath);
            var currentImage = await LoadImage(currentScreenshot);
            
            var matches = CompareImages(baselineImage, currentImage, threshold: 0.02);
            
            if (!matches)
            {
                SaveComparisonImage(baselineImage, currentImage, "MainWindow_Comparison");
                Assert.True(matches, "Current screenshot does not match baseline. Check comparison image for differences.");
            }
        }
        else
        {
            // First run - save as baseline
            Directory.CreateDirectory(BaselineImagesDirectory);
            File.Copy(currentScreenshot, baselinePath);
            Assert.True(true, "Baseline image created. Review and commit if correct.");
        }
    }

    [Fact(Skip = "Visual tests temporarily disabled due to complex Avalonia headless setup requirements")]
    public async Task ChessBoard_InitialPosition_ShouldMatchBaseline()
    {
        // Arrange
        var window = await CreateWindow<ChessScrambler.Client.MainWindow>();
        await Task.Delay(500);
        
        // Act
        var currentScreenshot = await TakeScreenshot(window, "ChessBoard_Current");
        var baselinePath = Path.Combine(BaselineImagesDirectory, "ChessBoard_Baseline.png");
        
        // Assert
        if (File.Exists(baselinePath))
        {
            var baselineImage = await LoadImage(baselinePath);
            var currentImage = await LoadImage(currentScreenshot);
            
            var matches = CompareImages(baselineImage, currentImage, threshold: 0.02);
            
            if (!matches)
            {
                SaveComparisonImage(baselineImage, currentImage, "ChessBoard_Comparison");
                Assert.True(matches, "Current chess board screenshot does not match baseline. Check comparison image for differences.");
            }
        }
        else
        {
            // First run - save as baseline
            Directory.CreateDirectory(BaselineImagesDirectory);
            File.Copy(currentScreenshot, baselinePath);
            Assert.True(true, "Chess board baseline image created. Review and commit if correct.");
        }
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
            var baselinePath = Path.Combine(BaselineImagesDirectory, $"MainWindow_Resize_{width}x{height}_Baseline.png");
            
            // Assert
            if (File.Exists(baselinePath))
            {
                var baselineImage = await LoadImage(baselinePath);
                var currentImage = await LoadImage(screenshotPath);
                
                var matches = CompareImages(baselineImage, currentImage, threshold: 0.05); // Higher threshold for resize tests
                
                if (!matches)
                {
                    SaveComparisonImage(baselineImage, currentImage, $"MainWindow_Resize_{width}x{height}_Comparison");
                    Assert.True(matches, $"Current screenshot for size {width}x{height} does not match baseline. Check comparison image for differences.");
                }
            }
            else
            {
                // First run - save as baseline
                Directory.CreateDirectory(BaselineImagesDirectory);
                File.Copy(screenshotPath, baselinePath);
                Assert.True(true, $"Baseline image for size {width}x{height} created. Review and commit if correct.");
            }
        }
    }
}
