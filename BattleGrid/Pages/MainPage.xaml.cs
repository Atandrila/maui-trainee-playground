using BattleGrid.Models;
using BattleGrid.PageModels;

namespace BattleGrid.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}