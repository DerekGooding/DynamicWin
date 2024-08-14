using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

internal class RegisterActiveTimerWidget : IRegisterableWidget
{
    public bool IsSmallWidget => true;

    public string WidgetName => "Active Timer Display";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) 
        => new ActiveTimerWidget(parent, position, alignment);
}
