using DynamicWin.Main;
using DynamicWin.Utils;
using SkiaSharp;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DynamicWin.UI;

public class UIObject
{
    public UIObject? Parent { get; set; }

    private Vec2 _position = Vec2.zero;
    private Col color = Col.White;

    public Vec2 RawPosition => _position;
    public Vec2 Position { get => GetPosition() + LocalPosition; set => _position = value; }
    public Vec2 LocalPosition { get; set; } = Vec2.zero;
    public Vec2 Anchor { get; set; } = new Vec2(0.5f, 0.5f);
    public Vec2 Size { get; set; } = Vec2.one;
    public Col Color { get => new(color.r, color.g, color.b, color.a * Alpha); set => color = value; }
    private bool isGlobalMouseDown;
    protected bool drawLocalObjects = true;

    public bool IsHovering { get; private set; }
    public bool IsMouseDown { get; private set; }

    public UIAlignment Alignment = UIAlignment.TopCenter;

    protected float localBlurAmount;
    public float blurAmount;
    public float roundRadius;
    public bool maskInToIsland = true;
    public List<UIObject> LocalObjects { get; } = [];

    private bool isEnabled = true;
    public bool IsEnabled { get => isEnabled; set => SetActive(value); }

    public float blurSizeOnDisable = 50;

    private float pAlpha = 1f;
    private float oAlpha = 1f;

    public float Alpha { get => (float)Math.Min(pAlpha, oAlpha); set => oAlpha = value; }

    protected void AddLocalObject(UIObject obj)
    {
        obj.Parent = this;
        LocalObjects.Add(obj);
    }

    protected void DestroyLocalObject(UIObject obj)
    {
        obj.DestroyCall();
        LocalObjects.Remove(obj);
    }

    public UIObject(UIObject? parent, Vec2 position, Vec2 size, UIAlignment alignment = UIAlignment.TopCenter)
    {
        Parent = parent;
        _position = position;
        Size = size;
        Alignment = alignment;

        contextMenu = CreateContextMenu();

        if (RendererMain.Instance != null)
        {
            RendererMain.Instance.ContextMenuOpening += CtxOpen;
            RendererMain.Instance.ContextMenuClosing += CtxClose;
        }
    }

    private void CtxOpen(object sender, RoutedEventArgs e)
    {
        if (RendererMain.Instance.ContextMenu != null)
            canInteract = false;
    }

    private void CtxClose(object sender, RoutedEventArgs e) => canInteract = true;

