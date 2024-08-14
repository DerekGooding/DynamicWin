using DynamicWin.Main;
using DynamicWin.UI.UIElements;
using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

public class TimeWidget : SmallWidgetBase
{
    private readonly DWText timeText;

    public TimeWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
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

        timeText.Text = Time;
    }

    private static string Time => Settings.MilitaryTime ? DateTime.Now.ToString("HH:mm") : DateTime.Now.ToString("hh:mm tt");
}