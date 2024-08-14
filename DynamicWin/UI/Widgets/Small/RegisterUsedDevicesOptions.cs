using DynamicWin.UI.Menu.Menus;
using DynamicWin.UI.UIElements;
using DynamicWin.UI.UIElements.Custom;
using DynamicWin.Utils;
using Newtonsoft.Json;

namespace DynamicWin.UI.Widgets.Small;

class RegisterUsedDevicesOptions : IRegisterableSetting
{
    public string SettingTitle => "Used Devices Widget";

    public string SettingID => "useddevicewidget";

    public static UsedDevicesOptionsSave saveData;

    public struct UsedDevicesOptionsSave
    {
        public bool enableIndicator;
        public float indicatorThreshold;
    }

    public void LoadSettings() => saveData = SaveManager.Contains(SettingID)
            ? JsonConvert.DeserializeObject<UsedDevicesOptionsSave>((string)SaveManager.Get(SettingID))
            : new UsedDevicesOptionsSave() { indicatorThreshold = 0.5f };

    public void SaveSettings() => SaveManager.Add(SettingID, JsonConvert.SerializeObject(saveData));

    public List<UIObject> SettingsObjects()
    {
        var objects = new List<UIObject>();

        var loudnessMeter = new LoudnessMeter(null, new Vec2(0, 0), new Vec2(400, 7.5f));

        var thresholdSlider = new DWSlider(null, new Vec2(0, 0), new Vec2(400, 25))
        {
            value = Mathf.Clamp(saveData.indicatorThreshold, 0.05f, 1f),
            clickCallback = (x) => saveData.indicatorThreshold = x
        };

        var enableIndicatorCheckbox = new Checkbox(null, "Show Microphone Indicator", new Vec2(25, 0), new Vec2(25, 25), null, alignment: UIAlignment.TopLeft)
        {
            IsChecked = saveData.enableIndicator
        };

        var thresholdText = new DWText(null, "Threshold", new Vec2(25, 0), UIAlignment.TopLeft);

        enableIndicatorCheckbox.clickCallback = () =>
        {
            saveData.enableIndicator = enableIndicatorCheckbox.IsChecked;
            thresholdSlider.SetActive(saveData.enableIndicator);
            loudnessMeter.SetActive(saveData.enableIndicator);
            thresholdText.SetActive(saveData.enableIndicator);
        };

        thresholdSlider.SilentSetActive(saveData.enableIndicator);
        loudnessMeter.SilentSetActive(saveData.enableIndicator);
        thresholdText.SilentSetActive(saveData.enableIndicator);

        objects.Add(enableIndicatorCheckbox);
        objects.Add(thresholdText);
        objects.Add(loudnessMeter);
        objects.Add(thresholdSlider);

        return objects;
    }
}
