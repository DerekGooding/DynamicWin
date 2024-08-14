using DynamicWin.Main;
using DynamicWin.Utils;

namespace DynamicWin.UI.UIElements.Custom;

public class DWSlider(UIObject? parent, Vec2 position, Vec2 size, UIAlignment alignment = UIAlignment.TopCenter) : DWProgressBar(parent, position, size, alignment)
{
    public Action<float>? clickCallback;
    private float valueBefore;

    public override void OnMouseDown()
    {
        base.OnMouseDown();

        valueBefore = value;
    }

    public override void OnGlobalMouseUp()
    {
        base.OnGlobalMouseUp();

        if (valueBefore != value)
            clickCallback?.Invoke(value);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (IsMouseDown)
        {
            value = Mathf.Clamp(Mathf.Remap(RendererMain.CursorPosition.X - Position.X, 0, Size.X, 0, 1), 0.05f, 1);
        }
    }
}