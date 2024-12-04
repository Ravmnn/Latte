using System.Numerics;

using SFML.Graphics;

using Latte.Core.Animation;


namespace Latte.Core.Type;


/// <summary>
/// Represents the four channels of color: red, green, blue and alpha (transparency)
/// </summary>
public struct ColorRGBA : IAnimatable<ColorRGBA>,
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


    public static implicit operator Color(ColorRGBA color) => new(color.R, color.G, color.B, color.A);
    public static implicit operator ColorRGBA(Color color) => new(color.R, color.G, color.B, color.A);


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


    public AnimationData AnimateThis(ColorRGBA to, double time, Easing easing = Easing.Linear)
        => Animate.Color(this, to, time, easing);
    
    
    public readonly IAnimatable AnimationValuesToThis(float[] values)
        => values.ToColor();

    
    public readonly override string ToString()
        => $"rgba({R}, {G}, {B}, {A})";
}