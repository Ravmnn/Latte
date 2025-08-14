using System.Linq;

using OpenTK.Graphics.OpenGL;

using SFML.Graphics;

using Latte.Elements.Primitives;


namespace Latte.Elements.Behavior;


// BUG: clip not 100% accurate... 1 pixel error margin.


public static class Clipping
{
    public static void ClipEnable()
    {
        GL.Enable(EnableCap.StencilTest);
        GL.Clear(ClearBufferMask.StencilBufferBit);
    }


    public static void ClipDisable()
    {
        GL.StencilFunc(StencilFunction.Always, 0, 0xff);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);

        GL.Disable(EnableCap.StencilTest);
    }


    public static void Clip(int layerIndex)
    {
        GL.StencilFunc(StencilFunction.Equal, layerIndex, 0xff);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
    }


    public static void SetClipToParents(RenderTarget target, Element element)
    {
        DisableColorMask();

        GL.StencilFunc(StencilFunction.Always, 1, 0xff);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);

        var parents = element.GetParents().Reverse().ToArray();

        if (parents.Length > 0)
            parents.First().BorderLessSimpleDraw(target);

        GL.StencilFunc(StencilFunction.Notequal, 0, 0xff);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Incr);

        for (var i = 1; i < parents.Length; i++)
            parents[i].BorderLessSimpleDraw(target);

        EnableColorMask();
    }


    public static int GetClipLayerIndexOf(Element element)
        => element.GetParents().Count();


    private static void EnableColorMask()
    {
        GL.ColorMask(true, true, true, true);
        GL.DepthMask(true);
    }


    private static void DisableColorMask()
    {
        GL.ColorMask(false, false, false, false);
        GL.DepthMask(false);
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
