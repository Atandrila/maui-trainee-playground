
using BattleGrid.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace BattleGrid;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(GameSetupView), typeof(GameSetupView));
    }

    public static async Task DisplayToastAsync(string message)
    {
        var toast = Toast.Make(
            message,
            ToastDuration.Short);

        await toast.Show();
    }
}
