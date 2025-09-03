using Latte.Core.Type;
using Latte.UI.Elements;


namespace Latte.Debugging.Elements;


[DebuggerIgnoreInspection]
public abstract class DebugWindow : WindowElement
{
    protected DebugWindow(string title, Vec2f position, Vec2f size)
        : base(title, position, size, WindowElementStyles.Moveable)
    {
        Title.Color = SFML.Graphics.Color.White;

        Radius = 5f;
        BorderSize = 1.5f;

        Color = new ColorRGBA(100, 100, 100, 100);
        BorderColor = new ColorRGBA(255, 255, 255, 200);

        PrioritySnap = PrioritySnap.AlwaysOnTop;
        PrioritySnapOffset = 2;
    }
}
