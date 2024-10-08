﻿using DynamicWin.UI;
using SkiaSharp;

namespace DynamicWin.Utils;

public class Animator : UIObject
{
    public Action<float> onAnimationUpdate;

    public Action onAnimationStart;
    public Action onAnimationEnd;
    public Action onAnimationInterrupt;

    public int animationDuration;
    public int animationInterval;

    private bool isRunning;
    public bool IsRunning => isRunning;

    public Animator(int animationDuration, int animationInterval = 1) : base(null, Vec2.zero, Vec2.zero)
    {
        this.animationDuration = animationDuration;
        this.animationInterval = animationInterval;
    }

    public override void Draw(SKCanvas canvas)
    {
    }

    public void Interrupt()
    {
        onAnimationInterrupt?.Invoke();
        Stop();
    }

    public void Start()
    {
        isRunning = true;
        elapsed = 0;
    }

    private float elapsed;

    public override void Update(float deltaTime)
    {
        if (!isRunning) return;

        elapsed += deltaTime * 1000;

        if (elapsed >= animationDuration)
        {
            Stop();

            return;
        }

        float progress = elapsed / animationDuration;

        onAnimationUpdate?.Invoke(progress);
    }

    public void Stop(bool triggerStopEvent = true)
    {
        isRunning = false;

        if (triggerStopEvent)
            onAnimationEnd?.Invoke();
    }
}