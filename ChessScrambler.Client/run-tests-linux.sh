#!/bin/bash

# ChessScrambler Test Runner for Linux
# This script runs all tests locally on Linux with proper environment setup

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🎯 ChessScrambler Test Runner${NC}"
echo "=========================================="

# Check if we're in the right directory
if [ ! -f "ChessScrambler.Client.sln" ]; then
    echo -e "${RED}❌ Error: Please run this script from the ChessScrambler.Client directory${NC}"
    exit 1
fi

# Install dependencies if needed
if ! command -v xvfb-run &> /dev/null; then
    echo -e "${YELLOW}📦 Installing Xvfb for headless testing...${NC}"
    sudo apt-get update
    sudo apt-get install -y xvfb libx11-6 libx11-xcb1 libxcb1 libxss1 libgconf-2-4 libxrandr2 libasound2 libpangocairo-1.0-0 libatk1.0-0 libcairo-gobject2 libgtk-3-0 libgdk-pixbuf2.0-0
fi

# Set environment variables
export SOLUTION_NAME="ChessScrambler.Client.sln"
export TEST_PROJECT_PATH="ChessScrambler.VisualTests/ChessScrambler.VisualTests.csproj"
export CONFIGURATION="Release"

echo -e "${YELLOW}🏗️  Building solution...${NC}"
dotnet build $SOLUTION_NAME --configuration $CONFIGURATION

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Build failed${NC}"
    exit 1
fi

echo -e "${YELLOW}🧪 Running unit tests...${NC}"
dotnet test ChessScrambler.Tests/ChessScrambler.Tests.csproj --configuration $CONFIGURATION --no-build --logger "console;verbosity=normal"

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Unit tests failed${NC}"
    exit 1
fi

echo -e "${YELLOW}🎨 Running visual tests...${NC}"
xvfb-run -a dotnet test $TEST_PROJECT_PATH --configuration $CONFIGURATION --no-build --logger "console;verbosity=normal"

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ All tests passed!${NC}"
else
    echo -e "${RED}❌ Visual tests failed!${NC}"
    exit 1
fi

echo -e "${GREEN}🎉 All tests completed successfully!${NC}"
