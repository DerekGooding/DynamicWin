using DynamicWin.UI.UIElements;
using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

internal class RegisterTimeWidget : IRegisterableWidget
{
    public bool IsSmallWidget => true;

    public string WidgetName => "Time Display";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter)
    {
        return new TimeWidget(parent, position, alignment);
    }
}

public class TimeWidget : SmallWidgetBase
{
    private readonly DWText timeText;

    public TimeWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
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

        timeText.Text = GetTime();
    }

    private string GetTime()
    {
        return DateTime.Now.ToString("HH:mm");
    }
}