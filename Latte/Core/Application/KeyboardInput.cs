using System;

using SFML.Window;

using Latte.Elements.Behavior;
using Latte.Elements.Primitives;
using OpenTK.Graphics.GL;


namespace Latte.Core.Application;



public static class KeyboardInput
{
    public static event EventHandler<KeyEventArgs>? KeyPressedEvent;
    public static event EventHandler<KeyEventArgs>? KeyReleasedEvent;
    public static KeyEventArgs? PressedKey { get; private set; }
    public static KeyEventArgs? ReleasedKey { get; private set; }
    public static Keyboard.Scancode? PressedKeyCode => PressedKey?.Scancode;
    public static Keyboard.Scancode? ReleasedKeyCode => ReleasedKey?.Scancode;

    public static Element? ElementWithFocus { get; private set; }


    static KeyboardInput()
    {
        KeyPressedEvent += (_, args) => OnKeyPressed(args);
    }


    public static void AddKeyListener(Window window)
    {
        window.KeyPressed += (sender, args) => KeyPressedEvent?.Invoke(sender, args);
        window.KeyReleased += (sender, args) => KeyReleasedEvent?.Invoke(sender, args);

        KeyPressedEvent += (_, args) => PressedKey = args;
        KeyReleasedEvent += (_, args) => ReleasedKey = args;
    }


    public static void ClearKeyBuffers()
    {
        PressedKey = null;
        ReleasedKey = null;
    }


    public static void Update()
    {
        ElementWithFocus = null;

        foreach (var element in App.Elements)
            if (element is IKeyboardInputTarget { Focused: true })
                ElementWithFocus = element;
    }


    private static void OnKeyPressed(KeyEventArgs key)
    {
        if (ElementWithFocus is IKeyboardInputTarget inputTarget)
            inputTarget.OnKeyboardInputReceived(key);
    }
}
