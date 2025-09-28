using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Platform;
using Avalonia.Skia;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ChessScrambler.VisualTests;

public abstract class VisualTestBase
{
    protected static readonly string ScreenshotsDirectory = Path.Combine(Environment.CurrentDirectory, "visual-test-screenshots");
    
    static VisualTestBase()
    {
        // Ensure screenshots directory exists
        Directory.CreateDirectory(ScreenshotsDirectory);
    }

    protected static AppBuilder CreateAppBuilder()
    {
        return AppBuilder.Configure<ChessScrambler.Client.App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .UseSkia()
            .WithInterFont()
            .LogToTrace();
    }

    protected static async Task<Window> CreateWindow<T>() where T : Window, new()
    {
        var app = CreateAppBuilder().SetupWithoutStarting();
        var window = new T();
        app.Instance.ApplicationLifetime = new HeadlessApplicationLifetime();
        await app.Instance.ApplicationLifetime.Start();
        window.Show();
        return window;
    }

    protected static async Task<string> TakeScreenshot(Window window, string testName)
    {
        // Wait for the window to be fully rendered
        await Task.Delay(100);
        
        var fileName = $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        var filePath = Path.Combine(ScreenshotsDirectory, fileName);
        
        // Use Avalonia's headless rendering to capture the window
        var pixelSize = new PixelSize(1920, 1080);
        var size = new Size(1920, 1080);
        
        using (var framebuffer = new HeadlessSkiaSurface(pixelSize))
        {
            window.Render(framebuffer);
            framebuffer.Save(filePath);
        }
        
        return filePath;
    }

    protected static async Task<Image<Rgba32>> LoadImage(string filePath)
    {
        return await Image.LoadAsync<Rgba32>(filePath);
    }

    protected static bool CompareImages(Image<Rgba32> image1, Image<Rgba32> image2, double threshold = 0.01)
    {
        if (image1.Width != image2.Width || image1.Height != image2.Height)
            return false;

        var differences = 0;
        var totalPixels = image1.Width * image1.Height;

        for (int y = 0; y < image1.Height; y++)
        {
            for (int x = 0; x < image1.Width; x++)
            {
                var pixel1 = image1[x, y];
                var pixel2 = image2[x, y];
                
                if (pixel1 != pixel2)
                {
                    differences++;
                }
            }
        }

        var differenceRatio = (double)differences / totalPixels;
        return differenceRatio <= threshold;
    }

    protected static void SaveComparisonImage(Image<Rgba32> baseline, Image<Rgba32> current, string testName)
    {
        var comparisonPath = Path.Combine(ScreenshotsDirectory, $"{testName}_comparison.png");
        
        // Create a side-by-side comparison image
        var width = Math.Max(baseline.Width, current.Width);
        var height = Math.Max(baseline.Height, current.Height) * 2;
        
        using var comparison = new Image<Rgba32>(width, height);
        
        // Copy baseline to top half
        for (int y = 0; y < baseline.Height; y++)
        {
            for (int x = 0; x < baseline.Width; x++)
            {
                comparison[x, y] = baseline[x, y];
            }
        }
        
        // Copy current to bottom half
        for (int y = 0; y < current.Height; y++)
        {
            for (int x = 0; x < current.Width; x++)
            {
                comparison[x, y + height / 2] = current[x, y];
            }
        }
        
        comparison.Save(comparisonPath);
    }
}
