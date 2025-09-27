using System;

using SFML.Window;


namespace Latte.UI;




public interface IKeyboardInputTarget : IFocusable
{
    bool IgnoreKeyboardInput { get; set; }


    event EventHandler<KeyEventArgs> KeyDownEvent;
    event EventHandler<KeyEventArgs> KeyUpEvent;
    event EventHandler<TextEventArgs> TextEnteredEvent;
    event EventHandler<KeyEventArgs> SubmitKeyDownEvent;
    event EventHandler<KeyEventArgs> SubmitKeyUpEvent;




    void OnKeyDown(KeyEventArgs key);
    void OnKeyUp(KeyEventArgs key);
    void OnTextEntered(TextEventArgs text);

    void OnSubmitKeyDown(KeyEventArgs key);
    void OnSubmitKeyUp(KeyEventArgs key);
}
