using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Rendering;
using Latte.Application;
using Latte.UI;


namespace Latte.Debugging;




public static class Draw
{
    // do not use a BaseObject as debugging draw annotation, since
    // they need to update once before being able to draw

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




    public static void Line(IRenderer renderer, Vec2f from, Vec2f to, Color? color = null)
    {
        color ??= Color.White;

        var vertices = new VertexArray(PrimitiveType.Lines, 2);
        vertices.Append(new Vertex(from, color.Value));
        vertices.Append(new Vertex(to, color.Value));

        renderer.Render(vertices);
    }




    public static void Circle(IRenderer renderer, Vec2f position, float radius, Color? color = null)
    {
        color ??= Color.White;

        var circle = new CircleShape(radius)
        {
            Origin = new Vec2f(radius, radius),
            Position = position,
            FillColor = color.Value
        };

        renderer.Render(circle);
    }


    public static void Point(IRenderer renderer, Vec2f position, Color? color = null)
        => Circle(renderer, position, 4f, color);




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
