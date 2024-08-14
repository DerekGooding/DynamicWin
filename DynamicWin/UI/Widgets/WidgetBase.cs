using DynamicWin.Main;
using DynamicWin.Utils;
using SkiaSharp;
using System.Windows.Controls;

namespace DynamicWin.UI.Widgets;

public class WidgetBase : UIObject
{
    public bool isEditMode;

    protected bool isSmallWidget;
    public bool IsSmallWidget => isSmallWidget;

    public Action onEditRemoveWidget = () => {};
    public Action onEditMoveWidgetLeft = () => { };
    public Action onEditMoveWidgetRight = () => { };

    private float hoverProgress;

    //DWText widgetName;

    public WidgetBase(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, Vec2.zero, alignment)
    {
        Size = GetWidgetSize();

        var objs = InitializeWidget;
        objs.ForEach(AddLocalObject);

        roundRadius = 15f;
    }

    public Vec2 GetWidgetSize() => new(GetWidgetWidth(), GetWidgetHeight());

    protected virtual float GetWidgetHeight() => 100;

    protected virtual float GetWidgetWidth() => 200;

    public static List<UIObject> InitializeWidget => [];

    public override ContextMenu? GetContextMenu()
    {
        if (!isEditMode) return null;

        var ctx = new ContextMenu();

        MenuItem remove = new() { Header = "Remove" };
        remove.Click += (x, y) => onEditRemoveWidget.Invoke();

        MenuItem pL = new() { Header = "<- Push Left" };
        pL.Click += (x, y) => onEditMoveWidgetLeft.Invoke();

        MenuItem pR = new() { Header = "Push Right ->" };
        pR.Click += (x, y) => onEditMoveWidgetRight.Invoke();

        ctx.Items.Add(remove);
        ctx.Items.Add(pL);
        ctx.Items.Add(pR);

        return ctx;
    }

    public override void Draw(SKCanvas canvas)
    {
        Size = GetWidgetSize();

        hoverProgress = Mathf.Lerp(hoverProgress, IsHovering ? 1f : 0f, 10f * RendererMain.Instance.DeltaTime);

        if (hoverProgress > 0.025f)
        {
            var paint = GetPaint();
            paint.ImageFilter = SKImageFilter.CreateDropShadowOnly(0, 0, hoverProgress * 10, hoverProgress * 10, Theme.WidgetBackground.Override(a: hoverProgress / 10).Value());

            int ogC = canvas.Save();

            var p = Position + (Size / 2);
            canvas.Scale(1 + (hoverProgress / 60), 1 + (hoverProgress / 60), p.X, p.Y);

            int sc = canvas.Save();
            canvas.ClipRoundRect(GetRect(), SKClipOperation.Difference, antialias: true);
            canvas.DrawRoundRect(GetRect(), paint);
            canvas.RestoreToCount(sc);
        }

        DrawWidget(canvas);

        if (isEditMode)
        {
            var paint = GetPaint();

            paint.IsStroke = true;
            paint.StrokeCap = SKStrokeCap.Round;
            paint.StrokeJoin = SKStrokeJoin.Round;
            paint.StrokeWidth = 2f;

            const float expand = 10;
            SKRect bRect = SKRect.Create(Position.X - (expand / 2), Position.Y - (expand / 2), Size.X + expand, Size.Y + expand);
            SKRoundRect bRoundRect = new(bRect, roundRadius);

            int noClip = canvas.Save();

            paint.Color = SKColors.DimGray;
            canvas.DrawRoundRect(bRoundRect, paint);

            canvas.RestoreToCount(noClip);
        }
    }

    public virtual void DrawWidget(SKCanvas canvas)
    { }
}