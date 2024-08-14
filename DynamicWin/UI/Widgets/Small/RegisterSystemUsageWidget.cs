using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

internal class RegisterSystemUsageWidget : IRegisterableWidget
{
    public bool IsSmallWidget => true;

    public string WidgetName => "System Usage Display";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter)
        => new SystemUsageWidget(parent, position, alignment);
}
