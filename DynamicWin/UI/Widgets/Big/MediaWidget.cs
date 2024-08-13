using DynamicWin.UI.UIElements;
using DynamicWin.Utils;
using SkiaSharp;
using System.Diagnostics;

namespace DynamicWin.UI.Widgets.Big;

class RegisterMediaWidget : IRegisterableWidget
{
    public bool IsSmallWidget => false;
    public string WidgetName => "Media Playback Control";

    public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter)
        => new MediaWidget(parent, position, alignment);
}

public class MediaWidget : WidgetBase
{
    MediaController controller;
    readonly AudioVisualizer audioVisualizer;
    readonly AudioVisualizer audioVisualizerBig;

    readonly DWImageButton playPause;
    readonly DWImageButton next;
    readonly DWImageButton prev;

    readonly DWText noMediaPlaying;

    readonly DWText title;
    readonly DWText artist;

    /*protected override float GetWidgetWidth()
    {
        return base.GetWidgetWidth() * 2f;
    }*/

    public MediaWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
    {
        InitMediaPlayer();

        playPause = new DWImageButton(this, Resources.Res.PlayPause, new Vec2(0, 25), new Vec2(30, 30), controller.PlayPause, alignment: UIAlignment.Center)
        {
            roundRadius = 25,
            normalColor = Col.Transparent,
            hoverColor = Col.White.Override(a: 0.1f),
            clickColor = Col.White.Override(a: 0.25f),
            hoverScaleMulti = Vec2.one * 1.25f,
            imageScale = 0.8f
        };
        AddLocalObject(playPause);

        next = new DWImageButton(this, Resources.Res.Next, new Vec2(50, 25), new Vec2(30, 30), controller.Next, alignment: UIAlignment.Center)
        {
            roundRadius = 25,
            normalColor = Col.Transparent,
            hoverColor = Col.White.Override(a: 0.1f),
            clickColor = Col.White.Override(a: 0.25f),
            hoverScaleMulti = Vec2.one * 1.25f,
            imageScale = 0.65f
        };
        AddLocalObject(next);

        prev = new DWImageButton(this, Resources.Res.Previous, new Vec2(-50, 25), new Vec2(30, 30), controller.Previous, alignment: UIAlignment.Center)
        {
            roundRadius = 25,
            normalColor = Col.Transparent,
            hoverColor = Col.White.Override(a: 0.1f),
            clickColor = Col.White.Override(a: 0.25f),
            hoverScaleMulti = Vec2.one * 1.25f,
            imageScale = 0.65f
        };
        AddLocalObject(prev);

        audioVisualizer = new AudioVisualizer(this, new Vec2(0, 30), new Vec2(125, 25), length: 16)
        {
            divisor = 1.35f
        };
        AddLocalObject(audioVisualizer);

        audioVisualizerBig = new AudioVisualizer(this, new Vec2(0, 0), GetWidgetSize(), alignment: UIAlignment.Center, length: 16,
            Primary: spotifyCol.Override(a: 0.35f), Secondary: spotifyCol.Override(a: 0.025f) * 0.1f)
        {
            divisor = 2f
        };
        audioVisualizerBig.SilentSetActive(false);

        noMediaPlaying = new DWText(this, "No Media Playing", new Vec2(0, 30))
        {
            Color = Theme.TextSecond,
            Font = Resources.Res.InterBold,
            TextSize = 16
        };
        noMediaPlaying.SilentSetActive(false);
        AddLocalObject(noMediaPlaying);

        title = new DWText(this, "Title", new Vec2(0, 22.5f))
        {
            Color = Theme.TextSecond,
            Font = Resources.Res.InterBold,
            TextSize = 15
        };
        title.SilentSetActive(false);
        AddLocalObject(title);

        artist = new DWText(this, "Artist", new Vec2(0, 42.5f))
        {
            Color = Theme.TextThird,
            Font = Resources.Res.InterRegular,
            TextSize = 13
        };
        artist.SilentSetActive(false);
        AddLocalObject(artist);
    }

    float smoothedAmp;
    readonly float smoothing = 1.5f;

    int cycle;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (cycle % 32 == 0)
        {
            isSpotifyAvaliable = IsSpotifyAvailable();

            if (isSpotifyAvaliable)
            {
                GetSpotifyTrackInfo(out string titleString, out string artistString, out string error);

                if (string.IsNullOrEmpty(error))
                {
                    title.Text = DWText.Truncate(titleString, GetWidgetWidth() == 400 ? 50 : 20);
                    artist.Text = DWText.Truncate(artistString, GetWidgetWidth() == 400 ? 60 : 28);
                }
                else if (!error.Equals("Paused")
                        || title.Text.Equals("title", StringComparison.OrdinalIgnoreCase)
                        || title.Text.Equals("error", StringComparison.OrdinalIgnoreCase))
                {
                    title.Text = "Error";
                    artist.Text = DWText.Truncate(error, 24);
                }
            }
        }
        cycle++;

