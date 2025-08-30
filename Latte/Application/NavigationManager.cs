using System;
using System.Linq;
using System.Collections.Generic;

using SFML.Window;

using Latte.Application.Elements.Behavior;
using Latte.Application.Elements.Primitives;


namespace Latte.Application;


public static class NavigationManager
{
    private static INavigationTarget? s_currentTarget;


    public static INavigationTarget? CurrentTarget
    {
        get => s_currentTarget;
        private set
        {
            s_currentTarget = value;

            CurrentNavigationTargetChangedEvent?.Invoke(null, EventArgs.Empty);
        }
    }

    public static event EventHandler? CurrentNavigationTargetChangedEvent;


    static NavigationManager()
    {
        KeyboardInput.KeyPressedEvent += (_, args) => OnKeyPressed(args);
    }


    public static void Update()
    {
        if (CurrentTarget is Element { Active: false })
            CurrentTarget = null;
    }


    public static void NextElement()
        => SelectElementFromCurrentTarget((index, targets) =>
            index + 1 >= targets.Length ? targets.First() : targets.ElementAt(index + 1));

    public static void PreviousElement()
        => SelectElementFromCurrentTarget((index, targets) =>
            index - 1 < 0 ? targets.Last() : targets.ElementAt(index - 1));


    private static void SelectElementFromCurrentTarget(Func<int, INavigationTarget[], INavigationTarget> func)
    {
        var targets = GetElementsOrderedByNavigationPriority().ToArray();

        if (CurrentTarget is null)
            CurrentTarget = targets.FirstOrDefault();

        else
        {
            for (var i = 0; i < targets.Length; i++)
                if (targets[i] == CurrentTarget)
                {
                    CurrentTarget = func(i, targets);
                    break;
                }
        }
    }


    private static IEnumerable<INavigationTarget> GetElementsOrderedByNavigationPriority()
        => from element in App.Elements
            let navigationTarget = element as INavigationTarget
            where element is { Active: true } && navigationTarget is { DisableFocus: false }
            orderby navigationTarget.NavigationPriority
            select navigationTarget;


    private static void OnKeyPressed(KeyEventArgs key)
    {
        switch (key.Scancode)
        {
            case Keyboard.Scancode.Tab:
                if (!key.Shift)
                    NextElement();
                else
                    PreviousElement();

                break;

            case Keyboard.Scancode.Escape:
                CurrentTarget = null;
                break;
        }
    }
}
