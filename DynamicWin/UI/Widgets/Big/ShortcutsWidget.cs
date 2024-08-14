using DynamicWin.Utils;
using SkiaSharp;

namespace DynamicWin.UI.Widgets.Big;

public class ShortcutsWidget : WidgetBase
{
    public ShortcutsWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
    {
        AddLocalObject(new ShortcutButton(this, "s1", new Vec2(GetWidgetWidth() / 4, GetWidgetHeight() / 4),
            GetWidgetSize() / 2f, UIAlignment.TopLeft));
        AddLocalObject(new ShortcutButton(this, "s2", new Vec2(-GetWidgetWidth() / 4, GetWidgetHeight() / 4),
            GetWidgetSize() / 2f, UIAlignment.TopRight));
        AddLocalObject(new ShortcutButton(this, "s3", new Vec2(GetWidgetWidth() / 4, -GetWidgetHeight() / 4),
            GetWidgetSize() / 2f, UIAlignment.BottomLeft));
        AddLocalObject(new ShortcutButton(this, "s4", new Vec2(-GetWidgetWidth() / 4, -GetWidgetHeight() / 4),
            GetWidgetSize() / 2f, UIAlignment.BottomRight));
    }

    public override void DrawWidget(SKCanvas canvas)
    {
        base.DrawWidget(canvas);

        var paint = GetPaint();
        paint.Color = GetColor(Theme.WidgetBackground).Value();
        canvas.DrawRoundRect(GetRect(), paint);
    }
}