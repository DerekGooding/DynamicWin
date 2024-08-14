using DynamicWin.Utils;
using SkiaSharp;

namespace DynamicWin.UI.Widgets.Small;

public class UsedDevicesWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : SmallWidgetBase(parent, position, alignment)
{
    readonly float camDotSize = 2.5f;
    float camDotSizeCurrent;
    float camDotPositionX;

    readonly float micDotSize = 2.5f;
    float micDotSizeCurrent;
    float micDotPositionX;

    readonly float separation = 6.5f;

    protected override float GetWidgetWidth() => (camDotSizeCurrent * 4) + (micDotSizeCurrent * 4);

    float sinCycleCamera = 1f;
    float sinCycleMicrophone;

    readonly float sinSpeed = 2.75f;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        sinCycleCamera += sinSpeed * deltaTime;
        sinCycleMicrophone += sinSpeed * deltaTime;

        const bool isCamActive = false; //DeviceUsageChecker.IsWebcamInUse();
        const bool isMicActive = false; //DeviceUsageChecker.IsMicrophoneInUse();

        camDotSizeCurrent = Mathf.Lerp(camDotSizeCurrent, isCamActive ? camDotSize : 0f, 5f * deltaTime);
        micDotSizeCurrent = Mathf.Lerp(micDotSizeCurrent, isMicActive ? micDotSize : 0f, 5f * deltaTime);

        camDotPositionX = Mathf.Lerp(camDotPositionX, 0, 5f * deltaTime);
        micDotPositionX = Mathf.Lerp(micDotPositionX, 0, 5f * deltaTime);

        isMicrophoneIndicatorShowing = LoudnessMeter.GetMicrophoneLoudness() > RegisterUsedDevicesOptions.saveData.indicatorThreshold;
    }

    bool isMicrophoneIndicatorShowing = false;

    public override void DrawWidget(SKCanvas canvas)
    {
        var paint = GetPaint();

        var camPos = GetScreenPosFromRawPosition(new Vec2(camDotPositionX, 0), new Vec2(0, camDotSizeCurrent / 2), UIAlignment.Center, this);

        paint.Color = Col.Lerp(Theme.Error, Theme.Error * 0.6f, Mathf.Remap((float)Math.Sin(sinCycleCamera), -1, 1, 0, 1)).Value();
        canvas.DrawCircle(camPos.X, camPos.Y, camDotSizeCurrent, paint);

        var micPos = GetScreenPosFromRawPosition(new Vec2(micDotPositionX, 0), new Vec2(0, micDotSizeCurrent / 2), UIAlignment.Center, this);

        paint.Color = Col.Lerp(Theme.Success, Theme.Success * 0.6f, Mathf.Remap((float)Math.Sin(sinCycleMicrophone), -1, 1, 0, 1)).Value();
        canvas.DrawCircle(micPos.X, micPos.Y, micDotSizeCurrent, paint);

        if (isMicrophoneIndicatorShowing && RegisterUsedDevicesOptions.saveData.enableIndicator)
        {
            paint.IsStroke = true;
            paint.StrokeWidth = 1.25f;

            var micIndicatorPos = GetScreenPosFromRawPosition(new Vec2(micDotPositionX, 0), new Vec2(0, micDotSizeCurrent / 2), UIAlignment.Center, this);
            canvas.DrawCircle(micIndicatorPos.X, micIndicatorPos.Y, micDotSizeCurrent + 3f, paint);
        }
    }
}