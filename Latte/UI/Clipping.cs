using System.Linq;

using OpenTK.Graphics.OpenGL;

using SFML.Graphics;

using Latte.Core;
using Latte.UI.Elements;


namespace Latte.UI;




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


    // The clipping logic is based on stencil bits to determine where
    // it is allowed to draw something. If the pixel stencil bit is
    // not 0, drawing is allowed.

    // The above approach would work perfectly if this framework didn't
    // use hierarchy to structure the elements. But, since it does,
    // it would not be possible to specify if an element could be drawn
    // in a location, while another one could not.
    // For instance, a text inside a button cannot be drawn outside the
    // button's content. This would work if the button hadn't a parent.
    // If it does, everywhere inside the top-most parent would be considered
    // drawable for all elements, since all its area would have a stencil bit of 1.
    // To solve that problem, stencil bits from 1 to 255 are used to map
    // different drawable layers. Layer index is the same as stencil bit.
    // In the example of the button I mentioned above, the parent of the button
    // would have a layer index of 1; the button, 2; the text inside the button, 3.
    // The button can only be drawn inside a layer with index 1 or more, and the text can only
    // be drawn in a layer of index 2 or more.


    public static void Clip(int layerIndex)
    {
        GL.StencilFunc(StencilFunction.Lequal, layerIndex, 0xff);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
    }


    public static void SetClipToParents(IRenderer renderer, Element element)
    {
        DisableColorMask();

        // now, every drawing operation will set the stencil bit to 1
        GL.StencilFunc(StencilFunction.Always, 1, 0xff);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);

        var parents = element.GetParents().Reverse().ToArray();

        // the first drawing sets the initial stencil bit to 1
        if (parents.Length > 0)
            parents.First().BorderLessSimpleDraw(renderer);

        // now, drawings are only allowed where the stencil bit is not 0.
        // if the pixel has a stencil bit > 0, increment it
        GL.StencilFunc(StencilFunction.Notequal, 0, 0xff);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Incr);

        for (var i = 1; i < parents.Length; i++)
            parents[i].BorderLessSimpleDraw(renderer);

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
