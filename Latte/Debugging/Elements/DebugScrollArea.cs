using Latte.Core.Type;
using Latte.UI;
using Latte.UI.Elements;


namespace Latte.Debugging.Elements;


[DebuggerIgnoreInspection]
public class DebugScrollArea : ScrollAreaElement
{
    public DebugScrollArea(Element? parent, Vec2f? position, Vec2f size, Orientation orientation = Orientation.Vertical)
        : base(parent, position, size, orientation)
    {
        Color = new ColorRGBA(150, 150, 150, 100);
        Radius = 3f;
    }
}
