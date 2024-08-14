using DynamicWin.UI.UIElements.Custom;
using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

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
