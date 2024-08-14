using DynamicWin.UI.UIElements;
using DynamicWin.UI.Widgets.Big;
using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

public class ActiveTimerWidget : SmallWidgetBase
{
    private readonly DWText timeText;

    public ActiveTimerWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
    {
        timeText = new DWText(this, Time, Vec2.zero, UIAlignment.Center)
        {
            TextSize = 14
        };
        AddLocalObject(timeText);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        timeText.SilentSetText(IsTimerActive ? Time : " ");
    }

    private static bool IsTimerActive => TimerWidget.Instance?.IsTimerRunning == true;

    private static string Time
    {
        get
        {
            if (TimerWidget.Instance != null)
            {
                TimeSpan t = TimeSpan.FromSeconds(TimerWidget.Instance.CurrentTime);
                return string.Format("{0:D2}:{1:D2}:{2:D2}",
                                t.Hours,
                                t.Minutes,
                                t.Seconds);
            }

            return " ";
        }
    }

    protected override float GetWidgetWidth() => IsTimerActive ? 60 : 0;
}