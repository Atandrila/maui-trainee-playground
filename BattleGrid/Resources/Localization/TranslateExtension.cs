using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BattleGrid.Resources.Localization;
[ContentProperty(nameof(Key))]
public sealed class TranslateExtension : IMarkupExtension
{
    public string Key { get; set; } = string.Empty;

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrWhiteSpace(Key))
            throw new ArgumentException("A localization resource key is required.", nameof(Key));

        return new Binding($"[{Key}]", source: LocalizedStrings.Instance, mode: BindingMode.OneWay);
    }
}
