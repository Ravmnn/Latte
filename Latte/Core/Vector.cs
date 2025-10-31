using System;

using Latte.Core.Type;


namespace Latte.Core;




public static class Vector
{
    public static double Length(Vec2f vec)
        => Math.Sqrt(Dot(vec, vec));


    public static Vec2f Normalized(Vec2f vec)
        => vec * (float)(1f / Length(vec));


    public static double Dot(Vec2f a, Vec2f b)
        => a.X * b.X + a.Y * b.Y;


    public static double Cross(Vec2f a, Vec2f b)
        => a.X * b.Y - a.Y * b.X;
}




public static class VectorF
{
    public static float Length(Vec2f vec)
        => MathF.Sqrt(Dot(vec, vec));


    public static Vec2f Normalized(Vec2f vec)
        => vec * (1f / Length(vec));


    public static float Dot(Vec2f a, Vec2f b)
        => a.X * b.X + a.Y * b.Y;


    public static float Cross(Vec2f a, Vec2f b)
        => a.X * b.Y - a.Y * b.X;
}
