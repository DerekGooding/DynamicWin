using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Big;

internal class RegisterTimerWidget : IRegisterableWidget
{
    public bool IsSmallWidget => false;
    public string WidgetName => "Timer";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter)
        => new TimerWidget(parent, position, alignment);
}
