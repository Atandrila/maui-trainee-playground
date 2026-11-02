using BattleGrid.ViewModels;
namespace BattleGrid.Views;
public partial class ProfileView : ContentPage
{
    private readonly ProfileViewModel _viewModel;
    public ProfileView(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
