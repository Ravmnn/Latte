using System;


namespace Latte.UI;


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
        if (!Focused)
            return;

        Focused = false;
        OnUnfocus();
    }

    void OnFocus();
    void OnUnfocus();
}
