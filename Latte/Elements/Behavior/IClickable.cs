using System;

using Latte.Core.Type;


namespace Latte.Elements.Behavior;


public interface IClickable : IMouseInputTarget
{
    MouseClickState MouseState { get; }
    bool DisableTruePressOnlyWhenMouseIsUpBigForDisableTruePressOnlyWhenMouseIsUp { get; }

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
