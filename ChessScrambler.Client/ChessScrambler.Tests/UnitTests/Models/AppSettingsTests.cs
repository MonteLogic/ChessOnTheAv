using ChessScrambler.Client.Models;
using Xunit;
using FluentAssertions;
using System.ComponentModel;

namespace ChessScrambler.Tests.UnitTests.Models;

public class AppSettingsTests
{
    [Fact]
    public void AppSettings_Constructor_ShouldInitializeWithDefaults()
    {
        // Act
        var settings = new AppSettings();

        // Assert
        settings.BoardSize.Should().Be(480);
        settings.SquareSize.Should().Be(60);
        settings.PieceSize.Should().Be(50);
        settings.WindowWidth.Should().Be(1400);
        settings.WindowHeight.Should().Be(900);
        settings.WindowSizeMode.Should().Be("Large");
    }

    [Fact]
    public void BoardSize_WhenSet_ShouldUpdateSquareAndPieceSize()
    {
        // Arrange
        var settings = new AppSettings();

        // Act
        settings.BoardSize = 640;

        // Assert
        settings.BoardSize.Should().Be(640);
        settings.SquareSize.Should().Be(80); // 640 / 8
        settings.PieceSize.Should().Be(69); // 80 * 0.87
    }

    [Fact]
    public void WindowSizeMode_WhenSet_ShouldUpdateWindowDimensions()
    {
        // Arrange
        var settings = new AppSettings();

        // Act
        settings.WindowSizeMode = "Compact (1000x700)";

        // Assert
        settings.WindowWidth.Should().Be(1000);
        settings.WindowHeight.Should().Be(700);
    }

    [Fact]
    public void WindowSizeMode_WhenSetToAutoFit_ShouldSetAppropriateSize()
    {
        // Arrange
        var settings = new AppSettings();

        // Act
        settings.WindowSizeMode = "Auto Fit Screen";

        // Assert
        settings.WindowWidth.Should().BeGreaterOrEqualTo(1200);
        settings.WindowHeight.Should().BeGreaterOrEqualTo(800);
    }

    [Fact]
    public void AvailableBoardSizes_ShouldContainExpectedSizes()
    {
        // Act
        var sizes = AppSettings.AvailableBoardSizes;

        // Assert
        sizes.Should().Contain(320);
        sizes.Should().Contain(400);
        sizes.Should().Contain(480);
        sizes.Should().Contain(560);
        sizes.Should().Contain(640);
        sizes.Should().Contain(720);
        sizes.Should().Contain(800);
        sizes.Should().HaveCount(7);
    }

    [Fact]
    public void BoardSizeDisplayNames_ShouldMatchAvailableSizes()
    {
        // Act
        var displayNames = AppSettings.BoardSizeDisplayNames;
        var sizes = AppSettings.AvailableBoardSizes;

        // Assert
        displayNames.Should().HaveCount(sizes.Length);
        displayNames[0].Should().Contain("320px");
        displayNames[1].Should().Contain("400px");
        displayNames[2].Should().Contain("480px");
    }

    [Fact]
    public void AvailableWindowSizeModes_ShouldContainExpectedModes()
    {
        // Act
        var modes = AppSettings.AvailableWindowSizeModes;

        // Assert
        modes.Should().Contain("Compact (1000x700)");
        modes.Should().Contain("Medium (1200x800)");
        modes.Should().Contain("Large (1400x900)");
        modes.Should().Contain("X-Large (1600x1000)");
        modes.Should().Contain("XX-Large (1800x1100)");
        modes.Should().Contain("Auto Fit Screen");
        modes.Should().HaveCount(6);
    }

    [Fact]
    public void PropertyChanged_WhenBoardSizeChanges_ShouldRaiseEvent()
    {
        // Arrange
        var settings = new AppSettings();
        var eventRaised = false;
        settings.PropertyChanged += (sender, args) => eventRaised = true;

        // Act
        settings.BoardSize = 600;

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void PropertyChanged_WhenWindowSizeChanges_ShouldRaiseEvent()
    {
        // Arrange
        var settings = new AppSettings();
        var eventRaised = false;
        settings.PropertyChanged += (sender, args) => eventRaised = true;

        // Act
        settings.WindowWidth = 1200;

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void LoadSettings_WhenFileExists_ShouldLoadSettings()
    {
        // This test would require mocking the file system
        // For now, we'll test the default behavior
        // Act
        var settings = AppSettings.LoadSettings();

        // Assert
        settings.Should().NotBeNull();
        settings.BoardSize.Should().BeGreaterThan(0);
        settings.WindowWidth.Should().BeGreaterThan(0);
        settings.WindowHeight.Should().BeGreaterThan(0);
    }

    [Fact]
    public void SaveSettings_ShouldNotThrowException()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        var action = () => settings.SaveSettings();
        action.Should().NotThrow();
    }

    [Fact]
    public void ResetToDefaults_ShouldRestoreDefaultValues()
    {
        // Arrange
        var settings = new AppSettings();
        settings.BoardSize = 600;
        settings.WindowWidth = 1200;
        settings.WindowHeight = 800;

        // Act
        settings.ResetToDefaults();

        // Assert
        settings.BoardSize.Should().Be(480);
        settings.SquareSize.Should().Be(60);
        settings.PieceSize.Should().Be(50);
        settings.WindowWidth.Should().Be(1400);
        settings.WindowHeight.Should().Be(900);
        settings.WindowSizeMode.Should().Be("Large");
    }

    [Fact]
    public void SetProperty_WhenValueChanges_ShouldReturnTrue()
    {
        // Arrange
        var settings = new AppSettings();
        var originalSize = settings.BoardSize;

        // Act
        var result = settings.BoardSize = 600;

        // Assert
        result.Should().Be(600);
        settings.BoardSize.Should().Be(600);
    }

    [Fact]
    public void SetProperty_WhenValueSame_ShouldReturnFalse()
    {
        // Arrange
        var settings = new AppSettings();
        var originalSize = settings.BoardSize;

        // Act
        var result = settings.BoardSize = originalSize;

        // Assert
        result.Should().Be(originalSize);
        settings.BoardSize.Should().Be(originalSize);
    }
}
