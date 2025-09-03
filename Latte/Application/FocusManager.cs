using System;

using Latte.UI;


namespace Latte.Application;


public static class FocusManager
{
    public static IFocusable? CurrentFocused { get; private set; }


    static FocusManager()
    {
        NavigationManager.CurrentNavigationTargetChangedEvent += (_, _) => OnCurrentNavigationTargetChanged();
    }


    public static void Update()
    {
        if (!CurrentFocused?.Focused ?? false)
            CurrentFocused = null;

        UnfocusAllOtherElements();
        UnfocusElementsWhichCannotBeFocused();
    }


    public static void FocusOn(IFocusable? focusable)
    {
        if (focusable is null)
        {
            CurrentFocused = null;
            return;
        }

        CurrentFocused = focusable;

        focusable.Focus();
    }


    private static void UnfocusAllOtherElements()
    {
        foreach (var element in App.Objects)
            if (element != CurrentFocused && element is IFocusable focusable)
                focusable.Unfocus();
    }

    private static void UnfocusElementsWhichCannotBeFocused()
    {
        foreach (var element in App.Objects)
            if (element is IFocusable { DisableFocus: true } focusable)
                focusable.Unfocus();
    }



    public static void AddFocusListenerTo(IFocusable focusable)
        => focusable.FocusEvent += OnFocusableFocus;

    public static void RemoveFocusListenerOf(IFocusable focusable)
        => focusable.FocusEvent -= OnFocusableFocus;



    private static void OnFocusableFocus(object? sender, EventArgs _)
    {
        if (sender is not IFocusable focusable)
            return;

        FocusOn(focusable);
    }


    private static void OnCurrentNavigationTargetChanged()
        => FocusOn(NavigationManager.CurrentTarget);
}
