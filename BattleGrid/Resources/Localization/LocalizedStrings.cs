using System.ComponentModel;

namespace BattleGrid.Resources.Localization;

/// <summary>Observable resource lookup for XAML bindings; one shared source for all labels.</summary>
public sealed class LocalizedStrings : INotifyPropertyChanged
{
    public static LocalizedStrings Instance { get; } = new();

    private LocalizedStrings() { }

    public string this[string key] => AppResources.Get(key);

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Refresh()
    {
        // A binding to [SomeKey] observes the standard indexer property name.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }
}
