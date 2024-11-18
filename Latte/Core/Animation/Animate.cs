using SFML.System;
using SFML.Graphics;


namespace Latte.Core.Animation;


/// <summary>
/// Creates an AnimationState to animate a specific data strucute.
/// </summary>
public static class Animate
{
    public static AnimationState Value(float from, float to, float time, EasingType easingType = EasingType.Linear) =>
        new([from], [to], time, easingType);


    public static AnimationState Vector2f(Vector2f from, Vector2f to, float time, EasingType easingType = EasingType.Linear)
        => new([from.X, from.Y], [to.X, to.Y], time, easingType);

    public static AnimationState Vector2i(Vector2i from, Vector2i to, float time, EasingType easingType = EasingType.Linear)
        => new([from.X, from.Y], [to.X, to.Y], time, easingType);

    public static AnimationState Vector2u(Vector2u from, Vector2u to, float time, EasingType easingType = EasingType.Linear)
        => new([from.X, from.Y], [to.X, to.Y], time, easingType);


    public static AnimationState SFColor(Color from, Color to, float time, EasingType easingType = EasingType.Linear)
        => new([from.R, from.G, from.B, from.A], [to.R, to.G, to.B, to.A], time, easingType);
}


/// <summary>
/// Converts a float[] to a specific data structure.
/// </summary>
public static class FloatArrayConverterExtensions
{
    public static float ToValue(this float[] values)
        => values[0];
    
    
    public static Vector2f ToVector2f(this float[] values)
        => new(values[0], values[1]);

    public static Vector2i ToVector2i(this float[] values)
        => new((int)values[0], (int)values[1]);

    public static Vector2u ToVector2u(this float[] values)
        => new((uint)values[0], (uint)values[1]);


    public static Color ToColor(this float[] values)
        => new((byte)System.Math.Clamp(values[0], 0, 255), (byte)System.Math.Clamp(values[1], 0, 255),
            (byte)System.Math.Clamp(values[2], 0, 255), (byte)System.Math.Clamp(values[3], 0, 255));
}