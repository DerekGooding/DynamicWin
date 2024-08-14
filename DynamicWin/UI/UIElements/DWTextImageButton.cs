using DynamicWin.Utils;
using SkiaSharp;

namespace DynamicWin.UI.UIElements;

public class DWTextImageButton : DWButton
{
    public DWText Text { get; set; }

    public float normalTextSize = 10;
    public float textSizeSmoothSpeed = 15f;
    public float imageScale = 0.85f;

    public DWImage Image { get; }

    public DWTextImageButton(UIObject? parent, SKBitmap image, string buttonText, Vec2 position, Vec2 size, Action clickCallback, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, size, clickCallback, alignment)
    {
        Text = new DWText(this, buttonText, new Vec2(-7.5f, 0), UIAlignment.MiddleRight);
        Text.Anchor.X = 0f;
        AddLocalObject(Text);

        Image = new DWImage(this, image, new Vec2(15, 0), Vec2.one * size.Y * imageScale, UIAlignment.MiddleLeft);
        Text.Anchor.X = 1f;
        AddLocalObject(Image);

        Text.TextSize = normalTextSize;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        float currentTextSize = normalTextSize;

        Image.Size = Vec2.one * Size.Y * imageScale;

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