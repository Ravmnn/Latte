using System;


namespace Latte.Application.Elements.Behavior;


public interface IDraggable : IClickable
{
    bool Dragging { get; }
    bool WasDragging { get; }

    event EventHandler? DragBeginEvent;
    event EventHandler? DragEndEvent;
    event EventHandler? DraggingEvent;


    void ProcessDraggingEvents()
    {
        if (Dragging && !WasDragging)
            OnDragBegin();

        if (!Dragging && WasDragging)
            OnDragEnd();

        if (Dragging)
            OnDragging();
    }


    void OnDragBegin();
    void OnDragEnd();
    void OnDragging();


    void ProcessDragging();
}
