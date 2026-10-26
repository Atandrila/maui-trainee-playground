using System.Globalization;

namespace BattleGrid.Resources.Localization;

public sealed class LanguageService
{
    private const string LanguagePreferenceKey = "battlegrid_profile_language";
    private string languageCode;

    public event EventHandler? LanguageChanged;

    public LanguageService()
    {
        string saved = Preferences.Default.Get(LanguagePreferenceKey, "en");
        languageCode = saved == "fr" ? "fr" : "en";
        AppResources.Culture = CultureInfo.GetCultureInfo(languageCode);
    }

    public string LanguageCode => languageCode;

    public void SetLanguage(string code)
    {
        if (code != "en" && code != "fr")
            throw new ArgumentException("Language must be en or fr.", nameof(code));
        if (languageCode == code) return;

        languageCode = code;
        AppResources.Culture = CultureInfo.GetCultureInfo(code);
        Preferences.Default.Set(LanguagePreferenceKey, code);
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}
