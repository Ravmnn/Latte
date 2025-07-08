using System;


namespace Latte.Elements;


public interface IDraggable : IDefaultClickable
{
    bool Dragging { get; }
    bool WasDragging { get; }

    event EventHandler? DragBeginEvent;
    event EventHandler? DragEndEvent;
    event EventHandler? DraggingEvent;


    void OnDragBegin();
    void OnDragEnd();
    void OnDragging();


    void ProcessDragging();
}
