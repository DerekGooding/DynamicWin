using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Big;

internal class RegisterMediaWidget : IRegisterableWidget
{
    public bool IsSmallWidget => false;
    public string WidgetName => "Media Playback Control";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter)
        => new MediaWidget(parent, position, alignment);
}
