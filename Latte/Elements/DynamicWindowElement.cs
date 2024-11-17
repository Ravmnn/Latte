using System;

using SFML.System;

using Latte.Core.Application;


using Math = Latte.Core.Math;


namespace Latte.Elements;


public class DynamicWindowElement : WindowElement, IDraggable
{
    public bool Dragging { get; protected set; }

    protected Vector2f DraggerPosition { get; set; }
    protected Vector2f LastDraggerPosition { get; set; }
    protected Vector2f DraggerPositionDelta => DraggerPosition - LastDraggerPosition;

    public MouseClickState MouseClickState { get; }
    public bool Continuous { get; protected set; }

    public event EventHandler? MouseEnterEvent;
    public event EventHandler? MouseLeaveEvent;
    public event EventHandler? MouseDownEvent;
    public event EventHandler? MouseUpEvent;


    public DynamicWindowElement(string title, Vector2f position, Vector2f size) : base(title, position, size)
    {
        MouseClickState = new();
        
        Continuous = true;
    }
    
    
    public override void Update()
    {
        (this as IDraggable).UpdateClickStateProperties();
        (this as IDraggable).ProcessMouseEvents();
        
        LastDraggerPosition = DraggerPosition;
        DraggerPosition = App.MainWindow.WorldMousePosition;
        
        if (Dragging)
            AbsolutePosition += DraggerPositionDelta;
        
        base.Update();
    }
    
    
    public virtual void OnMouseEnter()
        => MouseEnterEvent?.Invoke(this, EventArgs.Empty);
    
    public virtual void OnMouseLeave()
        => MouseLeaveEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnMouseDown()
    {
        Dragging = true;
        MouseDownEvent?.Invoke(this, EventArgs.Empty);
    }
    
    public virtual void OnMouseUp()
    {
        Dragging = false;
        MouseUpEvent?.Invoke(this, EventArgs.Empty);
    }

    
    public bool IsPointOver(Vector2f point)
        => Math.IsPointOverRoundedRect(point, AbsolutePosition, Size, Radius);
}