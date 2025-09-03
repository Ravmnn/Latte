using System;

using SFML.System;
using SFML.Graphics;

using Latte.Core.Type;
using Latte.Application.Elements.Behavior;
using Latte.Application.Elements.Primitives;


namespace Latte.Core;


public static class Point
{
    public static bool IsPointOverObject(this Vec2f point, BaseObject @object)
        => (@object as IClickable)?.IsPointOver(point) ?? point.IsPointOverObjectBounds(@object);

    public static bool IsPointOverObjectBounds(this Vec2f point, BaseObject @object)
        => point.IsPointOverRect(@object.GetBounds());

    public static bool IsPointOverElement(this Vec2f point, Element element)
        => point.IsPointOverObjectBounds(element) && point.IsPointOverElementClipArea(element);

    public static bool IsPointOverElementClipArea(this Vec2f point, Element element)
        => point.IsPointOverRect(element.GetIntersectedClipArea().ToWorldCoordinates());


    public static bool IsPointOverRect(this Vec2f point, Vec2f position, Vec2f size)
        => new FloatRect(position, size).Contains((Vector2f)point);

    public static bool IsPointOverRect(this Vec2f point, FloatRect rect)
        => IsPointOverRect(point, rect.Position, rect.Size);


    public static bool IsPointOverRoundedRect(this Vec2f point, Vec2f position, Vec2f size, float radius)
    {
        var leftTopCenter = new Vec2f(position.X + radius, position.Y + radius);
        var rightTopCenter = new Vec2f(position.X + size.X - radius, position.Y + radius);
        var leftBottomCenter = new Vec2f(position.X + radius, position.Y + size.Y - radius);
        var rightBottomCenter = new Vec2f(position.X + size.X - radius, position.Y + size.Y - radius);

        var horizontalRect = new FloatRect(new Vector2f(position.X, position.Y + radius), new Vector2f(size.X, size.Y - radius * 2));
        var verticalRect = new FloatRect(new Vector2f(position.X + radius, position.Y), new Vector2f(size.X - radius * 2, size.Y));

        var overCorners = IsPointOverCircle(point, leftTopCenter, radius) ||
                          IsPointOverCircle(point, rightTopCenter, radius) ||
                          IsPointOverCircle(point, leftBottomCenter, radius) ||
                          IsPointOverCircle(point, rightBottomCenter, radius);

        var overRects = horizontalRect.Contains((Vector2f)point) || verticalRect.Contains((Vector2f)point);

        return overCorners || overRects;
    }

    public static bool IsPointOverRoundedRect(this Vec2f point, FloatRect rect, float radius)
        => IsPointOverRoundedRect(point, rect.Position, rect.Size, radius);


    public static bool IsPointOverCircle(this Vec2f point, Vec2f circleCenter, float radius)
        => Distance(point, circleCenter) <= radius;


    public static float Distance(float x1, float y1, float x2, float y2)
        => MathF.Sqrt(MathF.Pow(x2 - x1, 2f) + MathF.Pow(y2 - y1, 2f));

    public static float Distance(this Vec2f p1, Vec2f p2)
        => Distance(p1.X, p1.Y, p2.X, p2.Y);


    public static Vec2f Round(this Vec2f vec) => new Vec2f(MathF.Round(vec.X), MathF.Round(vec.Y));
}
