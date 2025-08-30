using SFML.Window;

using Latte.Core.Type;


namespace Latte.Application.Elements.Primitives;


public class TextInputElement : ButtonElement
{
    public TextInputCaretElement Caret { get; protected set; }

    public new TextElement Text => base.Text!;


    public TextInputElement(Element? parent, Vec2f? position, Vec2f size)
        : base(parent, position, size, string.Empty)
    {
        Text.Selection.CanSelect = true;
        Text.Alignment.Set(Behavior.Alignment.Center | Behavior.Alignment.Left);

        Caret = new TextInputCaretElement(this);
    }


    public override void OnTextEntered(TextEventArgs enteredText)
    {
        var text = Text.Text;

        text.Set(text.Value.Insert(Caret.Index, enteredText.Unicode));

        base.OnTextEntered(enteredText);
    }
}
