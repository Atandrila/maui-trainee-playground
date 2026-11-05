using BattleGrid.Resources.Localization;
using BattleGrid.ViewModels;

namespace BattleGrid.Views;

public partial class ProfileView : ContentPage
{
    private readonly ProfileViewModel _viewModel;
    private readonly LanguageService _languageService;

    public ProfileView(ProfileViewModel viewModel, LanguageService languageService)
    {
        _viewModel = viewModel;
        _languageService = languageService;
        InitializeComponent();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _languageService.LanguageChanged += OnLanguageChanged;
        await _viewModel.LoadAsync();
    }

    protected override void OnDisappearing()
    {
        _languageService.LanguageChanged -= OnLanguageChanged;
        base.OnDisappearing();
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        // x:Static is evaluated when XAML is instantiated, not when Culture changes.
        // Recreate the Shell and return to Profile instead of running InitializeComponent twice.
        Dispatcher.Dispatch(() => _ = ReloadProfileAsync());
    }

    private async Task ReloadProfileAsync()
    {
        try
        {
            var window = Window;
            if (window is null)
                return;

            var shell = new AppShell();
            window.Page = shell;
            await shell.GoToAsync("//main/profile");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Unable to refresh language: {ex}");
        }
    }
}
