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

        OnFocus();
    }

    void Unfocus()
    {
        Focused = false;
        OnUnfocus();
    }

    void OnFocus();
    void OnUnfocus();
}
