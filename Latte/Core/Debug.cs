using System;
using System.Globalization;
using System.Net.Mime;
using SFML.Graphics;

using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements;
using Latte.Elements.Primitives;


namespace Latte.Core;


// TODO: add debug option for rendering the bounds dimensions
// TODO: add a way of changing these options in runtime


[Flags]
public enum DebugOptions
{
    None,

    Clip = 1 << 0,
    OnlyHoveredElement = 1 << 1,
    OnlyTrueHoveredElement = 1 << 2,

    RenderBounds = 1 << 3,
    RenderBoundsDimensions = 1 << 4,
    RenderClipBounds = 1 << 5,
    RenderPriority = 1 << 6
}


public static class Debug
{
    public static void DrawRect(RenderTarget target, FloatRect rect, Color color)
        => target.Draw(new RectangleShape(rect.Size)
        {
            Position = rect.Position,
            FillColor = color
        });

    public static void DrawLineRect(RenderTarget target, FloatRect rect, Color color, float thickness = 1f)
        => target.Draw(new RectangleShape(rect.Size)
        {
            Position = rect.Position,
            FillColor = Color.Transparent,
            OutlineColor = color,
            OutlineThickness = thickness
        });


    public static void DrawText(RenderTarget target, Vec2f position, string text, uint size = 5, Color? color = null, Color? backgroundColor = null)
    {
        Text textObject = new(text, App.DefaultFont, size)
        {
            Position = position.Round(),
            FillColor = color ?? Color.Black
        };

        if (backgroundColor is not null)
            DrawRect(target, textObject.GetGlobalBounds().ExpandRect(2f), backgroundColor.Value);

        target.Draw(textObject);
    }

    public static void DrawText(RenderTarget target, FloatRect parent, Alignment alignment, string text, uint size = 15, Color? color = null, Color? backgroundColor = null)
    {
        Text textObject = new(text, App.DefaultFont, size)
        {
            FillColor = color ?? Color.Black
        };
        textObject.Position = AlignmentCalculator.GetTextAlignedPositionOfChild(textObject, parent, alignment).Round();

        if (backgroundColor is not null)
            DrawRect(target, textObject.GetGlobalBounds().ExpandRect(2f), backgroundColor.Value);

        target.Draw(textObject);
    }


    public static void DrawElementBounds(RenderTarget target, Element element)
        => DrawLineRect(target, element.GetBounds(), Color.Red);

    public static void DrawElementBoundsDimensions(RenderTarget target, Element element)
    {
        FloatRect bounds = element.GetBounds();
        FloatRect borderLessBounds = element.GetBorderLessBounds();

        Color backgroundColor = new(255, 255, 255, 220);

        string width = bounds.Width.ToString(CultureInfo.InvariantCulture);
        string height = bounds.Height.ToString(CultureInfo.InvariantCulture);

        DrawText(target, borderLessBounds with { Top = borderLessBounds.Top + borderLessBounds.Height + 10 }, Alignment.HorizontalCenter | Alignment.Top, width, backgroundColor: backgroundColor);
        DrawText(target, borderLessBounds with { Left = borderLessBounds.Left + borderLessBounds.Width + 10 }, Alignment.VerticalCenter | Alignment.Left, height, backgroundColor: backgroundColor);
    }

    public static void DrawElementClipBounds(RenderTarget target, Element element)
        => DrawLineRect(target, (FloatRect)element.GetClipArea(), Color.Blue);

    public static void DrawElementPriority(RenderTarget target, Element element)
    {
        uint absolutePriority = (uint)System.Math.Abs(element.Priority);

        DrawRect(target, element.GetBounds(), ColorGenerator.FromIndex(absolutePriority, 50));
    }
}
