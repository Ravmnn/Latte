using System.Numerics;

using SFML.Graphics;

using Latte.Tweening;


namespace Latte.Core.Type;


/// <summary>
/// Represents the four channels of color: red, green, blue and alpha (transparency)
/// </summary>
public struct ColorRGBA : IFloatArrayModifiable,
    IAdditionOperators<ColorRGBA, byte, ColorRGBA>,
    ISubtractionOperators<ColorRGBA, byte, ColorRGBA>
{
    private byte _r;
    private byte _g;
    private byte _b;
    private byte _a;


    public byte R
    {
        readonly get => _r;
        set => _r = System.Math.Clamp(value, (byte)0, (byte)255);
    }

    public byte G
    {
        readonly get => _g;
        set => _g = System.Math.Clamp(value, (byte)0, (byte)255);
    }

    public byte B
    {
        readonly get => _b;
        set => _b = System.Math.Clamp(value, (byte)0, (byte)255);
    }

    public byte A
    {
        readonly get => _a;
        set => _a = System.Math.Clamp(value, (byte)0, (byte)255);
    }


    public ColorRGBA(byte red, byte green, byte blue, byte alpha = 255)
    {
        R = red;
        G = green;
        B = blue;
        A = alpha;
    }


    public ColorRGBA(NormalizedColorRGBA color)
        : this((byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255), (byte)(color.A * 255))
    {}


    public void ModifyFrom(float[] values)
        => (R, G, B, A) = ((byte)values[0], (byte)values[1], (byte)values[2], (byte)values[3]);


    public static implicit operator Color(ColorRGBA color) => new Color(color.R, color.G, color.B, color.A);
    public static implicit operator ColorRGBA(Color color) => new ColorRGBA(color.R, color.G, color.B, color.A);


    public static ColorRGBA operator+(ColorRGBA left, byte right)
    {
        left.R += right;
        left.G += right;
        left.B += right;

        return left;
    }

    public static ColorRGBA operator-(ColorRGBA left, byte right)
    {
        left.R -= right;
        left.G -= right;
        left.B -= right;

        return left;
    }


    public readonly override string ToString() => $"rgba({R}, {G}, {B}, {A})";
}
