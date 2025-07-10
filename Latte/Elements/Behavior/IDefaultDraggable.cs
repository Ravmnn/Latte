namespace Latte.Elements.Behavior;


public interface IDefaultDraggable : IDraggable
{
    void ProcessDraggingEvents()
    {
        if (Dragging && !WasDragging)
            OnDragBegin();

        if (!Dragging && WasDragging)
            OnDragEnd();

        if (Dragging)
            OnDragging();
    }
}