    public Vec2 GetScreenPosFromRawPosition(Vec2 position, Vec2? Size = null, UIAlignment alignment = UIAlignment.None, UIObject? parent = null)
    {
        parent ??= Parent;
        Size ??= this.Size;
        if (alignment == UIAlignment.None) alignment = Alignment;

        if (parent == null)
        {
            Vec2 screenDim = RendererMain.ScreenDimensions;
            Size ??= Vec2.one;
            switch (alignment)
            {
                case UIAlignment.TopLeft:
                    return new Vec2(position.X - (Size.X * Anchor.X),
                        position.Y - (Size.Y * Anchor.Y));

                case UIAlignment.TopCenter:
                    return new Vec2(position.X + (screenDim.X / 2) - (Size.X * Anchor.X),
                        position.Y - (Size.Y * Anchor.Y));

                case UIAlignment.TopRight:
                    return new Vec2(position.X + screenDim.X - (Size.X * Anchor.X),
                        position.Y - (Size.Y * Anchor.Y));

                case UIAlignment.MiddleLeft:
                    return new Vec2(position.X - (Size.X * Anchor.X),
                        position.Y + (screenDim.Y / 2) - (Size.Y * Anchor.Y));

                case UIAlignment.Center:
                    return new Vec2(position.X + (screenDim.X / 2) - (Size.X * Anchor.X),
                        position.Y + (screenDim.Y / 2) - (Size.Y * Anchor.Y));

                case UIAlignment.MiddleRight:
                    return new Vec2(position.X + screenDim.X - (Size.X * Anchor.X),
                        position.Y + (screenDim.Y / 2) - (Size.Y * Anchor.Y));

                case UIAlignment.BottomLeft:
                    return new Vec2(position.X - (Size.X * Anchor.X),
                        position.Y + screenDim.Y - (Size.Y * Anchor.Y));

                case UIAlignment.BottomCenter:
                    return new Vec2(position.X + (screenDim.X / 2) - (Size.X * Anchor.X),
                        position.Y + screenDim.Y - (Size.Y * Anchor.Y));

                case UIAlignment.BottomRight:
                    return new Vec2(position.X + screenDim.X - (Size.X * Anchor.X),
                        position.Y + screenDim.Y - (Size.Y * Anchor.Y));
            }
        }
        else
        {
            Vec2 parentDim = parent.Size;
            Vec2 parentPos = parent.Position;

            switch (alignment)
            {
                case UIAlignment.TopLeft:
                    return new Vec2(parentPos.X + position.X - (Size.X * Anchor.X),
                        parentPos.Y + position.Y - (Size.Y * Anchor.Y));

                case UIAlignment.TopCenter:
                    return new Vec2(parentPos.X + position.X + (parentDim.X / 2) - (Size.X * Anchor.X),
                        parentPos.Y + position.Y - (Size.Y * Anchor.Y));

                case UIAlignment.TopRight:
                    return new Vec2(parentPos.X + position.X + parentDim.X - (Size.X * Anchor.X),
                        parentPos.Y + position.Y - (Size.Y * Anchor.Y));

                case UIAlignment.MiddleLeft:
                    return new Vec2(parentPos.X + position.X - (Size.X * Anchor.X),
                        parentPos.Y + position.Y + (parentDim.Y / 2) - (Size.Y * Anchor.Y));

                case UIAlignment.Center:
                    return new Vec2(parentPos.X + position.X + (parentDim.X / 2) - (Size.X * Anchor.X),
                        parentPos.Y + position.Y + (parentDim.Y / 2) - (Size.Y * Anchor.Y));

                case UIAlignment.MiddleRight:
                    return new Vec2(parentPos.X + position.X + parentDim.X - (Size.X * Anchor.X),
                        parentPos.Y + position.Y + (parentDim.Y / 2) - (Size.Y * Anchor.Y));

                case UIAlignment.BottomLeft:
                    return new Vec2(parentPos.X + position.X - (Size.X * Anchor.X),
                        parentPos.Y + position.Y + parentDim.Y - (Size.Y * Anchor.Y));

                case UIAlignment.BottomCenter:
                    return new Vec2(parentPos.X + position.X + (parentDim.X / 2) - (Size.X * Anchor.X),
                        parentPos.Y + position.Y + parentDim.Y - (Size.Y * Anchor.Y));

                case UIAlignment.BottomRight:
                    return new Vec2(parentPos.X + position.X + parentDim.X - (Size.X * Anchor.X),
                        parentPos.Y + position.Y + parentDim.Y - (Size.Y * Anchor.Y));
            }
        }

        return Vec2.zero;
    }

