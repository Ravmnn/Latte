using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


public class TextSelectionElement : RectangleElement
{
    // parent must not change
    public new TextElement Parent => (base.Parent as TextElement)!;

    public TextElement.Character? Start { get; set; }
    public TextElement.Character? End { get; set; }

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
        SwapCharactersIfStartIsGreater(ref start, ref end);

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

        var start = Start.Value;
        var end = End.Value;

        SwapCharactersIfStartIsGreater(ref start, ref end);

        return Parent.Text.Value[(int)start.Index .. ((int)end.Index + 1)];
    }


    private static void SwapCharactersIfStartIsGreater(ref TextElement.Character start, ref TextElement.Character end)
    {
        if (end.Index < start.Index)
            (start, end) = (end, start);
    }
}
