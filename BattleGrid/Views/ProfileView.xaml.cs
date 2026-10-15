using BattleGrid.ViewModels;
namespace BattleGrid.Views;
public partial class ProfileView : ContentPage
{
    private readonly ProfileViewModel viewModel;
    public ProfileView(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = this.viewModel = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.LoadAsync();
    }
}
