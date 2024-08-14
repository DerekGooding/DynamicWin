using DynamicWin.Utils;
using SkiaSharp;

namespace DynamicWin.UI.UIElements;

public class DWText : UIObject
{
    private string text = "";
    public string Text
    { get { return text; } set { SetText(value); } }
    public float TextSize { get; set; } = 24;

    private Vec2 textBounds;
    public SKTypeface Font { get; set; }

    public Vec2 TextBounds => textBounds ?? Vec2.zero;

    public DWText(UIObject? parent, string text, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, Vec2.zero, alignment)
    {
        this.text = text;
        Color = Theme.TextMain;
        Font = Resources.Res.InterRegular;
    }

    public override void Draw(SKCanvas canvas)
    {
        var paint = GetPaint();
        paint.Color = Color.Value();
        paint.TextSize = TextSize;
        paint.Typeface = Font;

        // Measure the width of the text
        Size.X = paint.MeasureText(text);

        // Measure the height of the text
        var fontMetrics = paint.FontMetrics;
        Size.Y = fontMetrics.Descent + fontMetrics.Ascent;

        SKTextBlob blob = SKTextBlob.Create(text, new SKFont(paint.Typeface, TextSize));

        if (blob != null)
        {
            canvas.DrawText(blob, Position.X, Position.Y, paint);
            textBounds = new Vec2(blob.Bounds.Width, blob.Bounds.Height);
        }

        Size = textBounds;

        //canvas.DrawRoundRect(GetRect(), paint);
    }

    public Vec2 GetBoundsForString(string text)
    {
        SKTextBlob blob = SKTextBlob.Create(text, new SKFont(Font, TextSize));

        return new Vec2(blob.Bounds.Width, blob.Bounds.Height);
    }

    private Animator changeTextAnim;

    public void SilentSetText(string text) => this.text = text;

    public void SetText(string text)
    {
        if (this.text == text) return;
        if (changeTextAnim?.IsRunning == true) return;

        float ogTextSize = TextSize;

        changeTextAnim = new Animator(350, 1);

        changeTextAnim.onAnimationUpdate += (x) =>
        {
            if (x <= 0.5f)
            {
                float t = Easing.EaseInQuint(x * 2);

                TextSize = Mathf.Lerp(ogTextSize, ogTextSize * 1.5f, t);
                localBlurAmount = Mathf.Lerp(0, 10, t);
                Alpha = Mathf.Lerp(1, 0, x);
            }
            else
            {
                this.text = text;

                float t = Easing.EaseOutQuint((x - 0.5f) * 2);

                TextSize = Mathf.Lerp(ogTextSize / 2.5f, ogTextSize, t);
                localBlurAmount = Mathf.Lerp(10, 0, t);
                Alpha = Mathf.Lerp(0, 1, x);
            }
        };

        AddLocalObject(changeTextAnim);
        changeTextAnim.Start();
        changeTextAnim.onAnimationEnd += () =>
        {
            this.text = text;
            TextSize = ogTextSize;
            DestroyLocalObject(changeTextAnim);
        };
    }

    public static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : $"{value.AsSpan(0, maxLength)}…";
    }
}