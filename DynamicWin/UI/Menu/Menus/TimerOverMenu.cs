using DynamicWin.Main;
using DynamicWin.UI.UIElements;
using DynamicWin.Utils;
using SkiaSharp;

namespace DynamicWin.UI.Menu.Menus;

public class TimerOverMenu : BaseMenu
{
    private DWText overText;
    private System.Media.SoundPlayer player;

    private DWWave wave;

    public override List<UIObject> InitializeMenu(IslandObject island)
    {
        var objects = base.InitializeMenu(island);

        overText = new DWText(island, "Timer Over!", new Vec2(0, 0), UIAlignment.Center)
        {
            TextSize = 20,
            Font = Resources.Res.InterBold
        };

        objects.Add(overText);

        player = new System.Media.SoundPlayer(Resources.Res.TimerOverSound);
        player.PlayLooping();

        wave = new DWWave(island, IslandSize());
        wave.SilentSetActive(false);
        objects.Add(wave);

        return objects;
    }

    public override void OnUnload() => player.Stop();

    private float sinCycle = -0.65f;
    private readonly float speed = 3.5f;

    private float islandSizeMulti = 1f;

    public override void Update()
    {
        base.Update();

        var delta = RendererMain.Instance.DeltaTime;
        sinCycle += delta * speed;

        overText.TextSize = Mathf.Remap((float)Math.Sin(sinCycle), -1, 1, 20, 25);

        if (Mathf.LimitDecimalPoints((float)Math.Sin(sinCycle + 0.1f), 1) == 1f)
        {
            islandSizeMulti = 1.2f;
            if (wave.waveSize > 45)
            {
                wave.waveSize = 0f;
            }
            wave.Size = IslandSize();
            wave.SilentSetActive(true);
        }

        islandSizeMulti = Mathf.Lerp(islandSizeMulti, 1f, 5f * delta);

        if (RendererMain.Instance.MainIsland.IsHovering && sinCycle >= 1)
            MenuManager.CloseOverlay();
    }

    public override Vec2 IslandSize() => new Vec2(250, 65) * islandSizeMulti;

    public override Vec2 IslandSizeBig() => base.IslandSizeBig();
}

internal class DWWave : UIObject
{
    public DWWave(UIObject? parent, Vec2 size, UIAlignment alignment = UIAlignment.Center) : base(parent, Vec2.zero, size, alignment)
    {
        maskInToIsland = false;
        roundRadius = parent.roundRadius;
    }

    public float waveSize;
    public float waveGrowSpeed = 65f;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        waveSize += waveGrowSpeed * deltaTime;

        blurAmount = Mathf.Clamp(waveSize, 0, 45);
        Alpha = 1f - Mathf.Clamp((waveSize - 15) / waveGrowSpeed * 2, 0, 1);
    }

    public override void Draw(SKCanvas canvas)
    {
        var paint = GetPaint();

        var rect = SKRect.Create(Position.X, Position.Y, Size.X, Size.Y);

        canvas.ClipRoundRect(RendererMain.Instance.MainIsland.GetRect(), SKClipOperation.Difference, true);

        paint.Color = GetColor(Theme.Primary.Override(a: 1)).Value();
        paint.IsStroke = true;
        paint.StrokeWidth = 5f;

        rect.Inflate(waveSize - 35, waveSize - 25);

        var rRect = new SKRoundRect(rect, roundRadius * (waveSize / 5));

        canvas.DrawRoundRect(rRect, paint);
    }
}