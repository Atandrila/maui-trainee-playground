namespace MauiApp1.Pages;

public partial class MainPage : ContentPage
{
    private int _count;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCountClicked(object sender, EventArgs e)
    {
        _count++;
        CountLabel.Text = _count.ToString();
        CountButton.Text = _count == 1 ? "Tapped 1 time" : $"Tapped {_count} times";

        SemanticScreenReader.Announce($"Count is now {_count}");
    }
}
