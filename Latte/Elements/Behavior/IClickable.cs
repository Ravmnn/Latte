using System;

using SFML.Window;

using Latte.Core.Application;
using Latte.Core.Type;


namespace Latte.Elements.Behavior;


// TODO: IClickable should be INavigation


public interface IClickable : IMouseInputTarget
{
    MouseClickState MouseState { get; }
    bool DisableTruePressOnlyWhenMouseIsUp { get; }

    event EventHandler? MouseEnterEvent;
    event EventHandler? MouseLeaveEvent;
    event EventHandler? MouseDownEvent;
    event EventHandler? MouseUpEvent;

    event EventHandler? MouseClickEvent;


    void UpdateMouseState()
    {
        if (IgnoreMouseInput)
        {
            MouseState.SetAllToFalse();
            return;
        }

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
        var entered = MouseState.IsMouseHover && !MouseState.WasMouseHover;
        var leaved = !MouseState.IsMouseHover && MouseState.WasMouseHover;

        var pressed = MouseState.IsTruePressed && !MouseState.WasTruePressed;
        var unpressed = !MouseState.IsTruePressed && MouseState.WasTruePressed;

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


    void OnMouseEnter();
    void OnMouseLeave();
    void OnMouseDown();
    void OnMouseUp();

    void OnMouseClick();


    bool IsPointOver(Vec2f point);
}
