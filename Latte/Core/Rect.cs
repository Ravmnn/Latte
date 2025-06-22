using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;

using Latte.Core.Type;
using Latte.Core.Application;


namespace Latte.Core;


[Flags]
public enum Corner
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


public record struct FloatRectVertices(Vec2f TopLeft, Vec2f TopRight, Vec2f BottomLeft, Vec2f BottomRight)
{
    public FloatRectVertices(FloatRect rect) : this(
        rect.Position,
        new Vec2f(rect.Left + rect.Width, rect.Top),
        new Vec2f(rect.Left, rect.Top + rect.Height),
        rect.Position + rect.Size
    ) {}


    public FloatRectVertices() : this(new FloatRect())
    {}


    public static implicit operator FloatRect(FloatRectVertices vertices) => vertices.VerticesToRect();
    public static implicit operator FloatRectVertices(FloatRect rect) => rect.RectToVertices();
}


public static class RectExtensions
{
    public static FloatRect VerticesToRect(this FloatRectVertices vertices) =>
        new FloatRect(vertices.TopLeft, vertices.BottomRight - vertices.TopLeft);

    public static FloatRectVertices RectToVertices(this FloatRect rect) => new FloatRectVertices(rect);


    public static IntRect ToWindowCoordinates(this FloatRect rect)
    {
        Vec2i transformedPosition = App.Window.MapCoordsToPixel(rect.Position);
        Vec2i transformedSize = App.Window.MapCoordsToPixel(rect.Position + rect.Size) - transformedPosition;

        return new IntRect(transformedPosition, transformedSize);
    }


    public static FloatRect ToWorldCoordinates(this IntRect rect)
    {
        Vec2f transformedPosition = App.Window.MapPixelToCoords(rect.Position);
        Vec2f transformedSize = App.Window.MapPixelToCoords(rect.Position + rect.Size) - transformedPosition;

        return new FloatRect(transformedPosition, transformedSize);
    }


    public static FloatRect ShrinkRect(this FloatRect rect, Vec2f amount)
    {
        rect.Top += amount.Y;
        rect.Left += amount.X;
        rect.Width -= amount.X * 2;
        rect.Height -= amount.Y * 2;

        return rect;
    }

    public static FloatRect ShrinkRect(this FloatRect rect, float amount)
        => ShrinkRect(rect, new Vec2f(amount, amount));


    public static FloatRect ExpandRect(this FloatRect rect, Vec2f amount)
        => rect.ShrinkRect(-amount);

    public static FloatRect ExpandRect(this FloatRect rect, float amount)
        => ExpandRect(rect, new Vec2f(amount, amount));


    public static FloatRect GetBoundsOfRects(this IEnumerable<FloatRect> rects)
    {
        var rectArray = rects.ToArray();

        if (rectArray.Length == 0)
            return new FloatRect();

        var bounds = rectArray[0];

        foreach (var rect in rectArray)
        {
            if (rect.Left < bounds.Left)
                bounds.Left = rect.Left;

            if (rect.Top < bounds.Top)
                bounds.Top = rect.Top;

            if (rect.Left + rect.Width > bounds.Width)
                bounds.Width = rect.Left + rect.Width - bounds.Left;

            if (rect.Top + rect.Height > bounds.Height)
                bounds.Height = rect.Top + rect.Height - bounds.Top;
        }

        return bounds;
    }
}
