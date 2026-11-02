using System.Globalization;

namespace BattleGrid.Resources.Localization;

public sealed class LanguageService
{
    private const string _languagePreferenceKey = "battlegrid_profile_language";
    private string _languageCode;

    public event EventHandler? LanguageChanged;

    public LanguageService()
    {
        string saved = Preferences.Default.Get(_languagePreferenceKey, "en");
        _languageCode = saved == "fr" ? "fr" : "en";
        AppResources.Culture = CultureInfo.GetCultureInfo(_languageCode);
    }

    public string LanguageCode => _languageCode;

    public void SetLanguage(string code)
    {
        if (code != "en" && code != "fr")
            throw new ArgumentException("Language must be en or fr.", nameof(code));
        if (_languageCode == code) return;

        _languageCode = code;
        AppResources.Culture = CultureInfo.GetCultureInfo(code);
        Preferences.Default.Set(_languagePreferenceKey, code);
        LocalizedStrings.Instance.Refresh();
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}
