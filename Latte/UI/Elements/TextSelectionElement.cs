using System;

using Latte.Core.Type;
using Latte.Application;


using static SFML.Window.Cursor;


namespace Latte.UI.Elements;


public class TextSelectionElement : RectangleElement
{
    private TextElement.Character? _start;
    private TextElement.Character? _end;


    // parent must not change
    public new TextElement Parent => (base.Parent as TextElement)!;

    public bool CanSelect { get; set; }

    public TextElement.Character? Start
    {
        get => _start;
        set
        {
            _start = value;
            OnSelectionChanged();
        }
    }

    public TextElement.Character? End
    {
        get => _end;
        set
        {
            _end = value;
            OnSelectionChanged();
        }
    }

    public bool IsSelecting { get; protected set; }

    protected bool ShouldUpdateSelection { get; set; }

    public event EventHandler? SelectionChanged;


    public TextSelectionElement(TextElement parent) : base(parent, new Vec2f(), new Vec2f())
    {
        IgnoreMouseInput = true;
        ClipLayerIndexOffset = -1;
        Visible = false;

        Color = new ColorRGBA(190, 190, 190, 150);

        AddEventListeners(Parent);
    }


    private void AddEventListeners(TextElement element)
    {
        element.UnfocusEvent += OnParentUnfocus;
        element.MouseHoverEvent += OnParentMouseHover;
        element.MouseDownEvent += OnParentMouseDown;
        element.MouseUpEvent += OnParentMouseUp;
    }



    public override void ConstantUpdate()
    {
        if (IsSelecting)
            End = Parent.CharacterAtMousePosition() ?? End;

        if (Start is not null && End is not null)
            Select(Start.Value, End.Value);
        else
            Deselect();

        base.ConstantUpdate();
    }


    protected void Select(TextElement.Character start, TextElement.Character end)
    {
        if (!ShouldUpdateSelection)
            return;

        var fromEnd = end.Index < start.Index;

        SwapCharactersIfStartIsGreater(ref start, ref end);

        var startGeometry = start.AbsoluteGeometry;
        var endGeometry = end.AbsoluteGeometry;

        var startPosition = startGeometry.Position;
        var endPosition = endGeometry.Position;

        if (fromEnd)
        {
            startPosition.X += startGeometry.Width;
            endPosition.X += endGeometry.Width;
        }


        AbsolutePosition = startPosition;
        Size = new Vec2f(endPosition.X - startPosition.X, endGeometry.Height);
        Visible = true;

        ShouldUpdateSelection = false;
    }


    protected void Deselect()
    {
        AbsolutePosition = new Vec2f();
        Size = new Vec2f();
        Visible = false;
    }


    public string GetSelectedText()
    {
        if (Start is null || End is null)
            return string.Empty;

        var start = Start.Value;
        var end = End.Value;

        var fromEnd = end.Index < start.Index;

        SwapCharactersIfStartIsGreater(ref start, ref end);

        var startIndex = start.Index + (fromEnd ? 1 : 0);
        var endIndex = end.Index + 1 - (fromEnd ? 0 : 1);

        return Parent.Text[startIndex .. endIndex];
    }


    private static void SwapCharactersIfStartIsGreater(ref TextElement.Character start, ref TextElement.Character end)
    {
        if (end.Index < start.Index)
            (start, end) = (end, start);
    }



    public virtual void OnSelectionChanged()
    {
        ShouldUpdateSelection = true;
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }


    private void OnParentUnfocus(object? _, EventArgs __)
    {
        Start = End = null;
    }


    private void OnParentMouseHover(object? _, EventArgs __)
    {
        if (CanSelect)
            App.Window.Cursor.Type = CursorType.Text;
    }


    private void OnParentMouseDown(object? _, EventArgs __)
    {
        if (CanSelect && Parent.CharacterAtMousePosition() is { } character)
            Start = character;

        IsSelecting = true;
    }

    private void OnParentMouseUp(object? _, EventArgs __)
    {
        IsSelecting = false;
    }
}
