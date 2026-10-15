using BattleGrid.ViewModels;
namespace BattleGrid.Views;
public partial class DashboardView : ContentPage
{
    private readonly DashboardViewModel viewModel;
    public DashboardView(DashboardViewModel viewModel)
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
