using System;

using SFML.Window;

using Latte.Core.Application;
using Latte.Core.Type;


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
    MouseClickState ClickState { get; }
    bool DisableTruePressOnlyWhenMouseIsUp { get; }
    
    event EventHandler? MouseEnterEvent;
    event EventHandler? MouseLeaveEvent;
    event EventHandler? MouseDownEvent;
    event EventHandler? MouseUpEvent;
    
    
    void OnMouseEnter();
    void OnMouseLeave();
    void OnMouseDown();
    void OnMouseUp();
    
    
    bool IsPointOver(Vec2f point);
}


public interface IDefaultClickable : IClickable
{
    void UpdateClickStateProperties()
    {
        ClickState.WasMouseHover = ClickState.IsMouseHover;
        ClickState.WasMouseDown = ClickState.IsMouseDown;
        ClickState.WasTruePressed = ClickState.IsTruePressed;
        
        ClickState.IsMouseHover = IsPointOver(App.MainWindow.WorldMousePosition);
        ClickState.IsMouseDown = Mouse.IsButtonPressed(Mouse.Button.Left);
        ClickState.IsPressed = ClickState.IsMouseHover && ClickState.IsMouseDown;
        
        if (!ClickState.IsTruePressed)
            ClickState.IsTruePressed = ClickState.IsPressed && !ClickState.WasMouseDown;
        
        if (ClickState.IsTruePressed && !(DisableTruePressOnlyWhenMouseIsUp ? ClickState.IsMouseDown : ClickState.IsPressed))
            ClickState.IsTruePressed = false;
    }

    void ProcessMouseEvents()
    {
        bool entered = ClickState.IsMouseHover && !ClickState.WasMouseHover;
        bool leaved = !ClickState.IsMouseHover && ClickState.WasMouseHover;
        
        if (ClickState.IsTruePressed && !ClickState.WasTruePressed)
            OnMouseDown();
        
        else if (!ClickState.IsTruePressed && ClickState.WasTruePressed)
            OnMouseUp();
        
        if (entered)
            OnMouseEnter();
        
        else if (leaved)
            OnMouseLeave();
    }
}