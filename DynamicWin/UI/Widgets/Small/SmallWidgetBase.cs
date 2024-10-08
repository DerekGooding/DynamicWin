﻿using DynamicWin.Utils;

namespace DynamicWin.UI.Widgets.Small;

public class SmallWidgetBase : WidgetBase
{
    public SmallWidgetBase(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
    {
        roundRadius = 5f;
        isSmallWidget = true;
    }

    protected override float GetWidgetHeight() => 15;

    protected override float GetWidgetWidth() => 35;
}