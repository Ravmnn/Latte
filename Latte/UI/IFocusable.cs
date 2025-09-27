using System;


namespace Latte.UI;




public interface IFocusable
{
    bool Focused { get; set; }
    bool DisableFocus { get; }


    event EventHandler FocusEvent;
    event EventHandler UnfocusEvent;




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
