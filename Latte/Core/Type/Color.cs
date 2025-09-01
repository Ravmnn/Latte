using System.Numerics;
using Latte.Tweening;
using SFML.Graphics;


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


    public ColorRGBA Get() => this;


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


public static class ColorGenerator
{
    public static ColorRGBA FromIndex(uint index, int step = 1)
    {
        int[] color = [255, 0, 0];
        uint channelIndex = 1;

        var decreasePrevious = false;

        for (uint i = 0; i < index; i++)
        {
            var realChannelIndex = RelativateChannelIndex(channelIndex);
            ref var channel = ref color[realChannelIndex];
            ref var previousChannel = ref color[GetPreviousChannelIndex(realChannelIndex)];

            if (decreasePrevious)
                previousChannel -= step;
            else
                channel += step;

            if (decreasePrevious && previousChannel.IsChannelMined())
            {
                decreasePrevious = false;
                previousChannel = 0;
                channelIndex++;
            }

            else if (channel.IsChannelMaxed())
            {
                channel = 255;
                decreasePrevious = true;
            }
        }

        return new ColorRGBA((byte)color[0], (byte)color[1], (byte)color[2]);
    }

    private static bool IsChannelMaxed(this int channel) => channel >= 255;
    private static bool IsChannelMined(this int channel) => channel <= 0;

    private static uint RelativateChannelIndex(uint index)
    {
        while (index >= 3)
            index -= 3;

        return index;
    }

    private static uint GetNextChannelIndex(uint index) => index >= 2 ? 0 : index + 1;
    private static uint GetPreviousChannelIndex(uint index) => index == 0 ? 2 : index - 1;
}
