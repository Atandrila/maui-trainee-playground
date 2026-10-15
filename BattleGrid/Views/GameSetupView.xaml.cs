namespace BattleGrid.Views;
public partial class GameSetupView : ContentPage
{
    public GameSetupView() => InitializeComponent();
    private async void OnBackClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("..");
}
