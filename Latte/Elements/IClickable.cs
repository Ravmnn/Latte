using System;
using Latte.Core.Application;
using SFML.Window;

using Latte.Core.Type;


namespace Latte.Elements;


public class MouseClickState
{
    public bool IsMouseOver { get; set; }
    public bool IsMouseHover { get; set; }
    public bool IsMouseDown { get; set; }
    public bool IsPressed { get; set; }
    public bool IsTruePressed { get; set; }

    public bool WasMouseOver { get; set; }
    public bool WasMouseHover { get; set; }
    public bool WasMouseDown { get; set; }
    public bool WasPressed { get; set; }
    public bool WasTruePressed { get; set; }
}


public interface IClickable : IMouseInputTarget
{
    MouseClickState MouseState { get; }
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
    void UpdateMouseState()
    {
        MouseState.WasMouseOver = MouseState.IsMouseOver;
        MouseState.WasMouseHover = MouseState.IsMouseHover;
        MouseState.WasMouseDown = MouseState.IsMouseDown;
        MouseState.WasPressed = MouseState.IsPressed;
        MouseState.WasTruePressed = MouseState.IsTruePressed;

        MouseState.IsMouseOver = IsPointOver(App.ElementViewMousePosition);
        MouseState.IsMouseHover = CaughtMouseInput;
        MouseState.IsMouseDown = Mouse.IsButtonPressed(Mouse.Button.Left);
        MouseState.IsPressed = MouseState.IsMouseHover && MouseState.IsMouseDown;

        if (!MouseState.IsTruePressed)
            MouseState.IsTruePressed = MouseState.IsPressed && !MouseState.WasMouseDown;

        if (MouseState.IsTruePressed && !(DisableTruePressOnlyWhenMouseIsUp ? MouseState.IsMouseDown : MouseState.IsPressed))
            MouseState.IsTruePressed = false;
    }

    void ProcessMouseEvents()
    {
        bool entered = MouseState.IsMouseHover && !MouseState.WasMouseHover;
        bool leaved = !MouseState.IsMouseHover && MouseState.WasMouseHover;

        if (MouseState.IsTruePressed && !MouseState.WasTruePressed)
            OnMouseDown();

        else if (!MouseState.IsTruePressed && MouseState.WasTruePressed && !MouseState.IsMouseDown)
            OnMouseUp();

        if (entered)
            OnMouseEnter();

        else if (leaved)
            OnMouseLeave();
    }
}


public interface IMouseInputTarget
{
    bool IgnoreMouseInput { get; set; }
    bool CaughtMouseInput { get; set; }
}
