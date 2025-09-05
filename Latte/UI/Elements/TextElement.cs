using System;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Application;


using Math = System.Math;


namespace Latte.UI.Elements;


public class TextElement : Element, IClickable
{
    private float _lastFitSize;


    protected IClickable ThisClickable => this;
    protected IFocusable ThisFocusable => this;

    public override Transformable SfmlTransformable => SfmlText;
    public override Drawable SfmlDrawable => SfmlText;

    public Text SfmlText { get; }

    public TextSelectionElement Selection { get; protected set; }
    public string SelectedText => Selection.GetSelectedText();

    public bool Focused { get; set; }
    public bool DisableFocus { get; set; }

    public event EventHandler? FocusEvent;
    public event EventHandler? UnfocusEvent;

    public bool FocusOnMouseDown { get; set; }
    public bool UnfocusOnMouseDownOutside { get; set; }

    public MouseClickState MouseState { get; }
    public bool DisableTruePressOnlyWhenMouseIsUp { get; protected set; }

    public event EventHandler? MouseEnterEvent;
    public event EventHandler? MouseLeaveEvent;
    public event EventHandler? MouseDownEvent;
    public event EventHandler? MouseUpEvent;

    public event EventHandler? MouseHoverEvent;

    public event EventHandler? MouseClickEvent;

    public string Text { get; set; }
    public Text.Styles Style { get; set; }

    public uint Size { get; set; }
    public float LetterSpacing { get; set; }
    public float LineSpacing { get; set; }

    public float BorderSize { get; set; }

    public ColorRGBA Color { get; set; }
    public ColorRGBA BorderColor { get; set; }


    public readonly struct Character(int index, char @char, FloatRect absoluteGeometry)
    {
        public int Index { get; } = index;
        public char Char { get; } = @char;

        public FloatRect AbsoluteGeometry { get; } = absoluteGeometry;
    }


    public TextElement(Element? parent, Vec2f? position, uint? size, string text, Font? font = null) : base(parent)
    {
        SfmlText = new Text(text, font ?? App.DefaultFont);

        Selection = new TextSelectionElement(this);

        FocusOnMouseDown = true;
        UnfocusOnMouseDownOutside = true;

        MouseState = new MouseClickState();

        SetRelativePositionOrAlignment(position);

        if (size is null)
            SizePolicy = SizePolicy.FitParent;

        SizePolicyMargin = new Vec2f(3f, 3f);

        Text = text;
        Style = SFML.Graphics.Text.Styles.Regular;

        Size = size ?? 7;
        LetterSpacing = 1f;
        LineSpacing = 1f;

        BorderSize = 0f;

        Color = SFML.Graphics.Color.White;
        BorderColor = SFML.Graphics.Color.Black;
    }


    public override void Update()
    {
        ThisClickable.UpdateMouseState();
        ThisClickable.ProcessMouseEvents();

        base.Update();
    }


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        // round to avoid blurry text
        SfmlTransformable.Position = AbsolutePosition.Round();
        SfmlTransformable.Origin = new Vec2f(Origin.X, Origin.Y).Round();

        SfmlText.DisplayedString = Text;
        SfmlText.Style = Style;

        SfmlText.CharacterSize = Size;
        SfmlText.LetterSpacing = LetterSpacing;
        SfmlText.LineSpacing = LineSpacing;

