using BattleGrid.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BattleGrid.ViewModels;

public sealed class DashboardViewModel : ObservableObject
{
    private readonly PlayerDataService data;
    private string captainName = "Captain", missionStatus = "Complete your first battle to populate statistics.";
    private int battles, wins, losses, shots, hits, misses, shipsDestroyed, bestScore, bestStreak, totalXp;
    private double accuracyRatio, winRatio;
    public DashboardViewModel(PlayerDataService data) => this.data = data;

    public string CaptainName { get => captainName; private set => SetProperty(ref captainName, value); }
    public string MissionStatus { get => missionStatus; private set => SetProperty(ref missionStatus, value); }
    public int Battles { get => battles; private set => SetProperty(ref battles, value); }
    public int Wins { get => wins; private set => SetProperty(ref wins, value); }
    public int Losses { get => losses; private set => SetProperty(ref losses, value); }
    public int Shots { get => shots; private set => SetProperty(ref shots, value); }
    public int Hits { get => hits; private set => SetProperty(ref hits, value); }
    public int Misses { get => misses; private set => SetProperty(ref misses, value); }
    public int ShipsDestroyed { get => shipsDestroyed; private set => SetProperty(ref shipsDestroyed, value); }
    public int BestScore { get => bestScore; private set => SetProperty(ref bestScore, value); }
    public int BestStreak { get => bestStreak; private set => SetProperty(ref bestStreak, value); }
    public int TotalXp { get => totalXp; private set => SetProperty(ref totalXp, value); }
    public double AccuracyRatio { get => accuracyRatio; private set => SetProperty(ref accuracyRatio, value); }
    public double WinRatio { get => winRatio; private set => SetProperty(ref winRatio, value); }
    public string AccuracyText => $"{AccuracyRatio:P1}";
    public string WinRateText => $"{WinRatio:P1}";
    public string RecordText => $"{Wins} W  /  {Losses} L";
    public string LevelText => $"LEVEL {TotalXp / 500 + 1}";

    public async Task LoadAsync()
    {
        try
        {
            var p = await data.LoadAsync();
            CaptainName = p.CaptainName;
            Wins = p.Wins; Losses = p.Losses;
            Battles = Wins + Losses;
            Shots = p.TotalShots; Hits = p.TotalHits;
            Misses = Math.Max(0, Shots - Hits);
            ShipsDestroyed = p.ShipsDestroyed;
            BestScore = p.BestScore;
            BestStreak = p.BestStreak;
            TotalXp = p.Xp;
            AccuracyRatio = Shots > 0 ? (double)Hits / Shots : 0;
            WinRatio = Battles > 0 ? (double)Wins / Battles : 0;
            MissionStatus = Battles == 0 ? "Complete your first battle to populate statistics." : $"{Battles} recorded missions. Keep commanding your fleet.";
            OnPropertyChanged(nameof(AccuracyText));
            OnPropertyChanged(nameof(WinRateText));
            OnPropertyChanged(nameof(RecordText));
            OnPropertyChanged(nameof(LevelText));
        }
        catch (Exception ex) { MissionStatus = $"Could not load statistics: {ex.Message}"; }
    }
}
