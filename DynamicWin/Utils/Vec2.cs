namespace DynamicWin.Utils;

public class Vec2
{
    private float y;

    public float X { get; set; }
    public float Y { get => y; set => y = value; }

    public static Vec2 zero => new Vec2(0, 0);
    public static Vec2 one => new Vec2(1, 1);

    public float Magnitude => (Math.Abs(X) + Math.Abs(y)) / 2;

    public Vec2(float x, float y)
    {
        this.X = x;
        this.y = y;
    }

    public Vec2(double x, double y)
    {
        this.X = (float)x;
        this.y = (float)y;
    }

    public static float Distance(Vec2 v1, Vec2 v2)
    {
        return (float)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.y - v1.y, 2));
    }

    public Vec2 Normalized()
    {
        float length = (float)Math.Sqrt(X * X + y * y);

        if (length == 0)
        {
            return Vec2.zero;
        }

        return new Vec2(X / length, y / length);
    }

    public static Vec2 lerp(Vec2 a, Vec2 b, float t)
    {
        return new Vec2(Mathf.Lerp(a.X, b.X, t), Mathf.Lerp(a.y, b.y, t));
    }

    // Operators

    // Vec2 and Vec2

    public static Vec2 operator +(Vec2 a, Vec2 b)
    {
        if (a == null && b != null) return b;
        else if (a != null && b == null) return a;
        else if (a == null && b == null) return Vec2.zero;

        return new Vec2(a.X + b.X, a.y + b.y);
    }

    public static Vec2 operator *(Vec2 a, Vec2 b)
    {
        if (a == null && b != null) return b;
        else if (a != null && b == null) return a;
        else if (a == null && b == null) return Vec2.zero;

        return new Vec2(a.X * b.X, a.y * b.y);
    }

    public static Vec2 operator /(Vec2 a, Vec2 b)
    {
        if (a == null && b != null) return b;
        else if (a != null && b == null) return a;
        else if (a == null && b == null) return Vec2.zero;

        return new Vec2(a.X / b.X, a.y / b.y);
    }

    public static Vec2 operator -(Vec2 a, Vec2 b)
    {
        if (a == null && b != null) return b;
        else if (a != null && b == null) return a;
        else if (a == null && b == null) return Vec2.zero;

        return new Vec2(a.X - b.X, a.y - b.y);
    }

    // Vec2 and float

    public static Vec2 operator +(Vec2 a, float b)
    {
        return new Vec2(a.X + b, a.y + b);
    }

    public static Vec2 operator *(Vec2 a, float b)
    {
        return new Vec2(a.X * b, a.y * b);
    }

    public static Vec2 operator /(Vec2 a, float b)
    {
        return new Vec2(a.X / b, a.y / b);
    }

    public static Vec2 operator -(Vec2 a, float b)
    {
        return new Vec2(a.X - b, a.y - b);
    }

    // The other way around (Needed for some reason, WTF CSharp?)

    public static Vec2 operator +(float b, Vec2 a)
    {
        return new Vec2(a.X + b, a.y + b);
    }

    public static Vec2 operator *(float b, Vec2 a)
    {
        return new Vec2(a.X * b, a.y * b);
    }

    public static Vec2 operator /(float b, Vec2 a)
    {
        return new Vec2(a.X / b, a.y / b);
    }

    public static Vec2 operator -(float b, Vec2 a)
    {
        return new Vec2(a.X - b, a.y - b);
    }
}