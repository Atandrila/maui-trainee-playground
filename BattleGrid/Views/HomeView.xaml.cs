using BattleGrid.Controls;

namespace BattleGrid.Views;

public partial class HomeView : ContentPage
{
    public HomeView()
    {
        InitializeComponent();
        RadarPreview.Drawable = new NavalRadarDrawable();
    }

    private async void OnDeployClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(GameSetupView));
    }
}
