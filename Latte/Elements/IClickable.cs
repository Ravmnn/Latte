using System;

using SFML.System;
using SFML.Window;

using Latte.Core.Application;


namespace Latte.Elements;


public class MouseClickState
{
    public bool IsMouseHover { get; set; }
    public bool IsMouseDown { get; set; }
    public bool IsPressed { get; set; }
    public bool IsTruePressed { get; set; }
    
    public bool WasMouseHover { get; set; }
    public bool WasMouseDown { get; set; }
    public bool WasTruePressed { get; set; }
}


public interface IClickable
{
    MouseClickState MouseClickState { get; }
    bool Continuous { get; }
    
    event EventHandler? MouseEnterEvent;
    event EventHandler? MouseLeaveEvent;
    event EventHandler? MouseDownEvent;
    event EventHandler? MouseUpEvent;
    
    
    void OnMouseEnter();
    void OnMouseLeave();
    void OnMouseDown();
    void OnMouseUp();
    
    
    bool IsPointOver(Vector2f point);
}


public interface IDefaultClickable : IClickable
{
    void UpdateClickStateProperties()
    {
        MouseClickState.WasMouseHover = MouseClickState.IsMouseHover;
        MouseClickState.WasMouseDown = MouseClickState.IsMouseDown;
        MouseClickState.WasTruePressed = MouseClickState.IsTruePressed;
        
        MouseClickState.IsMouseHover = IsPointOver(App.MainWindow.WorldMousePosition);
        MouseClickState.IsMouseDown = Mouse.IsButtonPressed(Mouse.Button.Left);
        MouseClickState.IsPressed = MouseClickState.IsMouseHover && MouseClickState.IsMouseDown;
        
        if (!MouseClickState.IsTruePressed)
            MouseClickState.IsTruePressed = MouseClickState.IsPressed && !MouseClickState.WasMouseDown;
        
        if (MouseClickState.IsTruePressed && !(Continuous ? MouseClickState.IsMouseDown : MouseClickState.IsPressed))
            MouseClickState.IsTruePressed = false;
    }

    void ProcessMouseEvents()
    {
        bool entered = MouseClickState.IsMouseHover && !MouseClickState.WasMouseHover;
        bool leaved = !MouseClickState.IsMouseHover && MouseClickState.WasMouseHover;
        
        if (MouseClickState.IsTruePressed)
            OnMouseDown();
        
        else if (MouseClickState.WasTruePressed)
            OnMouseUp();
        
        if (entered)
            OnMouseEnter();
        
        else if (leaved)
            OnMouseLeave();
    }
}