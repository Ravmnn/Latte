using System;

using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Elements;


[Flags]
public enum Alignments 
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
    Vec2f GetAlignmentPosition(Alignments alignment);
}


public static class AlignmentCalculator
{
    public static Vec2f GetAlignedPositionOfChild(FloatRect child, FloatRect parent, Alignments alignment)
    {
        Vec2f position = child.Position;

        if (alignment.HasFlag(Alignments.Top))
            position.Y = parent.Top;
        
        else if (alignment.HasFlag(Alignments.Bottom))
            position.Y = parent.Top + parent.Height - child.Height;
        
        if (alignment.HasFlag(Alignments.Left))
            position.X = parent.Left;
        
        else if (alignment.HasFlag(Alignments.Right))
            position.X = parent.Left + parent.Width - child.Width;

        if (alignment.HasFlag(Alignments.HorizontalCenter))
            position.X = parent.Left + parent.Width / 2f - child.Width / 2f;
        
        if (alignment.HasFlag(Alignments.VerticalCenter))
            position.Y = parent.Top + parent.Height / 2f - child.Height / 2f;
        
        return position;
    }
}