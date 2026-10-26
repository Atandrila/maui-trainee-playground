using System.Globalization;
using System.Resources;

namespace BattleGrid.Resources.Localization;

public static class AppResources
{
    private static readonly ResourceManager Manager =
        new("BattleGrid.Resources.Localization.AppResources", typeof(AppResources).Assembly);

    public static CultureInfo Culture { get; set; } = CultureInfo.GetCultureInfo("en");

    public static string Get(string key) =>
        Manager.GetString(key, Culture) ?? key;
}
