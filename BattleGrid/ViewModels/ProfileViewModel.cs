using BattleGrid.Resources.Localization;
using BattleGrid.Services;
using BattleGrid.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BattleGrid.ViewModels;

public sealed class ProfileViewModel : ObservableObject
{
    private readonly PlayerDataService data;
    private readonly LanguageService language;
    private string captainName = AppResources.Get("DefaultCaptainName");
    private string statusKey = "StatusInitial";
    private string? statusDetail;
    private int level = 1, xp, wins, losses, bestStreak, currentStreak;
    private string rankKey = "RankRecruit";
    private readonly List<string> achievementKeys = new();
    private double levelProgress;
    private bool isBusy;
    private int selectedLanguageIndex;
    private int selectedDifficultyIndex = 1;
    private bool updatingOptions;
    private static readonly string[] DifficultyCodes = { "Easy", "Medium", "Hard" };

    public ProfileViewModel(PlayerDataService data, LanguageService language)
    {
        this.data = data;
        this.language = language;
        selectedLanguageIndex = language.LanguageCode == "fr" ? 1 : 0;
        RefreshDifficultyLabels();
        RefreshLanguageLabels();
        language.LanguageChanged += (_, _) => RefreshLocalizedStrings();
        SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
    }

    public string CaptainName { get => captainName; set => SetProperty(ref captainName, value); }
    public int Level { get => level; private set => SetProperty(ref level, value); }
    public int XP { get => xp; private set { if (SetProperty(ref xp, value)) OnPropertyChanged(nameof(LevelProgressText)); } }
    public int Wins { get => wins; private set => SetProperty(ref wins, value); }
    public int Losses { get => losses; private set => SetProperty(ref losses, value); }
    public int BestStreak { get => bestStreak; private set => SetProperty(ref bestStreak, value); }
    public int CurrentStreak { get => currentStreak; private set => SetProperty(ref currentStreak, value); }
    public double LevelProgress { get => levelProgress; private set => SetProperty(ref levelProgress, value); }
    public string Rank => AppResources.Get(rankKey);
    public string Achievements => achievementKeys.Count == 0
        ? AppResources.Get("AchievementNone")
        : string.Join("  •  ", achievementKeys.Select(AppResources.Get));
    public string LevelProgressText => string.Format(AppResources.Culture,
        AppResources.Get("LevelProgressFormat"), XP % 500);
    public string StatusMessage => statusDetail is null
        ? AppResources.Get(statusKey)
        : string.Format(AppResources.Culture, AppResources.Get(statusKey), statusDetail);
    public bool IsBusy
    {
        get => isBusy;
        private set
        {
            if (SetProperty(ref isBusy, value))
                ((Command)SaveCommand).ChangeCanExecute();
        }
    }

    public ObservableCollection<string> Languages { get; private set; } = new();
    public ObservableCollection<string> Difficulties { get; private set; } = new();
    public int SelectedDifficultyIndex
    {
        get => selectedDifficultyIndex;
        set
        {
            if (updatingOptions) return;
            SetProperty(ref selectedDifficultyIndex, value);
        }
    }
    public int SelectedLanguageIndex
    {
        get => selectedLanguageIndex;
        set
        {
            if (value < 0 || value > 1) return;
            if (SetProperty(ref selectedLanguageIndex, value))
                language.SetLanguage(value == 1 ? "fr" : "en");
        }
    }

    public ICommand SaveCommand { get; }
    public string ProfileTitleText => AppResources.Get("ProfileTitle");
    public string PersonnelSectionText => AppResources.Get("PersonnelSection");
    public string CaptainDossierText => AppResources.Get("CaptainDossier");
    public string EmblemNavyText => AppResources.Get("EmblemNavy");
    public string FleetCommanderText => AppResources.Get("FleetCommander");
    public string ServiceLevelText => AppResources.Get("ServiceLevel");
    public string ServiceRecordText => AppResources.Get("ServiceRecord");
    public string VictoriesText => AppResources.Get("Victories");
    public string LossesText => AppResources.Get("Losses");
    public string CurrentStreakText => AppResources.Get("CurrentStreak");
    public string BestStreakText => AppResources.Get("BestStreak");
    public string CaptainSettingsText => AppResources.Get("CaptainSettings");
    public string CaptainNameLabelText => AppResources.Get("CaptainNameLabel");
    public string CaptainNamePlaceholderText => AppResources.Get("CaptainNamePlaceholder");
    public string PreferredDifficultyText => AppResources.Get("PreferredDifficulty");
    public string SaveCaptainProfileText => AppResources.Get("SaveCaptainProfile");
    public string MedalsAchievementsText => AppResources.Get("MedalsAchievements");
    public string MedalsHintText => AppResources.Get("MedalsHint");
    public string LanguageLabelText => AppResources.Get("LanguageLabel");
    public string LanguageHintText => AppResources.Get("LanguageHint");

