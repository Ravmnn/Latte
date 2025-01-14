using Latte.Elements;
using Latte.Elements.Primitives;


namespace Latte.Core.Application.Debugging;


[DebuggerIgnoreShowBounds, DebuggerIgnoreShowBoundsDimensions, DebuggerIgnoreShowClipArea, DebuggerIgnoreShowPriority]
public class InspectorWindow : WindowElement
{
    public InspectorWindow() : base("Inspector", new(10, 10), new(400, 400), WindowElementStyles.Moveable)
    {
        Radius.Set(5f);

        BorderSize.Set(1f);

        Color.Set(new(100, 100, 100, 100));
        BorderColor.Set(new(255, 255, 255, 200));

        PrioritySnap = PrioritySnap.AlwaysOnTop;
    }
}
