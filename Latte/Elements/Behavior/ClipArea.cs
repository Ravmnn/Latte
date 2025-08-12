using Latte.Core.Application;
using Latte.Elements.Primitives;

using OpenTK.Graphics.OpenGL;

using SFML.Graphics;


namespace Latte.Elements.Behavior;


// BUG: clip not 100% accurate... 1 pixel error margin.


public static class ClipArea
{
    public static void BeginClip(IntRect area)
    {
        var windowSize = App.Window.Size;

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
        var element = start;
        IntRect? area = null;

        do
        {
            var newArea = element.GetClipArea();

            area ??= newArea;

            if (area.Value.Intersects(newArea, out var overlap))
                area = overlap;
            else
                return null;

            element = element.Parent;
        }
        while (element is not null);

        return area.Value;
    }
}
