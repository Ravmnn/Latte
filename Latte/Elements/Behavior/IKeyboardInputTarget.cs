using System;

using SFML.Window;


namespace Latte.Elements.Behavior;


public interface IKeyboardInputTarget : IFocusable
{
    event EventHandler<KeyEventArgs> KeyboardInputReceivedEvent;

    bool IgnoreKeyboardInput { get; set; }

    void OnKeyboardInputReceived(KeyEventArgs keyCode);
}
