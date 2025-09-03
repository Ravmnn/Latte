using System;

using SFML.Window;

using Latte.UI;


namespace Latte.Application;


public static class KeyboardInput
{
    public static event EventHandler<KeyEventArgs>? KeyPressedEvent;
    public static event EventHandler<KeyEventArgs>? KeyReleasedEvent;
    public static event EventHandler<TextEventArgs>? TextEnteredEvent;

    public static KeyEventArgs? PressedKey { get; private set; }
    public static KeyEventArgs? ReleasedKey { get; private set; }
    public static TextEventArgs? EnteredText { get; private set; }
    public static Keyboard.Scancode? PressedKeyCode => PressedKey?.Scancode;
    public static Keyboard.Scancode? ReleasedKeyCode => ReleasedKey?.Scancode;


    public static void AddKeyListeners(Window window)
    {
        window.KeyPressed += OnKeyPressed;
        window.KeyReleased += OnKeyReleased;
        window.TextEntered += OnTextEntered;
    }

    public static void RemoveKeyListeners(Window window)
    {
        window.KeyPressed -= OnKeyPressed;
        window.KeyReleased -= OnKeyReleased;
        window.TextEntered -= OnTextEntered;
    }


    public static void ClearKeyBuffers()
    {
        PressedKey = null;
        ReleasedKey = null;
        EnteredText = null;
    }


    private static void OnKeyPressed(object? sender, KeyEventArgs args)
    {
        PressedKey = args;

        if (FocusManager.CurrentFocused is IKeyboardInputTarget inputTarget)
            inputTarget.OnKeyDown(args);

        KeyPressedEvent?.Invoke(sender, args);
    }

    private static void OnKeyReleased(object? sender, KeyEventArgs args)
    {
        ReleasedKey = args;

        if (FocusManager.CurrentFocused is IKeyboardInputTarget inputTarget)
            inputTarget.OnKeyUp(args);

        KeyReleasedEvent?.Invoke(sender, args);
    }


    private static void OnTextEntered(object? sender, TextEventArgs args)
    {
        EnteredText = args;

        if (FocusManager.CurrentFocused is IKeyboardInputTarget inputTarget)
            inputTarget.OnTextEntered(args);

        TextEnteredEvent?.Invoke(sender, args);
    }
}
