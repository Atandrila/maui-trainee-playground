
using BattleGrid.Views;

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
        // Built-in MAUI dialog, with no third-party toast dependency.
        await Shell.Current.DisplayAlert("BattleGrid", message, "OK");
    }
}
