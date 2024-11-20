using System;

namespace Latte.Elements;


public interface IDraggable : IDefaultClickable
{
    bool Dragging { get; }

    event EventHandler? DraggingEvent;
}