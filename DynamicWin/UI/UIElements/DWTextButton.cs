﻿using DynamicWin.Utils;

namespace DynamicWin.UI.UIElements;

public class DWTextButton : DWButton
{
    private DWText text;

    public DWText Text
    { get { return text; } set => text = value; }

    public float normalTextSize = 14;
    public float textSizeSmoothSpeed = 15f;

    public DWTextButton(UIObject? parent, string buttonText, Vec2 position, Vec2 size, Action clickCallback, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, size, clickCallback, alignment)
    {
        text = new DWText(this, buttonText, Vec2.zero, UIAlignment.Center);
        AddLocalObject(text);

        Text.TextSize = normalTextSize;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        float currentTextSize = normalTextSize;

        if (IsHovering && !IsMouseDown)
            currentTextSize *= hoverScaleMulti.Magnitude;
        else if (IsMouseDown)
            currentTextSize *= clickScaleMulti.Magnitude;
        else if (!IsHovering && !IsMouseDown)
            currentTextSize *= normalScaleMulti.Magnitude;
        else
            currentTextSize *= normalScaleMulti.Magnitude;

        Text.TextSize = Mathf.Lerp(Text.TextSize, currentTextSize, textSizeSmoothSpeed * deltaTime);
    }
}