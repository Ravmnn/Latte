using SFML.Graphics;

using Latte.Core.Animation;


namespace Latte.Core.Type;


/// <summary>
/// Represents the four channels of color: red, green, blue and alpha (transparency)
/// </summary>
public struct ColorRGBA : IAnimatable<ColorRGBA>
{
    private byte _r;

    public byte R
    {
        readonly get => _r;
        set => _r = System.Math.Clamp(value, (byte)0, (byte)255);
    }


    private byte _g;

    public byte G
    {
        readonly get => _g;
        set => _g = System.Math.Clamp(value, (byte)0, (byte)255);
    }


    private byte _b;

    public byte B
    {
        readonly get => _b;
        set => _b = System.Math.Clamp(value, (byte)0, (byte)255);
    }


    private byte _a;

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


    public static implicit operator Color(ColorRGBA color) => new(color.R, color.G, color.B, color.A);
    public static implicit operator ColorRGBA(Color color) => new(color.R, color.G, color.B, color.A);


    public readonly AnimationState AnimateThis(ColorRGBA to, float time, EasingType easingType = EasingType.Linear)
        => Animate.Color(this, to, time, easingType);
    
    public readonly ColorRGBA AnimationValuesToThis(float[] values)
        => values.ToColor();

    
    public readonly override string ToString()
        => $"rgba({R}, {G}, {B}, {A})";
}