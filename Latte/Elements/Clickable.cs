using System;

using SFML.Window;

using Latte.Core.Type;
using Latte.Core.Application;


namespace Latte.Elements;


public class MouseClickState
{
    public bool IsMouseOver { get; set; }
    public bool IsMouseHover { get; set; }
    public bool IsMouseDown { get; set; }
    public bool IsPressed { get; set; }
    public bool IsTruePressed { get; set; }

    // I couldn't find a better name for it ^
    // "IsTruePressed" is true if "IsPressed" and the mouse wasn't down (mouse button pressed)
    // the last iteration.

    // Basically, it means that something is pressed and that the press state
    // started with the mouse inside of that something

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

    event EventHandler? MouseClickEvent;


    void OnMouseEnter();
    void OnMouseLeave();
    void OnMouseDown();
    void OnMouseUp();

    void OnMouseClick();


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

        MouseState.IsMouseOver = IsPointOver(MouseInput.PositionInElementView);
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

        bool pressed = MouseState.IsTruePressed && !MouseState.WasTruePressed;
        bool unpressed = !MouseState.IsTruePressed && MouseState.WasTruePressed;

        if (pressed)
            OnMouseDown();

        else if (unpressed)
        {
            OnMouseUp();

            if (MouseState.IsMouseOver)
                OnMouseClick();
        }

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
