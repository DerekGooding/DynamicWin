using DynamicWin.Resources;
using DynamicWin.UI;
using DynamicWin.UI.Menu;
using DynamicWin.UI.Menu.Menus;
using DynamicWin.UI.UIElements;
using DynamicWin.Utils;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace DynamicWin.Main;

public class RendererMain : SKElement, IDisposable
{
    public IslandObject MainIsland { get; }

    private List<UIObject> Objects => MenuManager.Instance.ActiveMenu.UiObjects;

    public static Vec2 ScreenDimensions => new(MainForm.Instance.Width, MainForm.Instance.Height);
    public static Vec2 CursorPosition => new(Mouse.GetPosition(MainForm.Instance).X, Mouse.GetPosition(MainForm.Instance).Y);

    public static RendererMain? Instance { get; private set; }

    public Vec2 renderOffset = Vec2.zero;

    public Action<float> onUpdate;
    public Action<SKCanvas> onDraw;

    public RendererMain()
    {
        MenuManager m = new();
        Instance = this;

        MainIsland = new IslandObject();
        m.Init();

        MainForm.Instance.onMainFormRender += Update;
        MainForm.Instance.onMainFormRender += Render;

        initialScreenBrightness = BrightnessAdjustMenu.GetBrightness();

        KeyHandler.onKeyDown += OnKeyRegistered;

        {
            MainForm.Instance.DragEnter += MainForm.Instance.MainForm_DragEnter;
            MainForm.Instance.DragLeave += MainForm.Instance.MainForm_DragLeave;
            MainForm.Instance.Drop += MainForm.Instance.OnDrop;

            MainForm.Instance.MouseWheel += MainForm.Instance.OnScroll;
        }

        isInitialized = true;
    }

    public void Dispose()
    {
        //timer.Stop();

        MainForm.Instance.onMainFormRender -= Update;
        MainForm.Instance.onMainFormRender -= Render;

        KeyHandler.onKeyDown -= OnKeyRegistered;

        {
            MainForm.Instance.DragEnter -= MainForm.Instance.MainForm_DragEnter;
            MainForm.Instance.DragLeave -= MainForm.Instance.MainForm_DragLeave;

            MainForm.Instance.MouseWheel -= MainForm.Instance.OnScroll;
        }

        Instance = null;
    }

    private void OnKeyRegistered(Keys key, KeyModifier modifier)
    {
        if (key == Keys.LWin && modifier.isCtrlDown)
        {
            MainIsland.hidden = !MainIsland.hidden;
        }

        if (key == Keys.VolumeDown || key == Keys.VolumeMute || key == Keys.VolumeUp)
        {
            if (MenuManager.Instance.ActiveMenu is HomeMenu)
            {
                MenuManager.OpenOverlayMenu(new VolumeAdjustMenu(), 100f);
            }
            else
            {
                if (VolumeAdjustMenu.timerUntilClose != null)
                    VolumeAdjustMenu.timerUntilClose = 0f;
            }
        }

        if (key == Keys.MediaNextTrack || key == Keys.MediaPreviousTrack)
        {
            if (MenuManager.Instance.ActiveMenu is HomeMenu)
            {
                if (key == Keys.MediaNextTrack) Res.HomeMenu.NextSong(); else Res.HomeMenu.PrevSong();
            }
        }
    }

    public float DeltaTime { get; private set; }

    private Stopwatch? updateStopwatch;

    private int initialScreenBrightness;

    // Called once every frame to update values

