using System;

using SFML.System;
using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Core;


public static class Math
{
    public static bool IsPointOverRoundedRect(Vec2f point, Vec2f position, Vec2f size, float radius)
    {
        Vec2f leftTopCenter = new(position.X + radius, position.Y + radius);
        Vec2f rightTopCenter = new(position.X + size.X - radius, position.Y + radius);
        Vec2f leftBottomCenter = new(position.X + radius, position.Y + size.Y - radius);
        Vec2f rightBottomCenter = new(position.X + size.X - radius, position.Y + size.Y - radius);

        FloatRect horizontalRect = new(new(position.X, position.Y + radius), new(size.X, size.Y - radius * 2));
        FloatRect verticalRect = new(new(position.X + radius, position.Y), new(size.X - radius * 2, size.Y));

        bool overCorners = IsPointOverCircle(point, leftTopCenter, radius) ||
                           IsPointOverCircle(point, rightTopCenter, radius) ||
                           IsPointOverCircle(point, leftBottomCenter, radius) ||
                           IsPointOverCircle(point, rightBottomCenter, radius);
        
        bool overRects = horizontalRect.Contains((Vector2f)point) || verticalRect.Contains((Vector2f)point);
        
        return overCorners || overRects;
    }
    
    
    public static bool IsPointOverCircle(Vector2f point, Vector2f circleCenter, float radius)
        => Distance(point, circleCenter) <= radius;


    public static float Distance(float x1, float y1, float x2, float y2)
        => MathF.Sqrt(MathF.Pow(x2 - x1, 2f) + MathF.Pow(y2 - y1, 2f));
    
    public static float Distance(Vector2f p1, Vector2f p2)
        => Distance(p1.X, p1.Y, p2.X, p2.Y);
}