        prev.normalColor = Theme.IconColor * audioVisualizer.GetActionCol().Override(a: 0.2f);
        next.normalColor = Theme.IconColor * audioVisualizer.GetActionCol().Override(a: 0.2f);
        playPause.normalColor = Theme.IconColor * audioVisualizer.GetActionCol().Override(a: 0.2f);

        if (!isSpotifyAvaliable)
            smoothedAmp = Math.Max(Mathf.Lerp(smoothedAmp, audioVisualizer.AverageAmplitude, smoothing * deltaTime), audioVisualizer.AverageAmplitude);
        else
            smoothedAmp = Math.Max(Mathf.Lerp(smoothedAmp, audioVisualizer.AverageAmplitude, smoothing * deltaTime), audioVisualizer.AverageAmplitude);

        if (smoothedAmp < 0.005f) smoothedAmp = 0f;

        spotifyBlur = Mathf.Lerp(spotifyBlur, isSpotifyAvaliable ? 0f : 25f, 10f * deltaTime);

        noMediaPlaying.SetActive(smoothedAmp.Equals(0f) && !isSpotifyAvaliable);
        title.SetActive(isSpotifyAvaliable);
        artist.SetActive(isSpotifyAvaliable);
        audioVisualizerBig.SetActive(isSpotifyAvaliable);
        audioVisualizer.SetActive(!isSpotifyAvaliable);

        audioVisualizerBig.UpdateCall(deltaTime);
    }

    private void InitMediaPlayer()
    {
        controller = new MediaController();
    }

    float spotifyBlur = 0f;
    bool isSpotifyAvaliable = false;
    readonly Col spotifyCol = Col.FromHex("#1cb351");

    public override void DrawWidget(SKCanvas canvas)
    {
        var paint = GetPaint();
        paint.Color = GetColor(Theme.WidgetBackground).Value();
        canvas.DrawRoundRect(GetRect(), paint);

        int saveCanvas = canvas.Save();

        canvas.ClipRoundRect(GetRect());

        audioVisualizerBig.Alpha = 0.75f;
        audioVisualizerBig.blurAmount = 15f;
        audioVisualizerBig.DrawCall(canvas);

        canvas.RestoreToCount(saveCanvas);

        if (isSpotifyAvaliable || spotifyBlur <= 24f)
        {
            paint.Color = spotifyCol.Override(a: Color.a * (1f - (spotifyBlur / 25f))).Value();

            paint.ImageFilter = SKImageFilter.CreateBlur((float)Math.Max(GetBlur(), spotifyBlur) + 0.1f, (float)Math.Max(GetBlur(), spotifyBlur) + 0.1f);

            var r = GetRect();

            const int inward = 5;
            var w = 25 + (spotifyBlur * (isSpotifyAvaliable ? 2.5f : -0.15f));
            var h = 25 + (spotifyBlur * (isSpotifyAvaliable ? 2.5f : -0.15f));
            var x = Position.X + r.Width - (w / 2) - inward;
            var y = Position.Y - (h / 2) + inward;

            canvas.DrawBitmap(Resources.Res.Spotify ?? Resources.Res.SevereWeatherWarning, SKRect.Create(x, y, w, h), paint);

            paint.Color = GetColor(spotifyCol.Override(a: 0.25f * Color.a)).Value();
            paint.StrokeWidth = 2f;

            float[] intervals = [5, 10];
            paint.PathEffect = SKPathEffect.CreateDash(intervals, -cycle * 0.1f);

            paint.IsStroke = true;
            paint.StrokeCap = SKStrokeCap.Round;
            paint.StrokeJoin = SKStrokeJoin.Round;

            canvas.DrawRoundRect(r, paint);

            canvas.RestoreToCount(saveCanvas);

            var shadowPaint = GetPaint();

            shadowPaint.ImageFilter = SKImageFilter.CreateDropShadowOnly(0, 0, (25f - spotifyBlur) / 2, (25f - spotifyBlur) / 2,
                spotifyCol.Override(a: (25f - spotifyBlur) / 100f).Value());
            shadowPaint.IsStroke = true;
            shadowPaint.StrokeWidth = 15;

            int s = canvas.Save();
            canvas.ClipRoundRect(GetRect());
            canvas.DrawRoundRect(GetRect(), shadowPaint);
            canvas.RestoreToCount(s);
        }
    }

    public static void GetSpotifyTrackInfo(out string title, out string artist, out string error)
    {
        var proc = Process.GetProcessesByName("Spotify").FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle));

        if (proc == null)
        {
            error = "Spotify is not open!";
            title = null;
            artist = null;
            return;
        }

        if (proc.MainWindowTitle.StartsWith("spotify", StringComparison.CurrentCultureIgnoreCase))
        {
            error = "Paused";
            title = null;
            artist = null;
            return;
        }

        string[] strings = proc.MainWindowTitle.Split(" - ");

        if (strings.Length >= 2)
        {
            title = strings[1];
            artist = strings[0];
            error = null;
        }
        else
        {
            error = null;
            title = "Advertisement";
            artist = "";
        }
    }

    public static bool IsSpotifyAvailable() => Process.GetProcessesByName("Spotify").Length != 0;
}