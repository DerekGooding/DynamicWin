using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

class RegisterUsedDevicesWidget : IRegisterableWidget
{
    public bool IsSmallWidget => true;
    public string WidgetName => "Used Devices";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) 
        => new UsedDevicesWidget(parent, position, alignment);
}
