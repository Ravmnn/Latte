using SFML.Graphics;


namespace Latte.Application;


public static class Debug
{
    public static void DrawLineRect(RenderTarget target, FloatRect rect, Color color, float thickness = 1f)
    {
        target.Draw(new RectangleShape(rect.Size)
        {
            Position = rect.Position,
            FillColor = Color.Transparent,
            OutlineColor = color,
            OutlineThickness = thickness
        });
    }
}