    private void SetStatus(string key, string? detail = null)
    {
        statusKey = key;
        statusDetail = detail;
        OnPropertyChanged(nameof(StatusMessage));
    }

    private void RefreshDifficultyLabels()
    {
        updatingOptions = true;
        Difficulties = new ObservableCollection<string>(new[]
        {
            AppResources.Get("DifficultyEasy"),
            AppResources.Get("DifficultyMedium"),
            AppResources.Get("DifficultyHard")
        });
        OnPropertyChanged(nameof(Difficulties));
        updatingOptions = false;
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
        int index = language.LanguageCode == "fr" ? 1 : 0;
        if (selectedLanguageIndex != index)
        {
            selectedLanguageIndex = index;
            OnPropertyChanged(nameof(SelectedLanguageIndex));
        }
        RefreshDifficultyLabels();
        RefreshLanguageLabels();
        OnPropertyChanged(nameof(Rank));
        OnPropertyChanged(nameof(Achievements));
        OnPropertyChanged(nameof(LevelProgressText));
        OnPropertyChanged(nameof(StatusMessage));
        foreach (var property in LocalizedPropertyNames)
            OnPropertyChanged(property);
    }

    private static readonly string[] LocalizedPropertyNames =
    {
        nameof(ProfileTitleText),
        nameof(PersonnelSectionText),
        nameof(CaptainDossierText),
        nameof(EmblemNavyText),
        nameof(FleetCommanderText),
        nameof(ServiceLevelText),
        nameof(ServiceRecordText),
        nameof(VictoriesText),
        nameof(LossesText),
        nameof(CurrentStreakText),
        nameof(BestStreakText),
        nameof(CaptainSettingsText),
        nameof(CaptainNameLabelText),
        nameof(CaptainNamePlaceholderText),
        nameof(PreferredDifficultyText),
        nameof(SaveCaptainProfileText),
        nameof(MedalsAchievementsText),
        nameof(MedalsHintText),
        nameof(LanguageLabelText),
        nameof(LanguageHintText)
    };

    public async Task LoadAsync()
    {
        try
        {
            var p = await data.LoadAsync();
            CaptainName = p.CaptainName;
            int difficultyIndex = Array.IndexOf(DifficultyCodes, p.PreferredDifficulty);
            SelectedDifficultyIndex = difficultyIndex < 0 ? 1 : difficultyIndex;
            XP = p.Xp;
            Level = XP / 500 + 1;
            LevelProgress = (XP % 500) / 500.0;
            rankKey = Level switch
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
            achievementKeys.Clear();
            if (p.Wins >= 1) achievementKeys.Add("AchievementFirstVictory");
            if (p.Battles.Any(b => b.Hits + b.Misses > 0 && b.Hits * 100.0 / (b.Hits + b.Misses) >= 80))
                achievementKeys.Add("AchievementSharpshooter");
            if (p.BestStreak >= 3) achievementKeys.Add("AchievementWinStreak");
            if (p.Wins >= 10) achievementKeys.Add("AchievementVeteran");
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
        if (SelectedDifficultyIndex < 0 || SelectedDifficultyIndex >= DifficultyCodes.Length)
        {
            SetStatus("StatusInvalidDifficulty");
            return;
        }
        IsBusy = true;
        try
        {
            var p = await data.LoadAsync();
            p.CaptainName = name;
            // Store stable language-independent codes in the existing JSON schema.
            p.PreferredDifficulty = DifficultyCodes[SelectedDifficultyIndex];
            await data.SaveAsync();
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
