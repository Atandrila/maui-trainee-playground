using BattleGrid.Services;
using BattleGrid.ViewModels.Base;

namespace BattleGrid.ViewModels;

public sealed class DashboardViewModel : ObservableObject
{
    private readonly PlayerDataService _playerDataService;
    private string _captainName = "Captain", _missionStatus = "Complete your first battle to populate statistics.";
    private int _battles, _wins, _losses, _shots, _hits, _misses, _shipsDestroyed, _bestScore, _bestStreak, _totalXp;
    private double _accuracyRatio, _winRatio;
    public DashboardViewModel(PlayerDataService playerDataService) => _playerDataService = playerDataService;

    public string CaptainName { get => _captainName; private set => SetProperty(ref _captainName, value); }
    public string MissionStatus { get => _missionStatus; private set => SetProperty(ref _missionStatus, value); }
    public int Battles { get => _battles; private set => SetProperty(ref _battles, value); }
    public int Wins { get => _wins; private set => SetProperty(ref _wins, value); }
    public int Losses { get => _losses; private set => SetProperty(ref _losses, value); }
    public int Shots { get => _shots; private set => SetProperty(ref _shots, value); }
    public int Hits { get => _hits; private set => SetProperty(ref _hits, value); }
    public int Misses { get => _misses; private set => SetProperty(ref _misses, value); }
    public int ShipsDestroyed { get => _shipsDestroyed; private set => SetProperty(ref _shipsDestroyed, value); }
    public int BestScore { get => _bestScore; private set => SetProperty(ref _bestScore, value); }
    public int BestStreak { get => _bestStreak; private set => SetProperty(ref _bestStreak, value); }
    public int TotalXp { get => _totalXp; private set => SetProperty(ref _totalXp, value); }
    public double AccuracyRatio { get => _accuracyRatio; private set => SetProperty(ref _accuracyRatio, value); }
    public double WinRatio { get => _winRatio; private set => SetProperty(ref _winRatio, value); }
    public string AccuracyText => $"{AccuracyRatio:P1}";
    public string WinRateText => $"{WinRatio:P1}";
    public string RecordText => $"{Wins} W  /  {Losses} L";
    public string LevelText => $"LEVEL {TotalXp / 500 + 1}";

    public async Task LoadAsync()
    {
        try
        {
            var p = await _playerDataService.LoadAsync();
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
