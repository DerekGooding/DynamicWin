﻿using DynamicWin.UI.UIElements;
using DynamicWin.Utils;
using SkiaSharp;
using System.Windows.Controls;

namespace DynamicWin.UI.Menu.Menus;

internal class BigWidgetAdderDisplay : UIObject
{
    public BigWidgetAdderDisplay(UIObject? parent, string widgetName, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, Vec2.zero, Vec2.zero, alignment)
    {
        Size.X = parent.Size.X / 2;
        Size.Y = 45;

        Anchor = Vec2.zero;

        AddLocalObject(new DWText(this, DWText.Truncate(widgetName, 25), Vec2.zero, UIAlignment.Center)
        {
            TextSize = 14
        });

        roundRadius = 45;

        color = Theme.WidgetBackground.Override(a: 0.15f);
    }

    public override void Draw(SKCanvas canvas)
    {
        int canvasRestore = canvas.Save();

        var p = Position + (Size / 2);
        canvas.Scale(this.s, this.s, p.X, p.Y);

        var paint = GetPaint();
        var rect = GetRect();

        paint.Color = color.Value();

        rect.Deflate(5, 5);
        canvas.DrawRoundRect(rect, paint);

        canvas.RestoreToCount(canvasRestore);
    }

    private readonly Col color;
    private float s = 1;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        color.a = Mathf.Lerp(color.a, IsHovering ? 0.2f : 0.15f, 7.5f * deltaTime);
        s = Mathf.Lerp(s, IsHovering ? 1.025f : 1, 15f * deltaTime);
    }

    public Action onEditRemoveWidget;
    public Action onEditMoveWidgetLeft;
    public Action onEditMoveWidgetRight;

    public override ContextMenu? GetContextMenu()
    {
        var ctx = new System.Windows.Controls.ContextMenu();

        MenuItem remove = new() { Header = "Remove" };
        remove.Click += (x, y) => onEditRemoveWidget?.Invoke();

        MenuItem pL = new() { Header = "<- Push Left" };
        pL.Click += (x, y) => onEditMoveWidgetLeft?.Invoke();

        MenuItem pR = new() { Header = "Push Right ->" };
        pR.Click += (x, y) => onEditMoveWidgetRight?.Invoke();

        ctx.Items.Add(remove);
        ctx.Items.Add(pL);
        ctx.Items.Add(pR);

        return ctx;
    }
}
