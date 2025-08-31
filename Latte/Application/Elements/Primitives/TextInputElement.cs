using System;

using SFML.Window;

using Latte.Core.Type;


using static SFML.Window.Cursor;


namespace Latte.Application.Elements.Primitives;


public class TextInputElement : ButtonElement
{
    public TextInputCaretElement Caret { get; protected set; }

    public new TextElement Text => base.Text!;


    public TextInputElement(Element? parent, Vec2f? position, Vec2f size)
        : base(parent, position, size, "a")
    {
        FocusOnMouseDown = true;

        Text.SizePolicy.Set(Behavior.SizePolicy.None);
        Text.Size.Set(20);
        Text.Alignment.Set(Behavior.Alignment.VerticalCenter | Behavior.Alignment.Left);

        Caret = new TextInputCaretElement(this);
    }


    public void InsertAtCaret(char character)
    {
        if (char.IsControl(character))
            return;

        var text = Text.Text;

        text.Set(text.Value.Insert(Caret.Index, character.ToString()));
        Caret.Index++;
    }


    public void EraseAtCaret()
    {
        if (Caret.Index <= 0)
            return;

        var text = Text.Text;

        text.Set(text.Value.Remove(Caret.Index - 1, 1));
        Caret.Index--;
    }


    public override void OnMouseDown()
    {
        base.OnMouseDown();
    }


    public override void OnMouseUp()
    {
        base.OnMouseUp();
    }


    public override void OnMouseHover()
    {
        App.Window.Cursor.Type = CursorType.Text;

        base.OnMouseHover();
    }


    public override void OnFocus()
    {
        Caret.Show();

        base.OnFocus();
    }

    public override void OnUnfocus()
    {
        Caret.Hide();

        base.OnUnfocus();
    }


    public override void OnTextEntered(TextEventArgs enteredText)
    {
        InsertAtCaret(enteredText.Unicode[0]);

        base.OnTextEntered(enteredText);
    }


    public override void OnKeyDown(KeyEventArgs key)
    {
        switch (key.Scancode)
        {
            case Keyboard.Scancode.Left:
                Caret.Back();
                break;

            case Keyboard.Scancode.Right:
                Caret.Advance();
                break;

            case Keyboard.Scancode.Backspace:
                EraseAtCaret();
                break;
        }

        base.OnKeyDown(key);
    }
}
