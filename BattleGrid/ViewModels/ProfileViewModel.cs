using BattleGrid.Resources.Localization;
using BattleGrid.Services;
using BattleGrid.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BattleGrid.ViewModels;

public sealed class ProfileViewModel : ObservableObject
{
    private readonly PlayerDataService _playerDataService;
    private readonly LanguageService _languageService;
    private string _captainName = AppResources.Get("DefaultCaptainName");
    private string _statusKey = "StatusInitial";
    private string? _statusDetail;
    private int _level = 1, _xp, _wins, _losses, _bestStreak, _currentStreak;
    private string _rankKey = "RankRecruit";
    private readonly List<string> _achievementKeys = new();
    private double _levelProgress;
    private bool _isBusy;
    private int _selectedLanguageIndex;
    private int _selectedDifficultyIndex = 1;
    private bool _updatingOptions;
    private static readonly string[] _difficultyCodes = { "Easy", "Medium", "Hard" };

    public ProfileViewModel(PlayerDataService playerDataService, LanguageService languageService)
    {
        _playerDataService = playerDataService;
        _languageService = languageService;
        _selectedLanguageIndex = _languageService.LanguageCode == "fr" ? 1 : 0;
        RefreshDifficultyLabels();
        RefreshLanguageLabels();
        _languageService.LanguageChanged += (_, _) => RefreshLocalizedStrings();
        SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
    }

    public string CaptainName { get => _captainName; set => SetProperty(ref _captainName, value); }
    public int Level { get => _level; private set => SetProperty(ref _level, value); }
    public int XP { get => _xp; private set { if (SetProperty(ref _xp, value)) OnPropertyChanged(nameof(LevelProgressText)); } }
    public int Wins { get => _wins; private set => SetProperty(ref _wins, value); }
    public int Losses { get => _losses; private set => SetProperty(ref _losses, value); }
    public int BestStreak { get => _bestStreak; private set => SetProperty(ref _bestStreak, value); }
    public int CurrentStreak { get => _currentStreak; private set => SetProperty(ref _currentStreak, value); }
    public double LevelProgress { get => _levelProgress; private set => SetProperty(ref _levelProgress, value); }
    public string Rank => AppResources.Get(_rankKey);
    public string Achievements => _achievementKeys.Count == 0
        ? AppResources.Get("AchievementNone")
        : string.Join("  •  ", _achievementKeys.Select(AppResources.Get));
    public string LevelProgressText => string.Format(AppResources.Culture,
        AppResources.Get("LevelProgressFormat"), XP % 500);
    public string StatusMessage => _statusDetail is null
        ? AppResources.Get(_statusKey)
        : string.Format(AppResources.Culture, AppResources.Get(_statusKey), _statusDetail);
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value))
                ((Command)SaveCommand).ChangeCanExecute();
        }
    }

    public ObservableCollection<string> Languages { get; private set; } = new();
    public ObservableCollection<string> Difficulties { get; private set; } = new();
    public int SelectedDifficultyIndex
    {
        get => _selectedDifficultyIndex;
        set
        {
            if (_updatingOptions) return;
            SetProperty(ref _selectedDifficultyIndex, value);
        }
    }
    public int SelectedLanguageIndex
    {
        get => _selectedLanguageIndex;
        set
        {
            if (value < 0 || value > 1) return;
            if (SetProperty(ref _selectedLanguageIndex, value))
                _languageService.SetLanguage(value == 1 ? "fr" : "en");
        }
    }

    public ICommand SaveCommand { get; }

    private void SetStatus(string key, string? detail = null)
    {
        _statusKey = key;
        _statusDetail = detail;
        OnPropertyChanged(nameof(StatusMessage));
    }

    private void RefreshDifficultyLabels()
    {
        _updatingOptions = true;
        Difficulties = new ObservableCollection<string>(new[]
        {
            AppResources.Get("DifficultyEasy"),
            AppResources.Get("DifficultyMedium"),
            AppResources.Get("DifficultyHard")
        });
        OnPropertyChanged(nameof(Difficulties));
        _updatingOptions = false;
        OnPropertyChanged(nameof(SelectedDifficultyIndex));
    }

    private void RefreshLanguageLabels()
    {
        Languages = new ObservableCollection<string>(new[]
        {
            AppResources.Get("LanguageEnglish"),
            AppResources.Get("LanguageFrench")
        });
        OnPropertyChanged(nameof(Languages));
        OnPropertyChanged(nameof(SelectedLanguageIndex));
    }

    private void RefreshLocalizedStrings()
    {
        int index = _languageService.LanguageCode == "fr" ? 1 : 0;
        if (_selectedLanguageIndex != index)
        {
            _selectedLanguageIndex = index;
            OnPropertyChanged(nameof(SelectedLanguageIndex));
        }
        RefreshDifficultyLabels();
        RefreshLanguageLabels();
        OnPropertyChanged(nameof(Rank));
        OnPropertyChanged(nameof(Achievements));
        OnPropertyChanged(nameof(LevelProgressText));
        OnPropertyChanged(nameof(StatusMessage));
    }


    public async Task LoadAsync()
    {
        try
        {
            var p = await _playerDataService.LoadAsync();
            CaptainName = p.CaptainName;
            int difficultyIndex = Array.IndexOf(_difficultyCodes, p.PreferredDifficulty);
            SelectedDifficultyIndex = difficultyIndex < 0 ? 1 : difficultyIndex;
            XP = p.Xp;
            Level = XP / 500 + 1;
            LevelProgress = (XP % 500) / 500.0;
            _rankKey = Level switch
            {
                >= 20 => "RankFleetAdmiral",
                >= 12 => "RankAdmiral",
                >= 7 => "RankCommander",
                >= 3 => "RankLieutenant",
                _ => "RankRecruit"
            };
            OnPropertyChanged(nameof(Rank));
            Wins = p.Wins;
            Losses = p.Losses;
            BestStreak = p.BestStreak;
            CurrentStreak = p.CurrentStreak;
            _achievementKeys.Clear();
            if (p.Wins >= 1) _achievementKeys.Add("AchievementFirstVictory");
            if (p.Battles.Any(b => b.Hits + b.Misses > 0 && b.Hits * 100.0 / (b.Hits + b.Misses) >= 80))
                _achievementKeys.Add("AchievementSharpshooter");
            if (p.BestStreak >= 3) _achievementKeys.Add("AchievementWinStreak");
            if (p.Wins >= 10) _achievementKeys.Add("AchievementVeteran");
            OnPropertyChanged(nameof(Achievements));
        }
        catch (Exception ex)
        {
            SetStatus("StatusLoadError", ex.Message);
        }
    }

    private async Task SaveAsync()
    {
        if (IsBusy) return;
        var name = CaptainName?.Trim() ?? string.Empty;
        if (name.Length is < 2 or > 24)
        {
            SetStatus("StatusInvalidName");
            return;
        }
        if (SelectedDifficultyIndex < 0 || SelectedDifficultyIndex >= _difficultyCodes.Length)
        {
            SetStatus("StatusInvalidDifficulty");
            return;
        }
        IsBusy = true;
        try
        {
            var p = await _playerDataService.LoadAsync();
            p.CaptainName = name;
            // Store stable language-independent codes in the existing JSON schema.
            p.PreferredDifficulty = _difficultyCodes[SelectedDifficultyIndex];
            await _playerDataService.SaveAsync();
            CaptainName = name;
            SetStatus("StatusSaved");
        }
        catch (Exception ex)
        {
            SetStatus("StatusSaveError", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
