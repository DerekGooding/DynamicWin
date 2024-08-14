using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Big;

class RegisterWeatherWidget : IRegisterableWidget
{
    public bool IsSmallWidget => false;
    public string WidgetName => "Weather";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) 
        => new WeatherWidget(parent, position, alignment);
}
