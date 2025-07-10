using SFML.Window;


namespace Latte.Core.Application;


public static class KeyboardInput
{
    public static KeyEventArgs? PressedKey { get; private set; }
    public static KeyEventArgs? ReleasedKey { get; private set; }
    public static Keyboard.Scancode? PressedKeyCode => PressedKey?.Scancode;
    public static Keyboard.Scancode? ReleasedKeyCode => ReleasedKey?.Scancode;


    public static void AddKeyListener(Window window)
    {
        window.KeyPressed += (_, args) => PressedKey = args;
        window.KeyReleased += (_, args) => ReleasedKey = args;
    }


    public static void ClearKeyBuffers()
    {
        PressedKey = null;
        ReleasedKey = null;
    }
}
