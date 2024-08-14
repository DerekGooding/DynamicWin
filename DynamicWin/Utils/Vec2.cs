namespace DynamicWin.Utils;

public class Vec2
{

    public float X { get; set; }
    public float Y { get; set; }

    public static Vec2 zero => new(0, 0);
    public static Vec2 one => new(1, 1);

    public float Magnitude => (Math.Abs(X) + Math.Abs(Y)) / 2;

    public Vec2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public Vec2(double x, double y)
    {
        X = (float)x;
        Y = (float)y;
    }

    public static float Distance(Vec2 v1, Vec2 v2) => (float)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2));

    public Vec2 Normalized()
    {
        float length = (float)Math.Sqrt(X * X + Y * Y);

        if (length == 0)
        {
            return zero;
        }

        return new Vec2(X / length, Y / length);
    }

    public static Vec2 Lerp(Vec2 a, Vec2 b, float t) => new(Mathf.Lerp(a.X, b.X, t), Mathf.Lerp(a.Y, b.Y, t));

    // Operators

    // Vec2 and Vec2

    public static Vec2 operator +(Vec2 a, Vec2 b)
    {
        if (a == null && b != null) return b;
        else if (a != null && b == null) return a;
        else if (a == null && b == null) return zero;

        return new Vec2(a.X + b.X, a.Y + b.Y);
    }

    public static Vec2 operator *(Vec2 a, Vec2 b)
    {
        if (a == null && b != null) return b;
        else if (a != null && b == null) return a;
        else if (a == null && b == null) return zero;

        return new Vec2(a.X * b.X, a.Y * b.Y);
    }

    public static Vec2 operator /(Vec2 a, Vec2 b)
    {
        if (a == null && b != null) return b;
        else if (a != null && b == null) return a;
        else if (a == null && b == null) return zero;

        return new Vec2(a.X / b.X, a.Y / b.Y);
    }

    public static Vec2 operator -(Vec2 a, Vec2 b)
    {
        if (a == null && b != null) return b;
        else if (a != null && b == null) return a;
        else if (a == null && b == null) return zero;

        return new Vec2(a.X - b.X, a.Y - b.Y);
    }

    // Vec2 and float

    public static Vec2 operator +(Vec2 a, float b) => new(a.X + b, a.Y + b);

    public static Vec2 operator *(Vec2 a, float b) => new(a.X * b, a.Y * b);

    public static Vec2 operator /(Vec2 a, float b) => new(a.X / b, a.Y / b);

    public static Vec2 operator -(Vec2 a, float b) => new(a.X - b, a.Y - b);

    // The other way around (Needed for some reason, WTF CSharp?)

    public static Vec2 operator +(float b, Vec2 a) => new(a.X + b, a.Y + b);

    public static Vec2 operator *(float b, Vec2 a) => new(a.X * b, a.Y * b);

    public static Vec2 operator /(float b, Vec2 a) => new(a.X / b, a.Y / b);

    public static Vec2 operator -(float b, Vec2 a) => new(a.X - b, a.Y - b);
}