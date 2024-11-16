using System;

using SFML.System;
using SFML.Window;

using Latte.Application;


namespace Latte.Elements;


public class DynamicWindowElement : WindowElement, IDraggable
{
    public bool Dragging { get; protected set; }

    protected Vector2f DraggerPosition { get; set; }
    protected Vector2f LastDraggerPosition { get; set; }
    protected Vector2f DraggerPositionDelta => DraggerPosition - LastDraggerPosition;
    
    protected bool IsMouseHover { get; set; }
    protected bool WasMouseHover { get; set; }
    protected bool IsMouseDown { get; set; }
    protected bool WasMouseDown { get; set; }
    
    public event EventHandler? MouseEnterEvent;
    public event EventHandler? MouseLeaveEvent;
    public event EventHandler? MouseDownEvent;
    public event EventHandler? MouseUpEvent;
    
    
    public DynamicWindowElement(string title, Vector2f position, Vector2f size) : base(title, position, size)
    {}
    
    
    public override void Update()
    {
        WasMouseHover = IsMouseHover;
        WasMouseDown = IsMouseDown;
        IsMouseHover = IsPointOver(App.MainWindow.WorldMousePosition);
        IsMouseDown = IsMouseHover && Mouse.IsButtonPressed(Mouse.Button.Left);
    
        if (IsMouseDown)
            OnMouseDown();
        
        else if (WasMouseDown)
            OnMouseUp();
        
        if (IsMouseHover && !WasMouseHover)
            OnMouseEnter();
        
        else if (!IsMouseHover && WasMouseHover)
            OnMouseLeave();
        
        LastDraggerPosition = DraggerPosition;
        DraggerPosition = App.MainWindow.WorldMousePosition;
        
        if (Dragging)
            AbsolutePosition += DraggerPositionDelta;
        
        base.Update();
    }
    
    
    protected virtual void OnMouseEnter() => MouseEnterEvent?.Invoke(this, EventArgs.Empty);
    protected virtual void OnMouseLeave() => MouseLeaveEvent?.Invoke(this, EventArgs.Empty);

    protected virtual void OnMouseDown()
    {
        Dragging = true;
        MouseDownEvent?.Invoke(this, EventArgs.Empty);
    }
    
    protected virtual void OnMouseUp()
    {
        Dragging = false;
        MouseUpEvent?.Invoke(this, EventArgs.Empty);
    }

    
    public bool IsPointOver(Vector2f point)
        => GetBounds().Contains(point);
}