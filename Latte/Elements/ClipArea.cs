using SFML.System;
using SFML.Graphics;

using OpenTK.Graphics.OpenGL;

using Latte.Core.Application;
using Latte.Elements.Primitives;


namespace Latte.Elements;


public static class ClipArea
{
    public static void BeginClip(IntRect area)
    {
        Vector2u windowSize = App.Window.Size;

        GL.Enable(EnableCap.ScissorTest);

        // the Y parameter needs to be converted to OpenGL coordinate system
        GL.Scissor(area.Left, (int)windowSize.Y - area.Height - area.Top, area.Width, area.Height);
    }


    public static void EndClip()
    {
        GL.Disable(EnableCap.ScissorTest);
    }


    public static IntRect? OverlapElementClipAreaToParents(Element start)
    {
        Element? element = start;
        IntRect? area = null;

        do
        {
            IntRect newArea = element.GetClipArea();

            if (area is null)
                area = newArea;

            if (area.Value.Intersects(newArea, out IntRect overlap))
                area = overlap;
            else
                return null;

            element = element.Parent;
        } while (element is not null);

        return area.Value;
    }


    public static IntRect GetTopMostClipAreaOfElement(Element element)
    {
        Element currentElement = element;
        
        while (currentElement.Parent is not null)
            currentElement = currentElement.Parent;

        return currentElement.GetThisClipArea();
    }
}