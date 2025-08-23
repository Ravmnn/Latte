using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Application;
using Latte.Application.Elements.Behavior;


namespace Latte.Debugging;


public static class Draw
{
    public static void Rect(RenderTarget target, FloatRect rect, Color color)
        => target.Draw(new RectangleShape(rect.Size)
        {
            Position = rect.Position,
            FillColor = color
        });

    public static void LineRect(RenderTarget target, FloatRect rect, Color color, float thickness = 1f)
        => target.Draw(new RectangleShape(rect.Size)
        {
            Position = rect.Position,
            FillColor = Color.Transparent,
            OutlineColor = color,
            OutlineThickness = thickness
        });


    public static void Text(RenderTarget target, Vec2f position, string text, uint size = 5, Color? color = null, Color? backgroundColor = null)
    {
        var textObject = new Text(text, App.DefaultFont, size)
        {
            Position = position.Round(),
            FillColor = color ?? Color.Black
        };

        if (backgroundColor is not null)
            Rect(target, textObject.GetGlobalBounds().ExpandRect(2f), backgroundColor.Value);

        target.Draw(textObject);
    }

    public static void Text(RenderTarget target, FloatRect parent, Alignment alignment, string text, uint size = 15, Color? color = null, Color? backgroundColor = null)
    {
        var textObject = new Text(text, App.DefaultFont, size)
        {
            FillColor = color ?? Color.Black
        };
        textObject.Position = AlignmentCalculator.GetTextAlignedPositionOfChild(textObject, parent, alignment).Round();

        if (backgroundColor is not null)
            Rect(target, textObject.GetGlobalBounds().ExpandRect(2f), backgroundColor.Value);

        target.Draw(textObject);
    }
}
