using SFML.Window;

using Latte.Core.Type;
using Latte.Application.Elements.Primitives.Shapes;


namespace Latte.Application.Elements.Primitives;


public class TextInputCaretElement : RectangleElement
{
    private int _index;


    public new TextInputElement Parent => (base.Parent as TextInputElement)!;

    public int Index
    {
        get => _index;
        set
        {
            if (value < 0)
                return;

            if (value >= Parent.Text.Text.Value.Length)
                return;

            _index = value;
        }
    }


    public TextInputCaretElement(TextInputElement parent)
        : base(parent, new Vec2f(), new Vec2f(5, 30))
    {
        Color.Set(new ColorRGBA(255, 255, 255, 100));

        Parent.KeyDownEvent += OnParentKeyDown;
    }


    public override void Update()
    {
        RelativePosition.Set(Parent.Text.GetRelativePositionOfCharacter((uint)Index));

        base.Update();
    }


    public void OnParentKeyDown(object? _, KeyEventArgs key)
    {
        switch (key.Scancode)
        {
            case Keyboard.Scancode.Left:
                Index--;
                break;

            case Keyboard.Scancode.Right:
                Index++;
                break;
        }
    }
}
