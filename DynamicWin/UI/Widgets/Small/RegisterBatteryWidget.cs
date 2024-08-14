using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

internal class RegisterBatteryWidget : IRegisterableWidget
{
    public bool IsSmallWidget => true;
    public string WidgetName => "Battery Display";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter)
        => new BatteryWidget(parent, position, alignment);
}
