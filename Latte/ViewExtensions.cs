using SFML.System;
using SFML.Graphics;


namespace Latte;


public static class ViewExtensions
{
    public static void MoveToPosition(this View view, Vector2f position)
        => view.Center = new(position.X + view.Size.X / 2, position.Y + view.Size.Y / 2);
    
    
    public static FloatRect ViewToRect(this View view)
    {
        Vector2f position = view.Center - view.Size / 2;
        Vector2f size = view.Size;

        return new(position, size);
    }
    

    public static bool IsRectVisibleToView(this View view, FloatRect rect)
        => rect.Intersects(view.ViewToRect());
}