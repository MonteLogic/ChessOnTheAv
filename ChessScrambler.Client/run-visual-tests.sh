#!/bin/bash

# Visual Regression Testing Script for ChessScrambler
# This script helps run visual tests locally and manage baseline images

set -e

echo "🎯 ChessScrambler Visual Regression Testing"
echo "=========================================="

# Check if we're in the right directory
if [ ! -f "ChessScrambler.Client.csproj" ]; then
    echo "❌ Error: Please run this script from the ChessScrambler.Client directory"
    exit 1
fi

# Function to run visual tests
run_tests() {
    echo "🔧 Setting up test environment..."
    
    # Install dependencies if needed
    if ! command -v xvfb-run &> /dev/null; then
        echo "📦 Installing Xvfb for headless testing..."
        sudo apt-get update
        sudo apt-get install -y xvfb libx11-6 libx11-xcb1 libxcb1 libxss1 libgconf-2-4 libxrandr2 libasound2 libpangocairo-1.0-0 libatk1.0-0 libcairo-gobject2 libgtk-3-0 libgdk-pixbuf2.0-0
    fi
    
    echo "🏗️  Building solution..."
    dotnet build --configuration Release
    
    echo "🧪 Running visual regression tests..."
    xvfb-run -a dotnet test ChessScrambler.VisualTests/ChessScrambler.VisualTests.csproj --configuration Release --logger "console;verbosity=detailed"
    
    echo "✅ Visual tests completed!"
}

# Function to generate baseline images
generate_baselines() {
    echo "📸 Generating baseline images..."
    
    # Create baseline directory if it doesn't exist
    mkdir -p ChessScrambler.VisualTests/baseline-images
    
    # Run tests to generate current screenshots
    echo "🔧 Running tests to generate screenshots..."
    xvfb-run -a dotnet test ChessScrambler.VisualTests/ChessScrambler.VisualTests.csproj --configuration Release --logger "console;verbosity=minimal"
    
    # Copy generated screenshots to baseline directory
    if [ -d "visual-test-screenshots" ]; then
        echo "📋 Copying screenshots to baseline directory..."
        cp visual-test-screenshots/*.png ChessScrambler.VisualTests/baseline-images/ 2>/dev/null || true
        echo "✅ Baseline images generated in ChessScrambler.VisualTests/baseline-images/"
    else
        echo "⚠️  No screenshots found. Make sure tests ran successfully."
    fi
}

# Function to clean up test artifacts
cleanup() {
    echo "🧹 Cleaning up test artifacts..."
    rm -rf visual-test-screenshots
    rm -rf TestResults
    echo "✅ Cleanup completed!"
}

# Function to show help
show_help() {
    echo "Usage: $0 [command]"
    echo ""
    echo "Commands:"
    echo "  test        Run visual regression tests"
    echo "  baseline    Generate baseline images"
    echo "  clean       Clean up test artifacts"
    echo "  help        Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 test              # Run all visual tests"
    echo "  $0 baseline          # Generate baseline images"
    echo "  $0 clean             # Clean up artifacts"
}

# Main script logic
case "${1:-test}" in
    "test")
        run_tests
        ;;
    "baseline")
        generate_baselines
        ;;
    "clean")
        cleanup
        ;;
    "help"|"-h"|"--help")
        show_help
        ;;
    *)
        echo "❌ Unknown command: $1"
        show_help
        exit 1
        ;;
esac
