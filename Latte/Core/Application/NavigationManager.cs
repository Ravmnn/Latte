using System;
using System.Linq;
using System.Collections.Generic;

using SFML.Window;

using Latte.Elements.Behavior;
using Latte.Elements.Primitives;


namespace Latte.Core.Application;


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
        if (CurrentTarget is Element { Visible: false })
            CurrentTarget = null;
    }


    // TODO: this may be cleaned
    public static void NextElement()
    {
        var targets = GetElementsOrderedByNavigationPriority().ToArray();

        if (CurrentTarget is null)
            CurrentTarget = targets.FirstOrDefault();

        else
        {
            for (var i = 0; i < targets.Length; i++)
                if (targets[i] == CurrentTarget)
                {
                    CurrentTarget = i + 1 >= targets.Length ? targets.First() : targets.ElementAt(i + 1);
                    break;
                }
        }
    }


    public static void PreviousElement()
    {
        var targets = GetElementsOrderedByNavigationPriority().ToArray();

        if (CurrentTarget is null)
            CurrentTarget = targets.FirstOrDefault();

        else
        {
            for (var i = 0; i < targets.Length; i++)
                if (targets[i] == CurrentTarget)
                {
                    CurrentTarget = i - 1 < 0 ? targets.Last() : targets.ElementAt(i - 1);
                    break;
                }
        }
    }


    // TODO: create a property Element.Active and replace it from Element.Visible in some cases.

    private static IEnumerable<INavigationTarget> GetElementsOrderedByNavigationPriority()
        => from element in App.Elements
            let navigationTarget = element as INavigationTarget
            where navigationTarget is { CanFocus: true }
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
