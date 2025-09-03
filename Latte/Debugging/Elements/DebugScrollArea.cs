using Latte.Core.Type;
using Latte.UI.Elements;


namespace Latte.Debugging.Elements;


[DebuggerIgnoreInspection]
public class DebugScrollArea : ScrollAreaElement
{
    public DebugScrollArea(Element? parent, Vec2f? position, Vec2f size, bool verticalScrollHandle = true, bool horizontalScrollHandle = false)
        : base(parent, position, size, verticalScrollHandle, horizontalScrollHandle)
    {
        Color = new ColorRGBA(150, 150, 150, 100);
        Radius = 3f;
    }
}
