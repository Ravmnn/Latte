using System;


namespace Latte.Elements.Behavior;


public interface IFocusable
{
    event EventHandler FocusEvent;
    event EventHandler UnfocusEvent;

    bool Focused { get; set; }

    void OnFocus();
    void OnUnfocus();
}
