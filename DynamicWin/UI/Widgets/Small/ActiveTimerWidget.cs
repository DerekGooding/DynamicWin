using DynamicWin.UI.UIElements;
using DynamicWin.UI.Widgets.Big;
using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

internal class RegisterActiveTimerWidget : IRegisterableWidget
{
    public bool IsSmallWidget => true;

    public string WidgetName => "Active Timer Display";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter)
    {
        return new ActiveTimerWidget(parent, position, alignment);
    }
}

public class ActiveTimerWidget : SmallWidgetBase
{
    private readonly DWText timeText;

    public ActiveTimerWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
    {
        timeText = new DWText(this, GetTime(), Vec2.zero, UIAlignment.Center)
        {
            TextSize = 14
        };
        AddLocalObject(timeText);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        timeText.SilentSetText(IsTimerActive() ? GetTime() : " ");
    }

    private bool IsTimerActive()
    {
        if (TimerWidget.Instance != null)
        {
            return TimerWidget.Instance.IsTimerRunning;
        }

        return false;
    }

    private string GetTime()
    {
        if (TimerWidget.Instance != null)
        {
            TimeSpan t = TimeSpan.FromSeconds(TimerWidget.Instance.CurrentTime);
            string formatedTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);

            return formatedTime;
        }

        return " ";
    }

    protected override float GetWidgetWidth()
    {
        return IsTimerActive() ? 60 : 0;
    }
}