    protected virtual Vec2 GetPosition()
    {
        if (Parent == null)
        {
            Vec2 screenDim = RendererMain.ScreenDimensions;
            switch (Alignment)
            {
                case UIAlignment.TopLeft:
                    return new Vec2(_position.X - (Size.X * Anchor.X),
                        _position.Y - (Size.Y * Anchor.Y));

                case UIAlignment.TopCenter:
                    return new Vec2(_position.X + (screenDim.X / 2) - (Size.X * Anchor.X),
                        _position.Y - (Size.Y * Anchor.Y));

                case UIAlignment.TopRight:
                    return new Vec2(_position.X + screenDim.X - (Size.X * Anchor.X),
                        _position.Y - (Size.Y * Anchor.Y));

                case UIAlignment.MiddleLeft:
                    return new Vec2(_position.X - (Size.X * Anchor.X),
                        _position.Y + (screenDim.Y / 2) - (Size.Y * Anchor.Y));

                case UIAlignment.Center:
                    return new Vec2(_position.X + (screenDim.X / 2) - (Size.X * Anchor.X),
                        _position.Y + (screenDim.Y / 2) - (Size.Y * Anchor.Y));

                case UIAlignment.MiddleRight:
                    return new Vec2(_position.X + screenDim.X - (Size.X * Anchor.X),
                        _position.Y + (screenDim.Y / 2) - (Size.Y * Anchor.Y));

                case UIAlignment.BottomLeft:
                    return new Vec2(_position.X - (Size.X * Anchor.X),
                        _position.Y + screenDim.Y - (Size.Y * Anchor.Y));

                case UIAlignment.BottomCenter:
                    return new Vec2(_position.X + (screenDim.X / 2) - (Size.X * Anchor.X),
                        _position.Y + screenDim.Y - (Size.Y * Anchor.Y));

                case UIAlignment.BottomRight:
                    return new Vec2(_position.X + screenDim.X - (Size.X * Anchor.X),
                        _position.Y + screenDim.Y - (Size.Y * Anchor.Y));
            }
        }
        else
        {
            Vec2 parentDim = Parent.Size;
            Vec2 parentPos = Parent.Position;

            switch (Alignment)
            {
                case UIAlignment.TopLeft:
                    return new Vec2(parentPos.X + _position.X - (Size.X * Anchor.X),
                        parentPos.Y + _position.Y - (Size.Y * Anchor.Y));

                case UIAlignment.TopCenter:
                    return new Vec2(parentPos.X + _position.X + (parentDim.X / 2) - (Size.X * Anchor.X),
                        parentPos.Y + _position.Y - (Size.Y * Anchor.Y));

                case UIAlignment.TopRight:
                    return new Vec2(parentPos.X + _position.X + parentDim.X - (Size.X * Anchor.X),
                        parentPos.Y + _position.Y - (Size.Y * Anchor.Y));

                case UIAlignment.MiddleLeft:
                    return new Vec2(parentPos.X + _position.X - (Size.X * Anchor.X),
                        parentPos.Y + _position.Y + (parentDim.Y / 2) - (Size.Y * Anchor.Y));

                case UIAlignment.Center:
                    return new Vec2(parentPos.X + _position.X + (parentDim.X / 2) - (Size.X * Anchor.X),
                        parentPos.Y + _position.Y + (parentDim.Y / 2) - (Size.Y * Anchor.Y));

                case UIAlignment.MiddleRight:
                    return new Vec2(parentPos.X + _position.X + parentDim.X - (Size.X * Anchor.X),
                        parentPos.Y + _position.Y + (parentDim.Y / 2) - (Size.Y * Anchor.Y));

                case UIAlignment.BottomLeft:
                    return new Vec2(parentPos.X + _position.X - (Size.X * Anchor.X),
                        parentPos.Y + _position.Y + parentDim.Y - (Size.Y * Anchor.Y));

                case UIAlignment.BottomCenter:
                    return new Vec2(parentPos.X + _position.X + (parentDim.X / 2) - (Size.X * Anchor.X),
                        parentPos.Y + _position.Y + parentDim.Y - (Size.Y * Anchor.Y));

                case UIAlignment.BottomRight:
                    return new Vec2(parentPos.X + _position.X + parentDim.X - (Size.X * Anchor.X),
                        parentPos.Y + _position.Y + parentDim.Y - (Size.Y * Anchor.Y));
            }
        }

        return Vec2.zero;
    }

    public float GetBlur()
    {
        if (!Settings.AllowBlur) return 0f;
        return Math.Max(blurAmount, localBlurAmount);
    }

    bool canInteract = true;

    public void UpdateCall(float deltaTime)
    {
        if (!isEnabled) return;

        if (Parent != null)
            pAlpha = Parent.Alpha;

        if (canInteract)
        {
            var rect = SKRect.Create(RendererMain.CursorPosition.X, RendererMain.CursorPosition.Y, 1, 1);
            IsHovering = GetInteractionRect().Contains(rect);

            if (!isGlobalMouseDown && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                isGlobalMouseDown = true;
                OnGlobalMouseDown();
            }
            else if (isGlobalMouseDown && Mouse.LeftButton != MouseButtonState.Pressed)
            {
                isGlobalMouseDown = false;
                OnGlobalMouseUp();
            }

            if (IsHovering && !IsMouseDown && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                IsMouseDown = true;
                OnMouseDown();
            }
            else if (IsHovering && IsMouseDown && Mouse.LeftButton != MouseButtonState.Pressed)
            {
                IsMouseDown = false;
                OnMouseUp();
            }
            else if (IsMouseDown && Mouse.LeftButton != MouseButtonState.Pressed)
            {
                IsMouseDown = false;
            }
        }

        Update(deltaTime);

        if (drawLocalObjects)
        {
            new List<UIObject>(LocalObjects).ForEach((UIObject obj) =>
            {
                obj.blurAmount = GetBlur();
                obj.UpdateCall(deltaTime);
            });
        }
    }

