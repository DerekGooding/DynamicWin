using DynamicWin.Main;

namespace DynamicWin.Utils;

public class SecondOrder(Vec2 x0, float f = 2f, float z = 0.4f, float r = 0.1f)
{
    private Vec2 yd = new(0, 0);
    private float k1 = (float)(z / (Math.PI * f));
    private float k2 = (float)(1 / (2 * Math.PI * f * (2 * Math.PI * f)));
    private float k3 = (float)(r * z / (2 * Math.PI * f));

    public void SetValues(float f = 2f, float z = 0.4f, float r = 0.1f)
    {
        k1 = (float)(z / (Math.PI * f));
        k2 = (float)(1 / (2 * Math.PI * f * (2 * Math.PI * f)));
        k3 = (float)(r * z / (2 * Math.PI * f));
    }

    public Vec2 Update(float T, Vec2 x, Vec2? xd = null)
    {
        if (!Settings.AllowAnimation) return x;

        if (xd != null)
        {
            xd = (x - x0) / new Vec2(T, T);
            x0 = x;
        }
        float k2_stable = (float)Math.Max(k2, Math.Max((T * T / 2) + (T * k1 / 2), T * k1));
        x0 += new Vec2(T, T) * yd;
        yd += T * (x + (new Vec2(k3, k3) * xd!) - x0 - (k1 * yd)) / k2_stable;

        x0.X = Mathf.LimitDecimalPoints(x0.X, 1);
        x0.Y = Mathf.LimitDecimalPoints(x0.Y, 1);

        return x0;
    }
}