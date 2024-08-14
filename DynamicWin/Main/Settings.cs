using DynamicWin.UI.UIElements;
using DynamicWin.Utils;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace DynamicWin.Main;

public static class Settings
{
    //private static readonly bool useCustomTheme;
    //private static ThemeHolder customTheme;

    public static IslandObject.IslandMode IslandMode { get; set; }
    public static bool AllowBlur { get; set; }
    public static bool AllowAnimation { get; set; }
    public static bool AntiAliasing { get; set; }
    public static bool MilitaryTime { get; set; }

    public static int Theme
    {
        get => theme;
        set
        {
            if(theme == value) return;
            theme = value;
            Utils.Theme.UpdateTheme();
        }
    }

    public static List<string> smallWidgetsLeft;
    public static List<string> smallWidgetsRight;
    public static List<string> smallWidgetsMiddle;
    public static List<string> bigWidgets;
    private static int theme;

    //public static ThemeHolder CustomTheme { get => customTheme; set => customTheme = value; }

    public static void InitializeSettings()
    {
        try
        {
            if (SaveManager.Contains("settings"))
            {
                IslandMode = ((long?)SaveManager.Get("settings.islandmode") == 0) ? IslandObject.IslandMode.Island : IslandObject.IslandMode.Notch;

                AllowBlur = (bool?)SaveManager.Get("settings.allowblur") ?? false;
                AllowAnimation = (bool?)SaveManager.Get("settings.allowanimtion") ?? false;
                AntiAliasing = (bool?)SaveManager.Get("settings.antialiasing") ?? false;
                MilitaryTime = (bool?)SaveManager.Get("settings.militarytime") ?? true;

                Theme = (int?)(long?)SaveManager.Get("settings.theme") ?? 0;

                Settings.smallWidgetsLeft = [];
                Settings.smallWidgetsRight = [];
                Settings.smallWidgetsMiddle = [];
                Settings.bigWidgets = [];

                JArray smallWidgetsLeft = (JArray?)SaveManager.Get("settings.smallwidgetsleft") ?? [];
                JArray smallWidgetsRight = (JArray?)SaveManager.Get("settings.smallwidgetsright") ?? [];
                JArray smallWidgetsMiddle = (JArray?)SaveManager.Get("settings.smallwidgetsmiddle") ?? [];
                JArray bigWidgets = (JArray?)SaveManager.Get("settings.bigwidgets") ?? [];

                foreach (JToken x in smallWidgetsLeft)
                    Settings.smallWidgetsLeft.Add(x.ToString());
                foreach (JToken x in smallWidgetsRight)
                    Settings.smallWidgetsRight.Add(x.ToString());
                foreach (JToken x in smallWidgetsMiddle)
                    Settings.smallWidgetsMiddle.Add(x.ToString());
                foreach (JToken x in bigWidgets)
                    Settings.bigWidgets.Add(x.ToString());
            }
            else
            {
                smallWidgetsLeft = [];
                smallWidgetsRight = [];
                smallWidgetsMiddle = [];
                bigWidgets = [];

                smallWidgetsRight.Add("DynamicWin.UI.Widgets.Small.RegisterUsedDevicesWidget");
                smallWidgetsLeft.Add("DynamicWin.UI.Widgets.Small.RegisterTimeWidget");
                bigWidgets.Add("DynamicWin.UI.Widgets.Big.RegisterMediaWidget");

                IslandMode = IslandObject.IslandMode.Island;
                AllowBlur = true;
                AllowAnimation = true;
                AntiAliasing = true;

                Theme = 0;

                SaveManager.SaveData.Add("settings", 1);
            }

            // This must be run after loading all settings
            //AfterSettingsLoaded();
        }
        catch
        {
            MessageBox.Show("An error occurred trying to load the settings. Please revert back to the default settings by deleting the \"Settings.json\" file located under \"%appdata%/DynamicWin/\".");
        }
    }

    //private static void AfterSettingsLoaded() => Utils.Theme.UpdateTheme();

    public static void Save()
    {
        SaveManager.Add("settings.islandmode", (IslandMode == IslandObject.IslandMode.Island) ? 0 : 1);

        SaveManager.Add("settings.allowblur", AllowBlur);
        SaveManager.Add("settings.allowanimtion", AllowAnimation);
        SaveManager.Add("settings.antialiasing", AntiAliasing);
        SaveManager.Add("settings.militarytime", MilitaryTime);

        SaveManager.Add("settings.theme", Theme);

        SaveManager.Add("settings.smallwidgetsleft", smallWidgetsLeft);
        SaveManager.Add("settings.smallwidgetsright", smallWidgetsRight);
        SaveManager.Add("settings.smallwidgetsmiddle", smallWidgetsMiddle);
        SaveManager.Add("settings.bigwidgets", bigWidgets);

        SaveManager.SaveAll();
    }
}