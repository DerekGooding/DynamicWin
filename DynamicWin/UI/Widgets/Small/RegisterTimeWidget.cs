using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

internal class RegisterTimeWidget : IRegisterableWidget
{
    public bool IsSmallWidget => true;

    public string WidgetName => "Time Display";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) => new TimeWidget(parent, position, alignment);
}
