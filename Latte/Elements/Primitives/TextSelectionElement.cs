using System;
using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


// TODO: create attribute ConstantParentAttribute, which prohibits the element parent to be changed

public class TextSelectionElement : RectangleElement
{
    private TextElement.Character? _start;
    private TextElement.Character? _end;


    public new TextElement Parent => (base.Parent as TextElement)!;

    public TextElement.Character? Start
    {
        get => _start;
        set
        {
            _start = value;
            SwapCharactersIfStartIsGreater(ref _start, ref _end);
        }
    }

    public TextElement.Character? End
    {
        get => _end;
        set
        {
            // TODO: not working
            _end = value;
            SwapCharactersIfStartIsGreater(ref _start, ref _end);
        }
    }

    public bool IsSelecting => Start is not null;


    public TextSelectionElement(TextElement parent) : base(parent, new Vec2f(), new Vec2f())
    {
        IgnoreMouseInput = true;
        Clip = false;
        Visible = false;

        Color.Set(new ColorRGBA(190, 190, 190, 150));
    }


    public override void ConstantUpdate()
    {
        if (Start is not null && End is not null)
            Select(Start.Value, End.Value);
        else
            Deselect();

        base.ConstantUpdate();
    }


    protected void Select(TextElement.Character start, TextElement.Character end)
    {
        AbsolutePosition = start.Geometry.Position;
        Size.Set(end.Geometry.Position + end.Geometry.Size - start.Geometry.Position);
        Visible = true;
    }


    protected void Deselect()
    {
        AbsolutePosition = new Vec2f();
        Size.Set(new Vec2f());
        Visible = false;
    }


    public string GetSelectedText()
    {
        if (Start is null || End is null)
            return string.Empty;

        return Parent.Text.Value[(int)Start.Value.Index .. ((int)End.Value.Index + 1)];
    }


    private static void SwapCharactersIfStartIsGreater(ref TextElement.Character? start, ref TextElement.Character? end)
    {
        if (start is not null && end is not null && end.Value.Index < start.Value.Index)
            (start, end) = (end, start);
    }
}