        SfmlText.FillColor = Color;
        SfmlText.OutlineColor = BorderColor;
    }


    public override void BorderLessSimpleDraw(IRenderer renderer)
    {
        SfmlText.OutlineThickness = 0f;
        SimpleDraw(renderer);
        SfmlText.OutlineThickness = BorderSize;
    }


    // use SFML's bound API instead of calculating it manually...
    // may not be the best option.

    public override FloatRect GetBounds()
        => SfmlText.GetGlobalBounds();

    public override FloatRect GetRelativeBounds()
        => SfmlText.GetLocalBounds();

    public override FloatRect GetBorderLessBounds()
        => GetBounds().ShrinkRect(BorderSize);

    public override FloatRect GetBorderLessRelativeBounds()
        => GetRelativeBounds().ShrinkRect(BorderSize);


    public override Vec2f GetAlignmentPosition(Alignment alignment)
    {
        var position = AlignmentCalculator.GetTextAlignedPositionOfChild(SfmlText, GetParentBorderLessBounds(), alignment);
        return AlignmentCalculator.ApplyBorderOffset(position, BorderSize, alignment);
    }

    public override Vec2f GetAlignmentRelativePosition(Alignment alignment)
    {
        var position = AlignmentCalculator.GetTextAlignedRelativePositionOfChild(SfmlText, GetParentBorderLessBounds(), alignment);
        return AlignmentCalculator.ApplyBorderOffset(position, BorderSize, alignment);
    }


    public override void ApplySizePolicy()
    {
        var (floatFitSize, fitSize) = CalculateFitSize(GetSizePolicyRect(), GetBounds());

        if (MathF.Abs(_lastFitSize - floatFitSize) > 0.5f)
            Size = fitSize;

        _lastFitSize = floatFitSize;
    }

    private (float, uint) CalculateFitSize(FloatRect targetRect, FloatRect bounds)
    {
        // first find the size based on the height.

        var floatFitSize = CalculateSizePolicyTextSize(targetRect.Height, bounds.Height);
        var fitSize = (uint)Math.Round(floatFitSize);

        // if the calculated text size (bounds) width is greater than the target width, then
        // calculates using the width instead.
        if (CalculateBoundsOfTextWithSize(SfmlText, fitSize).Width > targetRect.Width)
        {
            floatFitSize = CalculateSizePolicyTextSize(targetRect.Width, bounds.Width);
            fitSize = (uint)Math.Round(floatFitSize);
        }

        return (floatFitSize, fitSize);
    }

    // https://math.stackexchange.com/questions/857073/formula-for-adjusting-font-height
    private float CalculateSizePolicyTextSize(float targetSize, float currentSize)
        => targetSize * (Size / currentSize);


    private static FloatRect CalculateBoundsOfTextWithSize(Text text, uint size)
    {
        var oldSize = text.CharacterSize;
        text.CharacterSize = size;

        var bounds = text.GetGlobalBounds();
        text.CharacterSize = oldSize;

        return bounds;
    }


    public Character? CharacterAtPoint(Vec2f point)
    {
        if (!IsPointOver(point))
            return OutsideCharacterFromX(point.X);

        for (var i = 0; i < Text.Length; i++)
        {
            var character = CharacterAtIndex(i);

            if (point.IsPointOverRect(character.AbsoluteGeometry))
                return character;
        }

        return null;
    }

    private Character? OutsideCharacterFromX(float x)
    {
        var bounds = GetBounds();
        var vertices = bounds.RectToVertices();

        if (x < vertices.TopLeft.X)
            return new Character(-1, '\0', bounds with { Width = 0 });

        if (x > vertices.TopRight.X)
            return new Character(Text.Length, '\0', bounds with { Left = vertices.TopRight.X, Width = 0 });

        return null;
    }

    public Character? CharacterAtMousePosition()
        => CharacterAtPoint(MouseInput.PositionInObjectView);


    public Character CharacterAtIndex(int index)
    {
        var character = Text[index];
        var rect = GetAbsoluteGeometryOfCharacter(index);

        return new Character(index, character, rect);
    }


    public FloatRect GetRelativeGeometryOfCharacter(int index)
        => new FloatRect
        {
            Left = SfmlText.FindCharacterPos((uint)index).X,
            Top = GetRelativeBounds().Position.Y,
            Width = GetWidthOfCharacter(index),
            Height = GetBounds().Height
        };

    public FloatRect GetAbsoluteGeometryOfCharacter(int index)
    {
        var relativeGeometry = GetRelativeGeometryOfCharacter(index);
        var absolutePosition = MapToAbsolute(relativeGeometry.Position);

        return new FloatRect(absolutePosition, relativeGeometry.Size);
    }


    private float GetWidthOfCharacter(int index)
    {
        if (index >= Text.Length)
            return 0f;

        var character = Text[index];
        var glyph = SfmlText.Font.GetGlyph(character, Size, false, BorderSize);

        return glyph.Advance;
    }


    public bool IsPointOver(Vec2f point)
        => point.IsPointOverElement(this);


    public void OnFocus()
        => FocusEvent?.Invoke(this, EventArgs.Empty);

    public void OnUnfocus()
        => UnfocusEvent?.Invoke(this, EventArgs.Empty);

    public void OnMouseEnter()
        => MouseEnterEvent?.Invoke(this, EventArgs.Empty);

    public void OnMouseLeave()
        => MouseLeaveEvent?.Invoke(this, EventArgs.Empty);

    public void OnMouseDown()
        => MouseDownEvent?.Invoke(this, EventArgs.Empty);

    public void OnMouseUp()
        => MouseUpEvent?.Invoke(this, EventArgs.Empty);


    public void OnMouseHover()
        => MouseHoverEvent?.Invoke(this, EventArgs.Empty);


    public void OnMouseClick()
        => MouseClickEvent?.Invoke(this, EventArgs.Empty);
}
