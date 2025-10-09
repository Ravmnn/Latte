using System;


namespace Latte.UI;




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



    void ProcessDragging();

    void OnDragBegin();
    void OnDragEnd();
    void OnDragging();
}
