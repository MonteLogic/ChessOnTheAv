@echo off
REM Visual Regression Testing Script for ChessScrambler (Windows)
REM This script helps run visual tests locally and manage baseline images

echo 🎯 ChessScrambler Visual Regression Testing
echo ==========================================

REM Check if we're in the right directory
if not exist "ChessScrambler.Client.csproj" (
    echo ❌ Error: Please run this script from the ChessScrambler.Client directory
    exit /b 1
)

if "%1"=="test" goto :run_tests
if "%1"=="baseline" goto :generate_baselines
if "%1"=="clean" goto :cleanup
if "%1"=="help" goto :show_help
if "%1"=="-h" goto :show_help
if "%1"=="--help" goto :show_help
if "%1"=="" goto :run_tests

:run_tests
echo 🔧 Setting up test environment...
echo 🏗️  Building solution...
dotnet build --configuration Release
if errorlevel 1 (
    echo ❌ Build failed
    exit /b 1
)

echo 🧪 Running visual regression tests...
dotnet test ChessScrambler.VisualTests\ChessScrambler.VisualTests.csproj --configuration Release --logger "console;verbosity=detailed"
if errorlevel 1 (
    echo ❌ Tests failed
    exit /b 1
)

echo ✅ Visual tests completed!
goto :end

:generate_baselines
echo 📸 Generating baseline images...

REM Create baseline directory if it doesn't exist
if not exist "ChessScrambler.VisualTests\baseline-images" mkdir "ChessScrambler.VisualTests\baseline-images"

echo 🔧 Running tests to generate screenshots...
dotnet test ChessScrambler.VisualTests\ChessScrambler.VisualTests.csproj --configuration Release --logger "console;verbosity=minimal"
if errorlevel 1 (
    echo ❌ Tests failed
    exit /b 1
)

REM Copy generated screenshots to baseline directory
if exist "visual-test-screenshots" (
    echo 📋 Copying screenshots to baseline directory...
    copy "visual-test-screenshots\*.png" "ChessScrambler.VisualTests\baseline-images\" >nul 2>&1
    echo ✅ Baseline images generated in ChessScrambler.VisualTests\baseline-images\
) else (
    echo ⚠️  No screenshots found. Make sure tests ran successfully.
)

goto :end

:cleanup
echo 🧹 Cleaning up test artifacts...
if exist "visual-test-screenshots" rmdir /s /q "visual-test-screenshots"
if exist "TestResults" rmdir /s /q "TestResults"
echo ✅ Cleanup completed!
goto :end

:show_help
echo Usage: %0 [command]
echo.
echo Commands:
echo   test        Run visual regression tests
echo   baseline    Generate baseline images
echo   clean       Clean up test artifacts
echo   help        Show this help message
echo.
echo Examples:
echo   %0 test              # Run all visual tests
echo   %0 baseline          # Generate baseline images
echo   %0 clean             # Clean up artifacts
goto :end

:end
