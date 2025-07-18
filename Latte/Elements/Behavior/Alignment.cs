using System;

using Latte.Core.Type;

using SFML.Graphics;


namespace Latte.Elements.Behavior;


[Flags]
public enum Alignment
{
    None = 0,

    Top = 1 << 0,
    Bottom = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,

    TopLeft = Top | Left,
    TopRight = Top | Right,
    BottomLeft = Bottom | Left,
    BottomRight = Bottom | Right,

    HorizontalCenter = 1 << 4,
    VerticalCenter = 1 << 5,

    Center = HorizontalCenter | VerticalCenter
}


public interface IAlignable
{
    Vec2f GetAlignmentPosition(Alignment alignment);
}


public static class AlignmentCalculator
{
    public static Vec2f GetAlignedPositionOfChild(FloatRect child, FloatRect parent, Alignment alignment)
    {
        Vec2f position = child.Position;

        if (alignment.HasFlag(Alignment.Top))
            position.Y = parent.Top;

        else if (alignment.HasFlag(Alignment.Bottom))
            position.Y = parent.Top + parent.Height - child.Height;

        if (alignment.HasFlag(Alignment.Left))
            position.X = parent.Left;

        else if (alignment.HasFlag(Alignment.Right))
            position.X = parent.Left + parent.Width - child.Width;

        if (alignment.HasFlag(Alignment.HorizontalCenter))
            position.X = parent.Left + parent.Width / 2f - child.Width / 2f;

        if (alignment.HasFlag(Alignment.VerticalCenter))
            position.Y = parent.Top + parent.Height / 2f - child.Height / 2f;

        return position;
    }


    public static Vec2f GetAlignedRelativePositionOfChild(FloatRect child, FloatRect parent, Alignment alignment)
    {
        Vec2f position = child.Position;

        if (alignment.HasFlag(Alignment.Top))
            position.Y = 0;

        else if (alignment.HasFlag(Alignment.Bottom))
            position.Y = parent.Height - child.Height;

        if (alignment.HasFlag(Alignment.Left))
            position.X = 0;

        else if (alignment.HasFlag(Alignment.Right))
            position.X = parent.Width - child.Width;

        if (alignment.HasFlag(Alignment.HorizontalCenter))
            position.X = parent.Width / 2f - child.Width / 2f;

        if (alignment.HasFlag(Alignment.VerticalCenter))
            position.Y = parent.Height / 2f - child.Height / 2f;

        return position;
    }


    // text local bounds work quite different
    // https://learnsfml.com/basics/graphics/how-to-center-text/#set-a-string

    public static Vec2f GetTextAlignedPositionOfChild(Text text, FloatRect parent, Alignment alignment)
        => GetAlignedPositionOfChild(text.GetGlobalBounds(), parent, alignment) - text.GetLocalBounds().Position;

    public static Vec2f GetTextAlignedRelativePositionOfChild(Text text, FloatRect parent, Alignment alignment)
        => GetAlignedRelativePositionOfChild(text.GetGlobalBounds(), parent, alignment) - text.GetLocalBounds().Position;
}
