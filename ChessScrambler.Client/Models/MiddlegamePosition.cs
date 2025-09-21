using System;

namespace ChessScrambler.Client.Models;

public class MiddlegamePosition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Fen { get; set; }
    public string Theme { get; set; }
    public int Difficulty { get; set; } // 1-5 scale
    public string[] KeyMoves { get; set; }
    public string[] TacticalThemes { get; set; }
    public string Hint { get; set; }

    public MiddlegamePosition(string id, string name, string description, string fen, string theme, int difficulty, string[] keyMoves, string[] tacticalThemes, string hint)
    {
        Id = id;
        Name = name;
        Description = description;
        Fen = fen;
        Theme = theme;
        Difficulty = difficulty;
        KeyMoves = keyMoves;
        TacticalThemes = tacticalThemes;
        Hint = hint;
    }
}

public class MiddlegamePositionDatabase
{
    private static readonly MiddlegamePosition[] Positions = new[]
    {
        new MiddlegamePosition(
            "pos001",
            "Classic King's Indian Attack",
            "A typical middlegame position from the King's Indian Attack. White has a strong center and good piece coordination.",
            "r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/3P1N2/PPP2PPP/RNBQK2R w KQkq - 4 4",
            "King's Indian Attack",
            3,
            new[] { "e4-e5", "Nf3-g5", "Bc4-b3" },
            new[] { "Center Control", "Piece Activity", "King Safety" },
            "Look for ways to increase piece activity and control the center."
        ),
        new MiddlegamePosition(
            "pos002",
            "Sicilian Dragon Position",
            "A complex middlegame from the Sicilian Dragon. Both sides have active pieces and tactical opportunities.",
            "r1bqkb1r/pppp1ppp/2n2n2/2b1p3/2B1P3/3P1N2/PPP2PPP/RNBQK2R w KQkq - 6 5",
            "Sicilian Dragon",
            4,
            new[] { "d4-d5", "Bc4-b3", "Nf3-g5" },
            new[] { "Tactics", "King Safety", "Pawn Structure" },
            "Focus on tactical opportunities and king safety in this sharp position."
        ),
        new MiddlegamePosition(
            "pos003",
            "Queen's Gambit Declined",
            "A solid middlegame from the Queen's Gambit Declined. Both sides have equal chances with different plans.",
            "rnbqk2r/pppp1ppp/4pn2/8/1b2P3/2N2N2/PPP2PPP/R1BQKB1R w KQkq - 2 4",
            "Queen's Gambit Declined",
            2,
            new[] { "e4-e5", "Bf1-d3", "O-O" },
            new[] { "Development", "Center Control", "Endgame Preparation" },
            "Complete your development and look for central breakthroughs."
        ),
        new MiddlegamePosition(
            "pos004",
            "French Defense Tarrasch",
            "A typical position from the French Defense Tarrasch variation. White has a space advantage but Black has counterplay.",
            "r1bqkb1r/pp2pppp/2n2n2/2pp4/2P1P3/2N2N2/PP2PPPP/R1BQKB1R w KQkq - 0 6",
            "French Defense",
            3,
            new[] { "c4-c5", "Bf1-d3", "O-O" },
            new[] { "Space Advantage", "Pawn Structure", "Counterplay" },
            "Use your space advantage while being aware of Black's counterplay."
        ),
        new MiddlegamePosition(
            "pos005",
            "English Opening",
            "A flexible middlegame from the English Opening. Both sides have various plans and ideas.",
            "r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/3P1N2/PPP2PPP/RNBQK2R w KQkq - 4 4",
            "English Opening",
            2,
            new[] { "e4-e5", "Nf3-g5", "Bc4-b3" },
            new[] { "Flexibility", "Piece Coordination", "Pawn Breaks" },
            "Look for pawn breaks and piece coordination in this flexible position."
        ),
        new MiddlegamePosition(
            "pos006",
            "Nimzo-Indian Defense",
            "A complex middlegame from the Nimzo-Indian Defense. Both sides have different strategic plans.",
            "r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/3P1N2/PPP2PPP/RNBQK2R w KQkq - 4 4",
            "Nimzo-Indian Defense",
            4,
            new[] { "d4-d5", "Bc4-b3", "Nf3-g5" },
            new[] { "Complex Strategy", "Piece Activity", "Pawn Structure" },
            "Navigate the complex strategic possibilities in this rich position."
        ),
        new MiddlegamePosition(
            "pos007",
            "Caro-Kann Defense",
            "A solid middlegame from the Caro-Kann Defense. Black has a solid position with good counterplay.",
            "r1bqkb1r/pp2pppp/2n2n2/2pp4/2P1P3/2N2N2/PP2PPPP/R1BQKB1R w KQkq - 0 6",
            "Caro-Kann Defense",
            3,
            new[] { "c4-c5", "Bf1-d3", "O-O" },
            new[] { "Solid Defense", "Counterplay", "Endgame Preparation" },
            "Look for ways to break through Black's solid defense."
        ),
        new MiddlegamePosition(
            "pos008",
            "Ruy Lopez Position",
            "A classic middlegame from the Ruy Lopez. Both sides have developed pieces and are ready for the middlegame battle.",
            "r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/3P1N2/PPP2PPP/RNBQK2R w KQkq - 4 4",
            "Ruy Lopez",
            3,
            new[] { "e4-e5", "Nf3-g5", "Bc4-b3" },
            new[] { "Classical Development", "Center Control", "Tactical Awareness" },
            "Apply classical principles while staying alert for tactical opportunities."
        )
    };

    public static MiddlegamePosition GetRandomPosition()
    {
        var random = new Random(Guid.NewGuid().GetHashCode());
        return Positions[random.Next(Positions.Length)];
    }

    public static MiddlegamePosition GetPositionById(string id)
    {
        return Array.Find(Positions, p => p.Id == id) ?? Positions[0];
    }

    public static MiddlegamePosition[] GetPositionsByTheme(string theme)
    {
        return Array.FindAll(Positions, p => p.Theme.Equals(theme, StringComparison.OrdinalIgnoreCase));
    }

    public static MiddlegamePosition[] GetPositionsByDifficulty(int difficulty)
    {
        return Array.FindAll(Positions, p => p.Difficulty == difficulty);
    }

    public static MiddlegamePosition[] GetAllPositions()
    {
        return (MiddlegamePosition[])Positions.Clone();
    }
}
