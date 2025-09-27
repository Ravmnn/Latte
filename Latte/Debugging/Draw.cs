using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Application;
using Latte.UI;


namespace Latte.Debugging;




public static class Draw
{
    public static void Rect(IRenderer renderer, FloatRect rect, Color color)
        => renderer.Render(new RectangleShape(rect.Size)
        {
            Position = rect.Position,
            FillColor = color
        });


    public static void LineRect(IRenderer renderer, FloatRect rect, Color color, float thickness = 1f)
        => renderer.Render(new RectangleShape(rect.Size)
        {
            Position = rect.Position,
            FillColor = Color.Transparent,
            OutlineColor = color,
            OutlineThickness = thickness
        });




    public static void Text(IRenderer renderer, Vec2f position, string text, uint size = 5, Color? color = null, Color? backgroundColor = null)
    {
        var textObject = new Text(text, App.DefaultFont, size)
        {
            Position = position.Round(),
            FillColor = color ?? Color.Black
        };

        if (backgroundColor is not null)
            Rect(renderer, textObject.GetGlobalBounds().ExpandRect(2f), backgroundColor.Value);

        renderer.Render(textObject);
    }

    public static void Text(IRenderer renderer, FloatRect parent, Alignment alignment, string text, uint size = 15, Color? color = null, Color? backgroundColor = null)
    {
        var textObject = new Text(text, App.DefaultFont, size)
        {
            FillColor = color ?? Color.Black
        };
        textObject.Position = AlignmentCalculator.GetTextAlignedPositionOfChild(textObject, parent, alignment).Round();

        if (backgroundColor is not null)
            Rect(renderer, textObject.GetGlobalBounds().ExpandRect(2f), backgroundColor.Value);

        renderer.Render(textObject);
    }
}
