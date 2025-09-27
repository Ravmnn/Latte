using System.Numerics;

using Latte.Tweening;


using Color = SFML.Graphics.Color;


namespace Latte.Core.Type;




public struct NormalizedColorRGBA : IFloatArrayModifiable,
    IAdditionOperators<NormalizedColorRGBA, NormalizedColorRGBA, NormalizedColorRGBA>,
    ISubtractionOperators<NormalizedColorRGBA, NormalizedColorRGBA, NormalizedColorRGBA>,
    IMultiplyOperators<NormalizedColorRGBA, NormalizedColorRGBA, NormalizedColorRGBA>,
    IDivisionOperators<NormalizedColorRGBA, NormalizedColorRGBA, NormalizedColorRGBA>
{
    private float _r;
    public float R
    {
        readonly get => _r;
        set => _r = System.Math.Clamp(value, 0, 1);
    }


    private float _g;
    public float G
    {
        readonly get => _g;
        set => _g = System.Math.Clamp(value, 0, 1);
    }


    private float _b;
    public float B
    {
        readonly get => _b;
        set => _b = System.Math.Clamp(value, 0, 1);
    }


    private float _a;
    public float A
    {
        readonly get => _a;
        set => _a = System.Math.Clamp(value, 0, 1);
    }




    public NormalizedColorRGBA(float red, float green, float blue, float alpha = 1)
    {
        R = red;
        G = green;
        B = blue;
        A = alpha;
    }


    public NormalizedColorRGBA(ColorRGBA color)
        : this((float)color.R / 255, (float)color.G / 255, (float)color.B / 255, (float)color.A / 255)
    {}




    public void ModifyFrom(float[] values)
        => (R, G, B, A) = (values[0], values[1], values[2], values[3]);




    public static implicit operator Color(NormalizedColorRGBA color) => (ColorRGBA)color;
    public static implicit operator NormalizedColorRGBA(Color color) => (ColorRGBA)color;

    public static implicit operator NormalizedColorRGBA(ColorRGBA color) => new NormalizedColorRGBA(color);
    public static implicit operator ColorRGBA(NormalizedColorRGBA color) => new ColorRGBA(color);




    public static NormalizedColorRGBA operator +(NormalizedColorRGBA left, NormalizedColorRGBA right)
    {
        left.R += right.R;
        left.G += right.G;
        left.B += right.B;
        left.A += right.A;

        return left;
    }

    public static NormalizedColorRGBA operator -(NormalizedColorRGBA left, NormalizedColorRGBA right)
    {
        left.R -= right.R;
        left.G -= right.G;
        left.B -= right.B;
        left.A -= right.A;

        return left;
    }


    public static NormalizedColorRGBA operator *(NormalizedColorRGBA left, NormalizedColorRGBA right)
    {
        left.R *= right.R;
        left.G *= right.G;
        left.B *= right.B;
        left.A *= right.A;

        return left;
    }


    public static NormalizedColorRGBA operator /(NormalizedColorRGBA left, NormalizedColorRGBA right)
    {
        left.R /= right.R;
        left.G /= right.G;
        left.B /= right.B;
        left.A /= right.A;

        return left;
    }




    public readonly override string ToString() => $"normalized rgba({R}, {G}, {B}, {A})";
}