    private void Update()
    {
        if (updateStopwatch != null)
        {
            updateStopwatch.Stop();
            DeltaTime = updateStopwatch.ElapsedMilliseconds / 1000f;
        }
        else
        {
            DeltaTime = 1f / 1000f;
        }

        updateStopwatch = new Stopwatch();
        updateStopwatch.Start();

        onUpdate?.Invoke(DeltaTime);

        if (BrightnessAdjustMenu.GetBrightness() != initialScreenBrightness)
        {
            initialScreenBrightness = BrightnessAdjustMenu.GetBrightness();
            if (MenuManager.Instance.ActiveMenu is HomeMenu)
            {
                MenuManager.OpenOverlayMenu(new BrightnessAdjustMenu(), 100f);
            }
            else
            {
                if (BrightnessAdjustMenu.timerUntilClose != null)
                {
                    BrightnessAdjustMenu.PressBK();
                    BrightnessAdjustMenu.timerUntilClose = 0f;
                }
            }
        }

        // Update Menu

        MenuManager.Instance.Update(DeltaTime);

        MenuManager.Instance.ActiveMenu?.Update();

        if (MenuManager.Instance.ActiveMenu is DropFileMenu && !MainForm.Instance.isDragging)
            MenuManager.OpenMenu(Res.HomeMenu);

        // Update logic here

        MainIsland.UpdateCall(DeltaTime);

        if (MainIsland.hidden) return;

        foreach (UIObject uiObject in Objects)
        {
            uiObject.UpdateCall(DeltaTime);
        }
    }

    // Called once every frame to render frame, called after Update

    private void Render() => Dispatcher.Invoke(InvalidateVisual);

    public int canvasWithoutClip;
    private readonly bool isInitialized;

    //private GRContext Context;

    /*        public SKSurface GetOpenGlSurface(int width, int height)
            {
                if (Context == null)
                {
                    GLControl control = new GLControl(new GraphicsMode(32, 24, 8, 4));
                    control.MakeCurrent();
                    Context = GRContext.CreateGl();
                }
                var gpuSurface = SKSurface.Create(Context, true, new SKImageInfo(width, height));
                return gpuSurface;
            }*/

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);

        if (!isInitialized) return;

        // Render

        // Get the canvas and information about the surface
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;
        SKImageInfo info = e.Info;

        canvas.Clear(SKColors.Transparent);

        //var mainScale = 2f;
        //canvas.Scale(mainScale, mainScale, MainIsland.Position.X + MainIsland.currSize.X / 2, 0);

        canvasWithoutClip = canvas.Save();

        if (MainIsland.maskInToIsland) Mask(canvas);
        MainIsland.DrawCall(canvas);

        if (MainIsland.hidden) return;

        bool hasContextMenu = false;
        foreach (UIObject uiObject in Objects)
        {
            canvas.RestoreToCount(canvasWithoutClip);
            canvasWithoutClip = canvas.Save();

            if (uiObject.IsHovering && uiObject.GetContextMenu() != null)
            {
                hasContextMenu = true;

                var contextMenu = uiObject.GetContextMenu();
                //contextMenu.BackColor = Theme.IslandBackground.ValueSystem();
                //contextMenu.ForeColor = Theme.TextMain.ValueSystem();

                ContextMenu = contextMenu;
            }

            foreach (UIObject obj in uiObject.LocalObjects)
            {
                if (obj.IsHovering && obj.GetContextMenu() != null)
                {
                    hasContextMenu = true;

                    var contextMenu = obj.GetContextMenu();
                    //contextMenu.BackColor = Theme.IslandBackground.ValueSystem();
                    //contextMenu.ForeColor = Theme.TextMain.ValueSystem();

                    ContextMenu = contextMenu;
                }
            }

            if (uiObject.maskInToIsland)
            {
                Mask(canvas);
            }

            canvas.Translate(renderOffset.X, renderOffset.Y);
            uiObject.DrawCall(canvas);
        }

        onDraw?.Invoke(canvas);

        if (!hasContextMenu) ContextMenu = null;

        canvas.Flush();
    }

    private void Mask(SKCanvas canvas)
    {
        var islandMask = GetMask();
        canvas.ClipRoundRect(islandMask);
    }

    public SKRoundRect GetMask()
    {
        var islandMask = MainIsland.GetRect();
        islandMask.Deflate(new SKSize(1, 1));
        return islandMask;
    }
}