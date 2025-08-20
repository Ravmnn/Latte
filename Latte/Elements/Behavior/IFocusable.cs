using System;


namespace Latte.Elements.Behavior;


public interface IFocusable
{
    event EventHandler FocusEvent;
    event EventHandler UnfocusEvent;

    bool Focused { get; set; }
    bool DisableFocus { get; }

    void Focus()
    {
        if (DisableFocus)
        {
            Unfocus();
            return;
        }

        if (Focused)
            return;

        Focused = true;

        // TODO: detect the current focused object from FocusManager.
        // mixing application logic with element logic is bad, so do not use FocusManager.FocusOn to
        // set the current global focused object.

        OnFocus();
    }

    void Unfocus()
        => Focused = false;

    void OnFocus();
    void OnUnfocus();
}
