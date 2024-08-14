using DynamicWin.Main;
using Newtonsoft.Json;
using System.IO;

namespace DynamicWin.Utils;

public static class Theme
{
    private static ThemeHolder darkTheme = new()
    {
        IslandColor = "#000000",
        TextMain = "#ffffff",
        TextSecond = "#a6a6a6",
        TextThird = "#595959",
        Primary = "#6988b7",
        Secondary = "#061122",
        Success = "#bad844",
        Error = "#d84444",
        IconColor = "#ffffff",
        WidgetBackground = "#11ffffff"
    };

    private static ThemeHolder lightTheme = new()
    {
        IslandColor = "#ffffff",
        TextMain = "#000000",
        TextSecond = "#333333",
        TextThird = "#666666",
        Primary = "#7a9fd6",
        Secondary = "#c1d7f7",
        Success = "#99d844",
        Error = "#ff6666",
        IconColor = "#000000",
        WidgetBackground = "#11000000"
    };

    private static ThemeHolder candyTheme = GetTheme("""
        {
          "IslandColor": "#f7cac9",
          "TextMain": "#ff6f61",
          "TextSecond": "#d66853",
          "TextThird": "#b94a45",
          "Primary": "#ff6f61",
          "Secondary": "#f7cac9",
          "Success": "#88b04b",
          "Error": "#c0392b",
          "IconColor": "#ff6f61",
          "WidgetBackground": "#88ffebee"
        }
        """);
    private static ThemeHolder forestDawnTheme = GetTheme("""
        {
          "IslandColor": "#1c1c1c",
          "TextMain": "#8e44ad",
          "TextSecond": "#9b59b6",
          "TextThird": "#6c3483",
          "Primary": "#8e44ad",
          "Secondary": "#34495e",
          "Success": "#27ae60",
          "Error": "#e74c3c",
          "IconColor": "#8e44ad",
          "WidgetBackground": "#2c3e50"
        }
        """);
    private static ThemeHolder sunsetGlow = GetTheme("""
        {
          "IslandColor": "#2c3e50",
          "TextMain": "#f39c12",
          "TextSecond": "#e67e22",
          "TextThird": "#d35400",
          "Primary": "#e74c3c",
          "Secondary": "#c0392b",
          "Success": "#27ae60",
          "Error": "#c0392b",
          "IconColor": "#f39c12",
          "WidgetBackground": "#22ecf0f1"
        }
        """);

    public struct ThemeHolder
    {
        public string TextMain;
        public string TextSecond;
        public string TextThird;
        public string Primary;
        public string Secondary;
        public string IslandColor;
        public string Success;
        public string Error;
        public string IconColor;
        public string WidgetBackground;
    }

    public static void ApplyTheme(ThemeHolder theme)
    {
        TextMain = GetColor(theme.TextMain);
        TextSecond = GetColor(theme.TextSecond);
        TextThird = GetColor(theme.TextThird);
        Primary = GetColor(theme.Primary);
        Secondary = GetColor(theme.Secondary);
        IslandBackground = GetColor(theme.IslandColor);
        Success = GetColor(theme.Success);
        Error = GetColor(theme.Error);
        IconColor = GetColor(theme.IconColor);
        WidgetBackground = GetColor(theme.WidgetBackground);
    }

    public static Col GetColor(string hex) => Col.FromHex(hex);

    public static void UpdateTheme()
    {
        switch (Settings.Theme)
        {
            case 0:
                ApplyTheme(darkTheme);
                return;

            case 1:
                ApplyTheme(lightTheme);
                return;

            case 2:
                ApplyTheme(candyTheme);
                return;

            case 3:
                ApplyTheme(forestDawnTheme);
                return;

            case 4:
                ApplyTheme(sunsetGlow);
                return;
        }

        ThemeHolder customTheme = new();

        try
        {
            const string defaultTheme = "{\r\n  \"IslandColor\": \"#000000\",\r\n  \"TextMain\": \"#dd11dd\",\r\n  \"TextSecond\": \"#aa11aa\",\r\n  \"TextThird\": \"#661166\",\r\n  \"Primary\": \"#dd11dd\",\r\n  \"Secondary\": \"#111111\",\r\n  \"Success\": \"#991199\",\r\n  \"Error\": \"#3311933\",\r\n  \"IconColor\": \"#dd11dd\",\r\n  \"WidgetBackground\": \"#11ffffff\"\r\n}";

            var directory = SaveManager.SavePath;
            const string fileName = "Theme.json";

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(directory, fileName);
            if (!File.Exists(fullPath))
            {
                var fs = File.Create(fullPath);
                fs.Close();
                File.WriteAllText(fullPath, defaultTheme);
            }

            var json = File.ReadAllText(fullPath);

            if (string.IsNullOrEmpty(json))
            {
                File.WriteAllText(fullPath, defaultTheme);
                json = defaultTheme;
            }

            System.Diagnostics.Debug.WriteLine($"Loaded theme: {json}");

            ApplyTheme(GetTheme(json));
        }
        catch
        {
            System.Diagnostics.Debug.WriteLine("Couldn't load custom theme.");
            ApplyTheme(darkTheme);
        }
    }

    private static ThemeHolder GetTheme(string json) => JsonConvert.DeserializeObject<ThemeHolder>(json);

    public static Col TextMain { get; set; }
    public static Col TextSecond { get; set; }
    public static Col TextThird { get; set; }
    public static Col Primary { get; set; }
    public static Col Secondary { get; set; }
    public static Col IslandBackground { get; set; }
    public static Col Success { get; set; }
    public static Col Error { get; set; }
    public static Col IconColor { get; set; }
    public static Col WidgetBackground { get; set; }
}