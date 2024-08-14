using DynamicWin.UI.Menu.Menus;
using DynamicWin.UI.UIElements;
using DynamicWin.UI.UIElements.Custom;
using DynamicWin.Utils;
using Newtonsoft.Json;
using SkiaSharp;

namespace DynamicWin.UI.Widgets.Small;

class RegisterUsedDevicesWidget : IRegisterableWidget
{
    public bool IsSmallWidget => true;
    public string WidgetName => "Used Devices";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) 
        => new UsedDevicesWidget(parent, position, alignment);
}

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

internal class LoudnessMeter : DWProgressBar
{
    public LoudnessMeter(UIObject? parent, Vec2 position, Vec2 size, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, size, alignment)
    {
        vaueSmoothing = 25f;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        value = GetMicrophoneLoudness();

        contentColor = GetColor(
            (value > 0.85f) ? new Col(1, 0, 0) : (value > 0.65f) ? new Col(1, 1, 0) : new Col(0, 1, 0)
            ) * (RegisterUsedDevicesOptions.saveData.indicatorThreshold < value ? 1f : 0.45f);

        Color = contentColor * 0.25f;
    }

    public static float GetMicrophoneLoudness()
        => DynamicWinMain.DefaultMicrophone == null ? 0.0f : (float)Math.Sqrt(DynamicWinMain.DefaultMicrophone.AudioMeterInformation.MasterPeakValue + 0.001);
}

public class UsedDevicesWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : SmallWidgetBase(parent, position, alignment)
{
    readonly float camDotSize = 2.5f;
    float camDotSizeCurrent;
    float camDotPositionX;

    readonly float micDotSize = 2.5f;
    float micDotSizeCurrent;
    float micDotPositionX;

    readonly float separation = 6.5f;

    protected override float GetWidgetWidth() => (camDotSizeCurrent * 4) + (micDotSizeCurrent * 4);

    float sinCycleCamera = 1f;
    float sinCycleMicrophone;

    readonly float sinSpeed = 2.75f;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        sinCycleCamera += sinSpeed * deltaTime;
        sinCycleMicrophone += sinSpeed * deltaTime;

        const bool isCamActive = false; //DeviceUsageChecker.IsWebcamInUse();
        const bool isMicActive = false; //DeviceUsageChecker.IsMicrophoneInUse();

        camDotSizeCurrent = Mathf.Lerp(camDotSizeCurrent, isCamActive ? camDotSize : 0f, 5f * deltaTime);
        micDotSizeCurrent = Mathf.Lerp(micDotSizeCurrent, isMicActive ? micDotSize : 0f, 5f * deltaTime);

        camDotPositionX = Mathf.Lerp(camDotPositionX, 0, 5f * deltaTime);
        micDotPositionX = Mathf.Lerp(micDotPositionX, 0, 5f * deltaTime);

        isMicrophoneIndicatorShowing = LoudnessMeter.GetMicrophoneLoudness() > RegisterUsedDevicesOptions.saveData.indicatorThreshold;
    }

    bool isMicrophoneIndicatorShowing = false;

    public override void DrawWidget(SKCanvas canvas)
    {
        var paint = GetPaint();

        var camPos = GetScreenPosFromRawPosition(new Vec2(camDotPositionX, 0), new Vec2(0, camDotSizeCurrent / 2), UIAlignment.Center, this);

        paint.Color = Col.Lerp(Theme.Error, Theme.Error * 0.6f, Mathf.Remap((float)Math.Sin(sinCycleCamera), -1, 1, 0, 1)).Value();
        canvas.DrawCircle(camPos.X, camPos.Y, camDotSizeCurrent, paint);

        var micPos = GetScreenPosFromRawPosition(new Vec2(micDotPositionX, 0), new Vec2(0, micDotSizeCurrent / 2), UIAlignment.Center, this);

        paint.Color = Col.Lerp(Theme.Success, Theme.Success * 0.6f, Mathf.Remap((float)Math.Sin(sinCycleMicrophone), -1, 1, 0, 1)).Value();
        canvas.DrawCircle(micPos.X, micPos.Y, micDotSizeCurrent, paint);

        if (isMicrophoneIndicatorShowing && RegisterUsedDevicesOptions.saveData.enableIndicator)
        {
            paint.IsStroke = true;
            paint.StrokeWidth = 1.25f;

            var micIndicatorPos = GetScreenPosFromRawPosition(new Vec2(micDotPositionX, 0), new Vec2(0, micDotSizeCurrent / 2), UIAlignment.Center, this);
            canvas.DrawCircle(micIndicatorPos.X, micIndicatorPos.Y, micDotSizeCurrent + 3f, paint);
        }
    }
}