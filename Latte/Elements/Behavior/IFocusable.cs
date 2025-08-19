using System;


namespace Latte.Elements.Behavior;


public interface IFocusable
{
    event EventHandler FocusEvent;
    event EventHandler UnfocusEvent;

    bool Focused { get; set; }
    bool CanFocus { get; }
    bool DisableFocus { get; set; }

    void Focus() => Focused = true;
    void Unfocus() => Focused = false;

    void OnFocus();
    void OnUnfocus();
}
