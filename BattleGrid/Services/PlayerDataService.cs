using System.Text.Json;
using BattleGrid.Models;

namespace BattleGrid.Services;

// Shared local persistence used by both MTP-11 and MTP-12.
public sealed class PlayerDataService
{
    private readonly string path = Path.Combine(FileSystem.AppDataDirectory, "battlegrid_player.json");
    private readonly SemaphoreSlim gate = new(1, 1);
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    private PlayerRecord? cache;

    public async Task<PlayerRecord> LoadAsync()
    {
        await gate.WaitAsync();
        try
        {
            if (cache is not null) return cache;
            if (!File.Exists(path)) return cache = new PlayerRecord();
            var json = await File.ReadAllTextAsync(path);
            cache = JsonSerializer.Deserialize<PlayerRecord>(json) ?? new PlayerRecord();
            cache.Battles ??= new List<BattleRecord>();
            return cache;
        }
        catch (JsonException)
        {
            // Preserve the damaged file; don't silently destroy player data.
            throw new InvalidDataException("Saved BattleGrid data could not be read.");
        }
        finally { gate.Release(); }
    }

    public async Task SaveAsync()
    {
        var record = await LoadAsync();
        await gate.WaitAsync();
        try
        {
            var temp = path + ".tmp";
            await File.WriteAllTextAsync(temp, JsonSerializer.Serialize(record, Options));
            File.Move(temp, path, true);
        }
        finally { gate.Release(); }
    }

    // Call this from the completed battle logic in a future task.
    // The UI never invents sample wins, shots or scores.
    public async Task RecordBattleAsync(BattleRecord battle)
    {
        if (battle.Hits < 0 || battle.Misses < 0 || battle.ShipsDestroyed < 0 || battle.Score < 0)
            throw new ArgumentOutOfRangeException(nameof(battle), "Battle statistics cannot be negative.");
        var record = await LoadAsync();
        record.Battles.Add(battle);
        if (battle.Won)
        {
            record.Wins++;
            record.CurrentStreak++;
            record.BestStreak = Math.Max(record.BestStreak, record.CurrentStreak);
        }
        else
        {
            record.Losses++;
            record.CurrentStreak = 0;
        }
        record.TotalHits += battle.Hits;
        record.TotalShots += battle.Hits + battle.Misses;
        record.ShipsDestroyed += battle.ShipsDestroyed;
        record.BestScore = Math.Max(record.BestScore, battle.Score);
        record.Xp += Math.Max(0, battle.Score / 10) + (battle.Won ? 100 : 25);
        await SaveAsync();
    }
}
