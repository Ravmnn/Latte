using System;


namespace Latte.Elements.Behavior;


public interface IFocusable
{
    event EventHandler FocusEvent;
    event EventHandler UnfocusEvent;

    bool Focused { get; set; }
    bool CanFocus { get; }
    bool DisableFocus { get; set; }

    void OnFocus();
    void OnUnfocus();
}
