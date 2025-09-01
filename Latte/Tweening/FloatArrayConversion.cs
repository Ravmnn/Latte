using SFML.System;
using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Tweening;


/// <summary>
/// Converts a float[] to a specific data structure.
/// </summary>
public static class FloatArrayConversion
{
    public static float ToValue(this float[] values)
        => values[0];


    public static Vec2f ToVec2f(this float[] values) => new Vec2f(values[0], values[1]);
    public static Vec2i ToVec2i(this float[] values) => new Vec2i((int)values[0], (int)values[1]);
    public static Vec2u ToVec2u(this float[] values) => new Vec2u((uint)values[0], (uint)values[1]);

    // forces the values to be between 0 and 255
    public static ColorRGBA ToColor(this float[] values) =>
        new ColorRGBA((byte)System.Math.Clamp(values[0], 0, 255), (byte)System.Math.Clamp(values[1], 0, 255),
            (byte)System.Math.Clamp(values[2], 0, 255), (byte)System.Math.Clamp(values[3], 0, 255));


    public static Vector2f ToVector2f(this float[] values) => ToVec2f(values);
    public static Vector2i ToVector2i(this float[] values) => ToVec2i(values);
    public static Vector2u ToVector2u(this float[] values) => ToVec2u(values);

    public static Color ToSFColor(this float[] values) => ToColor(values);
}
