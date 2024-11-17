using System;

using SFML.System;
using SFML.Graphics;


namespace Latte.Core;


public static class Math
{
    public static bool IsPointOverRoundedRect(Vector2f point, Vector2f position, Vector2f size, float radius)
    {
        Vector2f ltcenter = new(position.X + radius, position.Y + radius);
        Vector2f rtcenter = new(position.X + size.X - radius, position.Y + radius);
        Vector2f lbcenter = new(position.X + radius, position.Y + size.Y - radius);
        Vector2f rbcenter = new(position.X + size.X - radius, position.Y + size.Y - radius);

        FloatRect hrect = new(new(position.X, position.Y + radius), new(size.X, size.Y - radius * 2));
        FloatRect vrect = new(new(position.X + radius, position.Y), new(size.X - radius * 2, size.Y));

        bool overCorners = IsPointOverCircle(point, ltcenter, radius) ||
                           IsPointOverCircle(point, rtcenter, radius) ||
                           IsPointOverCircle(point, lbcenter, radius) ||
                           IsPointOverCircle(point, rbcenter, radius);
        
        bool overRects = hrect.Contains(point) || vrect.Contains(point);
        
        return overCorners || overRects;
    }
    
    
    public static bool IsPointOverCircle(Vector2f point, Vector2f circleCenter, float radius)
        => Distance(point, circleCenter) <= radius;


    public static float Distance(float x1, float y1, float x2, float y2)
        => MathF.Sqrt(MathF.Pow(x2 - x1, 2f) + MathF.Pow(y2 - y1, 2f));
    
    public static float Distance(Vector2f p1, Vector2f p2)
        => Distance(p1.X, p1.Y, p2.X, p2.Y);
}