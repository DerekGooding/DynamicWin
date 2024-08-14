using DynamicWin.UI.UIElements;
using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

public class SystemUsageWidget : SmallWidgetBase
{
    private readonly DWText text;

    public SystemUsageWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
    {
        text = new DWText(this, Usage, Vec2.zero, UIAlignment.Center)
        {
            TextSize = 12,
            Color = Theme.TextSecond
        };
        AddLocalObject(text);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        updateCycle += deltaTime;

        if (updateCycle > 1.5f)
        {
            text.SilentSetText(Usage);
            updateCycle = 0f;
        }
    }

    private float updateCycle;

    private static string Usage => HardwareMonitor.usageString;

    protected override float GetWidgetWidth() => Math.Max(225f, text != null ? text.TextBounds.X : 10 - 10);
}