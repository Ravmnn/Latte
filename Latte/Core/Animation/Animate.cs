using SFML.System;
using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Core.Animation;


/// <summary>
/// Creates an AnimationState to animate a specific data strucute.
/// </summary>
public static class Animate
{
    public static AnimationState Value(float from, float to, float time, EasingType easingType = EasingType.Linear) =>
        new([from], [to], time, easingType);
    
    
    public static AnimationState Vec2f(Vec2f from, Vec2f to, float time, EasingType easingType = EasingType.Linear)
        => new([from.X, from.Y], [to.X, to.Y], time, easingType);
    
    public static AnimationState Vec2i(Vec2i from, Vec2i to, float time, EasingType easingType = EasingType.Linear)
        => new([from.X, from.Y], [to.X, to.Y], time, easingType);
    
    public static AnimationState Vec2u(Vec2u from, Vec2u to, float time, EasingType easingType = EasingType.Linear)
        => new([from.X, from.Y], [to.X, to.Y], time, easingType);

    
    public static AnimationState Color(ColorRGBA from, ColorRGBA to, float time, EasingType easingType = EasingType.Linear)
        => new([from.R, from.G, from.B, from.A], [to.R, to.G, to.B, to.A], time, easingType);
    
    

    public static AnimationState Vector2f(Vector2f from, Vector2f to, float time, EasingType easingType = EasingType.Linear)
        => Vec2f(from, to, time, easingType);

    public static AnimationState Vector2i(Vector2i from, Vector2i to, float time, EasingType easingType = EasingType.Linear)
        => Vec2i(from, to, time, easingType);

    public static AnimationState Vector2u(Vector2u from, Vector2u to, float time, EasingType easingType = EasingType.Linear)
        => Vec2u(from, to, time, easingType);


    public static AnimationState SFColor(Color from, Color to, float time, EasingType easingType = EasingType.Linear)
        => Color(from, to, time, easingType);
}


/// <summary>
/// Converts a float[] to a specific data structure.
/// </summary>
public static class FloatArrayConverterExtensions
{
    public static float ToValue(this float[] values)
        => values[0];
    
    
    public static Vec2f ToVec2f(this float[] values) => new(values[0], values[1]);
    public static Vec2i ToVec2i(this float[] values) => new((int)values[0], (int)values[1]);
    public static Vec2u ToVec2u(this float[] values) => new((uint)values[0], (uint)values[1]);

    // forces the values to be between 0 and 255
    public static ColorRGBA ToColor(this float[] values)
        => new((byte)System.Math.Clamp(values[0], 0, 255), (byte)System.Math.Clamp(values[1], 0, 255),
            (byte)System.Math.Clamp(values[2], 0, 255), (byte)System.Math.Clamp(values[3], 0, 255));


    public static Vector2f ToVector2f(this float[] values) => ToVec2f(values);
    public static Vector2i ToVector2i(this float[] values) => ToVec2i(values);
    public static Vector2u ToVector2u(this float[] values) => ToVec2u(values);

    public static Color ToSFColor(this float[] values) => ToColor(values);
}