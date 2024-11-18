using System;

using SFML.System;
using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Core;


[Flags]
public enum AlignmentType
{
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
    Vec2f GetAlignmentPosition(AlignmentType alignment);
}


public static class AlignmentCalculator
{
    public static Vec2f GetAlignedPositionOfChild(FloatRect child, FloatRect parent, AlignmentType alignment)
    {
        Vec2f position = new();

        if (alignment.HasFlag(AlignmentType.Top))
            position.Y = parent.Top;
        
        else if (alignment.HasFlag(AlignmentType.Bottom))
            position.Y = parent.Top + parent.Height - child.Height;
        
        if (alignment.HasFlag(AlignmentType.Left))
            position.X = parent.Left;
        
        else if (alignment.HasFlag(AlignmentType.Right))
            position.X = parent.Left + parent.Width - child.Width;

        if (alignment.HasFlag(AlignmentType.HorizontalCenter))
            position.X = parent.Left + parent.Width / 2f - child.Width / 2f;
        
        if (alignment.HasFlag(AlignmentType.VerticalCenter))
            position.Y = parent.Top + parent.Height / 2f - child.Height / 2f;
        
        return position;
    }
}