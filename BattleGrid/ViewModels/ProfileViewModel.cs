using BattleGrid.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace BattleGrid.ViewModels;

public sealed class ProfileViewModel : ObservableObject
{
    private readonly PlayerDataService data;
    private string captainName = "Captain";
    private string difficulty = "Medium";
    private string statusMessage = "Change your captain name or difficulty, then save.";
    private int level = 1, xp, wins, losses, bestStreak, currentStreak;
    private string rank = "Recruit", achievements = "No medals unlocked yet.";
    private double levelProgress;
    private bool isBusy;

    public ProfileViewModel(PlayerDataService data)
    {
        this.data = data;
        SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
    }

    public string CaptainName { get => captainName; set => SetProperty(ref captainName, value); }
    public string Difficulty { get => difficulty; set => SetProperty(ref difficulty, value); }
    public string StatusMessage { get => statusMessage; private set => SetProperty(ref statusMessage, value); }
    public int Level { get => level; private set => SetProperty(ref level, value); }
    public int XP { get => xp; private set => SetProperty(ref xp, value); }
    public int Wins { get => wins; private set => SetProperty(ref wins, value); }
    public int Losses { get => losses; private set => SetProperty(ref losses, value); }
    public int BestStreak { get => bestStreak; private set => SetProperty(ref bestStreak, value); }
    public int CurrentStreak { get => currentStreak; private set => SetProperty(ref currentStreak, value); }
    public string Rank { get => rank; private set => SetProperty(ref rank, value); }
    public string Achievements { get => achievements; private set => SetProperty(ref achievements, value); }
    public double LevelProgress { get => levelProgress; private set => SetProperty(ref levelProgress, value); }
    public string LevelProgressText => $"{XP % 500} / 500 XP to next level";
    public bool IsBusy { get => isBusy; private set { if (SetProperty(ref isBusy, value)) ((Command)SaveCommand).ChangeCanExecute(); } }
    public IReadOnlyList<string> Difficulties { get; } = new[] { "Easy", "Medium", "Hard" };
    public ICommand SaveCommand { get; }

    public async Task LoadAsync()
    {
        try
        {
            var p = await data.LoadAsync();
            CaptainName = p.CaptainName;
            Difficulty = p.PreferredDifficulty;
            XP = p.Xp;
            Level = XP / 500 + 1;
            LevelProgress = (XP % 500) / 500.0;
            OnPropertyChanged(nameof(LevelProgressText));
            Rank = Level switch { >= 20 => "Fleet Admiral", >= 12 => "Admiral", >= 7 => "Commander", >= 3 => "Lieutenant", _ => "Recruit" };
            Wins = p.Wins; Losses = p.Losses;
            BestStreak = p.BestStreak; CurrentStreak = p.CurrentStreak;
            var medals = new List<string>();
            if (p.Wins >= 1) medals.Add("FIRST VICTORY");
            if (p.Battles.Any(b => b.Hits + b.Misses > 0 && b.Hits * 100.0 / (b.Hits + b.Misses) >= 80)) medals.Add("SHARPSHOOTER (80%+)");
            if (p.BestStreak >= 3) medals.Add("WIN STREAK (3)");
            if (p.Wins >= 10) medals.Add("VETERAN (10 WINS)");
            Achievements = medals.Count == 0 ? "No medals unlocked yet." : string.Join("  •  ", medals);
        }
        catch (Exception ex) { StatusMessage = $"Could not load profile: {ex.Message}"; }
    }

    private async Task SaveAsync()
    {
        if (IsBusy) return;
        var name = CaptainName?.Trim() ?? "";
        if (name.Length is < 2 or > 24) { StatusMessage = "Captain name must be 2–24 characters."; return; }
        if (!Difficulties.Contains(Difficulty)) { StatusMessage = "Select a valid difficulty."; return; }
        IsBusy = true;
        try
        {
            var p = await data.LoadAsync();
            p.CaptainName = name;
            p.PreferredDifficulty = Difficulty;
            await data.SaveAsync();
            CaptainName = name;
            StatusMessage = "Captain profile saved on this device.";
        }
        catch (Exception ex) { StatusMessage = $"Save failed: {ex.Message}"; }
        finally { IsBusy = false; }
    }
}
