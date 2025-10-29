using System;

using Latte.Core.Type;


namespace Latte.Core;




public static class Vector
{
    public static float Length(this Vec2f vec)
        => MathF.Sqrt(vec.Dot(vec));


    public static Vec2f Normalized(this Vec2f vec)
        => vec * (1f / vec.Length());


    public static float Dot(this Vec2f a, Vec2f b)
        => a.X * b.X + a.Y * b.Y;


    public static float Cross(this Vec2f a, Vec2f b)
        => a.X * b.Y - a.Y * b.X;
}
