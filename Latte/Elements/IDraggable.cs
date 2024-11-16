namespace Latte.Elements;


public interface IDraggable : IClickable
{
    bool Dragging { get; }
}