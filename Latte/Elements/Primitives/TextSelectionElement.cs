using System;

using SFML.Window;

using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


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

        Color.Set(new ColorRGBA(190, 190, 190, 150));

        AddEventListeners(Parent);
    }


    private void AddEventListeners(TextElement element)
    {
        element.UnfocusEvent -= OnParentUnfocus;
        element.MouseEnterEvent += OnParentMouseEnter;
        element.MouseLeaveEvent += OnParentMouseLeave;
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

        SwapCharactersIfStartIsGreater(ref start, ref end);

        AbsolutePosition = start.Geometry.Position;
        Size.Set(end.Geometry.Position + end.Geometry.Size - start.Geometry.Position);
        Visible = true;

        ShouldUpdateSelection = false;
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



    public virtual void OnSelectionChanged()
    {
        ShouldUpdateSelection = true;
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }


    private void OnParentUnfocus(object? _, EventArgs __)
    {
        Start = End = null;
    }



    private void OnParentMouseEnter(object? _, EventArgs __)
    {
        if (CanSelect)
            App.Window.Cursor = new Cursor(Cursor.CursorType.Text);
    }


    private void OnParentMouseLeave(object? _, EventArgs __)
    {
        // TODO: changing manually may not be a good idea. Create methods like Cursor.Set and Cursor.Unset.
        // Set changes the cursor, while Unset resets the cursor to the type before Set was called.

        if (CanSelect)
            App.Window.Cursor = new Cursor(Cursor.CursorType.Arrow);
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
