using Microsoft.Extensions.Logging;
using BattleGrid.Services;
using BattleGrid.Resources.Localization;
using BattleGrid.ViewModels;
using BattleGrid.Views;

namespace BattleGrid;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<PlayerDataService>();
        builder.Services.AddSingleton<LanguageService>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<ProfileView>();
        builder.Services.AddTransient<DashboardView>();

        return builder.Build();
    }
}
