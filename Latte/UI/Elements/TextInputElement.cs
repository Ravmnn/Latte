using SFML.Window;

using Latte.Core.Type;
using Latte.Application;


using static SFML.Window.Cursor;


namespace Latte.UI.Elements;


public class TextInputElement : ButtonElement
{
    public TextInputCaretElement Caret { get; protected set; }

    public new TextElement Text => base.Text!;


    public TextInputElement(Element? parent, Vec2f? position, Vec2f size)
        : base(parent, position, size, "a")
    {
        FocusOnMouseDown = true;

        Text.SizePolicy = SizePolicy.None;
        Text.Size = 20;
        Text.Alignment = Alignment.VerticalCenter | Alignment.Left;

        Caret = new TextInputCaretElement(this);
    }


    public void InsertAtCaret(char character)
    {
        if (char.IsControl(character))
            return;

        Text.Text = Text.Text.Insert(Caret.Index, character.ToString());
        Caret.Index++;
    }


    public void EraseAtCaret()
    {
        if (Caret.Index <= 0)
            return;

        Text.Text = Text.Text.Remove(Caret.Index - 1, 1);
        Caret.Index--;
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
