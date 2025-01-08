using SFML.Graphics;


namespace Latte.Core;


// TODO: add a debug mode for only showing the priority of elements

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
