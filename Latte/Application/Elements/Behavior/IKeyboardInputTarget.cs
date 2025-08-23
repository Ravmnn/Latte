using System;

using SFML.Window;


namespace Latte.Application.Elements.Behavior;


public interface IKeyboardInputTarget : IFocusable
{
    event EventHandler<KeyEventArgs> KeyDownEvent;
    event EventHandler<KeyEventArgs> KeyUpEvent;
    event EventHandler<KeyEventArgs> SubmitKeyDownEvent;
    event EventHandler<KeyEventArgs> SubmitKeyUpEvent;

    bool IgnoreKeyboardInput { get; set; }

    void OnKeyDown(KeyEventArgs key);
    void OnKeyUp(KeyEventArgs key);

    void OnSubmitKeyDown(KeyEventArgs key);
    void OnSubmitKeyUp(KeyEventArgs key);
}
