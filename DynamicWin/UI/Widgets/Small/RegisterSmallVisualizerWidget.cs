using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

internal class RegisterSmallVisualizerWidget : IRegisterableWidget
{
    public bool IsSmallWidget => true;
    public string WidgetName => "Audio Visualizer";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter)
        => new SmallVisualizerWidget(parent, position, alignment);
}
