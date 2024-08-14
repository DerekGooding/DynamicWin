using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

public class SmallVisualizerWidget : SmallWidgetBase
{
    private readonly AudioVisualizer audioVisualizer;

    public SmallVisualizerWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
    {
        audioVisualizer = new AudioVisualizer(this, new Vec2(-2.25f, 0), new Vec2(GetWidgetSize().X, GetWidgetSize().Y), UIAlignment.Center, length: 16, 16)
        {
            divisor = 1.75f,
            barDownSmoothing = 10,
            barUpSmoothing = 20
        };
        AddLocalObject(audioVisualizer);
    }

    protected override float GetWidgetWidth() => base.GetWidgetWidth() * 2;
}