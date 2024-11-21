using System;

using SFML.Graphics;

using Latte.Core.Type;
using Latte.Core.Application;


namespace Latte.Core;


[Flags]
public enum Corners
{
    None,
    
    Top = 1 << 0,
    Left = 1 << 1,
    Right = 1 << 2,
    Bottom = 1 << 3,
    
    TopLeft = Top | Left,
    TopRight = Top | Right,
    BottomLeft = Bottom | Left,
    BottomRight = Bottom | Right
}


public static class RectExtensions
{
    public static IntRect ToWindowCoordinates(this FloatRect rect)
    {
        Vec2i transformedPosition = App.Window.MapCoordsToPixel(rect.Position);
        Vec2i transformedSize = App.Window.MapCoordsToPixel(rect.Position + rect.Size) - transformedPosition;
        
        return new(transformedPosition, transformedSize);
    }


    public static FloatRect ToWorldCoordinates(this IntRect rect)
    {
        Vec2f transformedPosition = App.Window.MapPixelToCoords(rect.Position);
        Vec2f transformedSize = App.Window.MapPixelToCoords(rect.Position + rect.Size) - transformedPosition;
        
        return new(transformedPosition, transformedSize);
    }
}