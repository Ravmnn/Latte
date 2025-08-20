using Latte.Elements.Behavior;
using Latte.Elements.Primitives;


namespace Latte.Core.Application;


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
        foreach (var element in App.Elements)
            if (element != CurrentFocused && element is IFocusable focusable)
                focusable.Unfocus();
    }

    private static void UnfocusElementsWhichCannotBeFocused()
    {
        foreach (var element in App.Elements)
            if (element is IFocusable { DisableFocus: true } focusable)
                focusable.Unfocus();
    }


    private static void OnCurrentNavigationTargetChanged()
        => FocusOn(NavigationManager.CurrentTarget);
}
