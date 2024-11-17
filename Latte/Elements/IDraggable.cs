namespace Latte.Elements;


public interface IDraggable : IDefaultClickable
{
    bool Dragging { get; }
}