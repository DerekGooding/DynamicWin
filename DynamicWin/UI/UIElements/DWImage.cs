using DynamicWin.Utils;
using SkiaSharp;

namespace DynamicWin.UI.UIElements;

public class DWImage : UIObject
{
    public SKBitmap Image { get; set; }

    public bool maskOwnRect;
    public bool allowIconThemeColor = true;

    public DWImage(UIObject? parent, SKBitmap sprite, Vec2 position, Vec2 size, UIAlignment alignment = UIAlignment.TopCenter, bool maskOwnRect = false) : base(parent, position, size, alignment)
    {
        Image = sprite;
        this.maskOwnRect = maskOwnRect;

        Color = Theme.IconColor;
    }

    public override void Draw(SKCanvas canvas)
    {
        var paint = GetPaint();

        if (Image == null) return;

        if (allowIconThemeColor)
        {
            var imageFilter = SKImageFilter.CreateBlendMode(SKBlendMode.DstIn,
                    SKImageFilter.CreateColorFilter(SKColorFilter.CreateBlendMode(Color.Value(), SKBlendMode.Multiply)));

            if (GetBlur() != 0f)
            {
                var blur = SKImageFilter.CreateBlur(GetBlur(), GetBlur());

                if (blur != null && imageFilter != null)
                {
                    var composedFilter = SKImageFilter.CreateCompose(blur, imageFilter);
                    paint.ImageFilter = composedFilter;
                }
            }
            else
                paint.ImageFilter = imageFilter;
        }

        if (maskOwnRect)
        {
            int save = canvas.Save();
            canvas.ClipRoundRect(GetRect(), antialias: true);

            canvas.DrawBitmap(Image, SKRect.Create(Position.X, Position.Y, Size.X, Size.Y), paint);
            canvas.RestoreToCount(save);
        }
        else
        {
            canvas.DrawBitmap(Image, SKRect.Create(Position.X, Position.Y, Size.X, Size.Y), paint);
        }
    }
}