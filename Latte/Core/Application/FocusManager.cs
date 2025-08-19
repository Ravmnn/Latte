using Latte.Elements.Behavior;
using Latte.Elements.Primitives;


namespace Latte.Core.Application;


public static class FocusManager
{
    public static Element? ElementWithFocus { get; private set; }


    static FocusManager()
    {
        NavigationManager.CurrentNavigationTargetChangedEvent += (_, _) => OnCurrentNavigationTargetChanged();
    }


    public static void Update()
    {
        if (ElementWithFocus is IFocusable { Focused: false })
            ElementWithFocus = null;

        UnfocusAllOtherElements();
        UnfocusElementsWhichCannotBeFocused();
    }


    public static void FocusOn(Element? element)
    {
        if (element is null)
            ElementWithFocus = null;

        if (element is not IFocusable focusable)
            return;

        ElementWithFocus = element;

        if (!focusable.Focused)
            focusable.Focus();
    }


    private static void UnfocusAllOtherElements()
    {
        foreach (var element in App.Elements)
            if (element != ElementWithFocus && element is IFocusable focusable)
                focusable.Unfocus();
    }

    private static void UnfocusElementsWhichCannotBeFocused()
    {
        foreach (var element in App.Elements)
            if (element is IFocusable { CanFocus: false } focusable)
                focusable.Unfocus();
    }


    private static void OnCurrentNavigationTargetChanged()
        => FocusOn(NavigationManager.CurrentTarget as Element);
}
