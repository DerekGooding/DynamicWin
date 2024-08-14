namespace DynamicWin.Utils;

public static class Easing
{
    public static float EaseInQuint(float x) => x * x * x * x * x;

    public static float EaseOutQuint(float x) => 1 - (float)Math.Pow(1 - x, 5);

    public static float EaseOutSin(float x) => (float)Math.Sin((x * Math.PI) / 2);

    public static float EaseInSin(float x) => 1 - (float)Math.Cos((x * Math.PI) / 2);

    public static float EaseOutCubic(float x) => 1 - (float)Math.Pow(1 - x, 3);

    public static float EaseInCubic(float x) => x * x * x;
}