    public virtual void Update(float deltaTime) { }

    public void DrawCall(SKCanvas canvas)
    {
        if (!isEnabled) return;

        Draw(canvas);

        if (drawLocalObjects)
        {
            new List<UIObject>(LocalObjects).ForEach((UIObject obj) => obj.DrawCall(canvas));
        }
    }

    public virtual void Draw(SKCanvas canvas)
    {
        var rect = SKRect.Create(Position.X, Position.Y, Size.X, Size.Y);
        var roundRect = new SKRoundRect(rect, roundRadius);

        var paint = GetPaint();

        canvas.DrawRoundRect(roundRect, paint);
    }

    public virtual SKPaint GetPaint()
    {
        var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = Color.Value(),
            IsAntialias = Settings.AntiAliasing,
            IsDither = true,
            SubpixelText = false,
            FilterQuality = SKFilterQuality.Medium,
            HintingLevel = SKPaintHinting.Normal,
            IsLinearText = true
        };

        if (GetBlur() != 0f)
        {
            paint.ImageFilter = SKImageFilter.CreateBlur(GetBlur(), GetBlur());
        }

        return paint;
    }

    public Col GetColor(Col col) => new(col.r, col.g, col.b, col.a * Alpha);

    public void DestroyCall()
    {
        LocalObjects.ForEach((UIObject obj) => obj.DestroyCall());

        OnDestroy();
    }

    public virtual void OnDestroy() { }

    public virtual void OnMouseDown() { }
    public virtual void OnGlobalMouseDown() { }

    public virtual void OnMouseUp() { }
    public virtual void OnGlobalMouseUp() { }

    public void SilentSetActive(bool isEnabled) => this.isEnabled = isEnabled;

    Animator? toggleAnimation;

    public void SetActive(bool isEnabled)
    {
        if (this.isEnabled == isEnabled) return;
        if (toggleAnimation?.IsRunning == true) toggleAnimation.Stop();

        if (isEnabled)
            this.isEnabled = isEnabled;

        toggleAnimation = new Animator(250, 1);
        toggleAnimation.onAnimationUpdate += (t) =>
        {
            if (t >= 0.5f) this.isEnabled = isEnabled;

            if (isEnabled)
            {
                var tEased = Easing.EaseOutCubic(t);

                localBlurAmount = Mathf.Lerp(blurSizeOnDisable, 0, tEased);
                Alpha = Mathf.Lerp(0, 1, tEased);
            }
            else
            {
                var tEased = Easing.EaseOutCubic(t);

                localBlurAmount = Mathf.Lerp(0, blurSizeOnDisable, tEased);
                Alpha = Mathf.Lerp(1, 0, tEased);
            }
        };
        toggleAnimation.onAnimationEnd += () =>
        {
            this.isEnabled = isEnabled;
            DestroyLocalObject(toggleAnimation);
        };

        AddLocalObject(toggleAnimation);
        toggleAnimation.Start();
    }

    public virtual SKRoundRect GetRect()
    {
        var rect = SKRect.Create((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        return new SKRoundRect(rect, roundRadius);
    }

    public int expandInteractionRect = 5;

    public virtual SKRoundRect GetInteractionRect()
    {
        var rect = SKRect.Create((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        var r = new SKRoundRect(rect, roundRadius);
        r.Deflate(-expandInteractionRect, -expandInteractionRect);
        return r;
    }

    readonly ContextMenu? contextMenu;

    public virtual ContextMenu? CreateContextMenu() => null;
    public virtual ContextMenu? GetContextMenu() => contextMenu;
}

public enum UIAlignment
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    Center,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight,
    None
}