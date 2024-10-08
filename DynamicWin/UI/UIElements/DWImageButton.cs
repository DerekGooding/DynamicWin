﻿using DynamicWin.Utils;
using SkiaSharp;

namespace DynamicWin.UI.UIElements;

public class DWImageButton : DWButton
{
    public float imageScale = 0.85f;

    public DWImage Image { get; set; }

    public DWImageButton(UIObject? parent, SKBitmap image, Vec2 position, Vec2 size, Action clickCallback, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, size, clickCallback, alignment)
    {
        Image = new DWImage(this, image, Vec2.zero, size * imageScale, UIAlignment.Center);
        AddLocalObject(Image);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        Image.Size = Size * imageScale;
    }
}