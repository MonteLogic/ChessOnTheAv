# ChessScrambler Test Runner for Windows
# This script runs all tests locally on Windows with proper environment setup

param(
    [Parameter(Position=0)]
    [string]$Configuration = "Release"
)

# Colors for output
$Red = "Red"
$Green = "Green"
$Yellow = "Yellow"
$Blue = "Blue"

Write-Host "🎯 ChessScrambler Test Runner" -ForegroundColor $Blue
Write-Host "==========================================" -ForegroundColor $Blue

# Check if we're in the right directory
if (-not (Test-Path "ChessScrambler.Client.sln")) {
    Write-Host "❌ Error: Please run this script from the ChessScrambler.Client directory" -ForegroundColor $Red
    exit 1
}

# Set environment variables
$env:SOLUTION_NAME = "ChessScrambler.Client.sln"
$env:TEST_PROJECT_PATH = "ChessScrambler.VisualTests\ChessScrambler.VisualTests.csproj"
$env:CONFIGURATION = $Configuration

Write-Host "🏗️  Building solution..." -ForegroundColor $Yellow
dotnet build $env:SOLUTION_NAME --configuration $env:CONFIGURATION

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed" -ForegroundColor $Red
    exit 1
}

Write-Host "🧪 Running unit tests..." -ForegroundColor $Yellow
dotnet test ChessScrambler.Tests\ChessScrambler.Tests.csproj --configuration $env:CONFIGURATION --no-build --logger "console;verbosity=normal"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Unit tests failed" -ForegroundColor $Red
    exit 1
}

Write-Host "🎨 Running visual tests..." -ForegroundColor $Yellow
dotnet test $env:TEST_PROJECT_PATH --configuration $env:CONFIGURATION --no-build --logger "console;verbosity=normal"

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ All tests passed!" -ForegroundColor $Green
} else {
    Write-Host "❌ Visual tests failed!" -ForegroundColor $Red
    exit 1
}

Write-Host "🎉 All tests completed successfully!" -ForegroundColor $Green
