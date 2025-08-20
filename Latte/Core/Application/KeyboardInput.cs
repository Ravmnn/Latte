using System;

using SFML.Window;

using Latte.Elements.Behavior;


namespace Latte.Core.Application;



public static class KeyboardInput
{
    public static event EventHandler<KeyEventArgs>? KeyPressedEvent;
    public static event EventHandler<KeyEventArgs>? KeyReleasedEvent;
    public static KeyEventArgs? PressedKey { get; private set; }
    public static KeyEventArgs? ReleasedKey { get; private set; }
    public static Keyboard.Scancode? PressedKeyCode => PressedKey?.Scancode;
    public static Keyboard.Scancode? ReleasedKeyCode => ReleasedKey?.Scancode;


    static KeyboardInput()
    {
        KeyPressedEvent += (_, args) => OnKeyPressed(args);
        KeyReleasedEvent += (_, args) => OnKeyReleased(args);
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


    private static void OnKeyPressed(KeyEventArgs key)
    {
        if (FocusManager.CurrentFocused is IKeyboardInputTarget inputTarget)
            inputTarget.OnKeyDown(key);
    }

    private static void OnKeyReleased(KeyEventArgs key)
    {
        if (FocusManager.CurrentFocused is IKeyboardInputTarget inputTarget)
            inputTarget.OnKeyUp(key);
    }
}
