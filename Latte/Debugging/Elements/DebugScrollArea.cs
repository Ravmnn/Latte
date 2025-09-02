using Latte.Application.Elements.Primitives;
using Latte.Core.Type;


namespace Latte.Debugging.Elements;


public class DebugScrollArea : ScrollAreaElement
{
    public DebugScrollArea(Element? parent, Vec2f? position, Vec2f size, bool verticalScrollHandle = true, bool horizontalScrollHandle = false)
        : base(parent, position, size, verticalScrollHandle, horizontalScrollHandle)
    {
        Color = new ColorRGBA(150, 150, 150, 100);
        Radius = 3f;
    }
}
