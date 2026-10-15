namespace BattleGrid.Models;

public sealed class PlayerRecord
{
    public string CaptainName { get; set; } = "Captain";
    public string PreferredDifficulty { get; set; } = "Medium";
    public int Xp { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public int TotalShots { get; set; }
    public int TotalHits { get; set; }
    public int ShipsDestroyed { get; set; }
    public int BestScore { get; set; }
    public List<BattleRecord> Battles { get; set; } = new();
}

public sealed class BattleRecord
{
    public DateTime PlayedAt { get; set; } = DateTime.Now;
    public bool Won { get; set; }
    public string Difficulty { get; set; } = "Medium";
    public int Hits { get; set; }
    public int Misses { get; set; }
    public int ShipsDestroyed { get; set; }
    public int Score { get; set; }
}
