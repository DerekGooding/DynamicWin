using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Big;

internal class RegisterShortcutsWidget : IRegisterableWidget
{
    public bool IsSmallWidget => false;
    public string WidgetName => "Shortcuts";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) => new ShortcutsWidget(parent, position, alignment);
}
