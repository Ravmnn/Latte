using System;

using SFML.Graphics;

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

    RenderBounds = 1 << 1,
    RenderClipBounds = 1 << 2,
    RenderPriority = 1 << 3
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


    public static void DrawText(RenderTarget target, Vec2f position, string text, uint size = 5, Color? color = null)
    {
        target.Draw(new Text(text, TextElement.DefaultTextFont, size)
        {
            Position = position,
            FillColor = color ?? Color.Black
        });
    }

    public static void DrawCenteredText(RenderTarget target, FloatRect parent, string text, uint size = 10, Color? color = null)
    {
        Text textObject = new(text, TextElement.DefaultTextFont, size)
        {
            FillColor = color ?? Color.Black
        };
        textObject.Position = AlignmentCalculator.GetTextAlignedPositionOfChild(textObject, parent, Alignment.Center);

        target.Draw(textObject);
    }


    public static void DrawElementBounds(RenderTarget target, Element element)
        => DrawLineRect(target, element.GetBounds(), Color.Red);

    public static void DrawElementClipBounds(RenderTarget target, Element element)
        => DrawLineRect(target, (FloatRect)element.GetClipArea(), Color.Blue);

    public static void DrawElementPriority(RenderTarget target, Element element)
    {
        uint absolutePriority = (uint)System.Math.Abs(element.Priority);

        DrawRect(target, element.GetBounds(), ColorGenerator.FromIndex(absolutePriority, 50));
    }
}
