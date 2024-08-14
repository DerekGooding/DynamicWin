﻿using DynamicWin.Resources;
using DynamicWin.UI.UIElements;
using DynamicWin.Utils;

namespace DynamicWin.UI.Menu.Menus;

public class Checkbox : DWImageButton
{
    private bool isChecked = false;
    public bool IsChecked
    { get { return isChecked; } set => SetChecked(value); }

    private void SetChecked(bool isChecked)
    {
        this.isChecked = isChecked;
        Image.Image = isChecked ? Res.Check : null;
    }

    public Checkbox(UIObject? parent, string buttonText, Vec2 position, Vec2 size, Action clickCallback, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, Res.Check, position, size, clickCallback, alignment)
    {
        var text = new DWText(this, buttonText, new Vec2(15, 0), UIAlignment.MiddleRight)
        {
            Color = Theme.TextSecond
        };
        text.Anchor.X = 0;
        text.TextSize = size.Y / 1.5f;
        AddLocalObject(text);

        SetChecked(false);

        hoverScaleMulti = new Vec2(1.05f, 1f);
        clickScaleMulti = new Vec2(0.975f, 1f);
    }

    public override void OnMouseUp()
    {
        IsChecked = !IsChecked;
        base.OnMouseUp();
    }
}
