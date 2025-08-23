using System;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements.Behavior;
using Latte.Elements.Properties;


using Math = System.Math;


namespace Latte.Elements.Primitives;


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

    public Property<string> Text { get; }
    public Property<Text.Styles> Style { get; }

    public Property<uint> Size { get; }
    public AnimatableProperty<Float> LetterSpacing { get; }
    public AnimatableProperty<Float> LineSpacing { get; }

    public AnimatableProperty<Float> BorderSize { get; }

    public AnimatableProperty<ColorRGBA> Color { get; }
    public AnimatableProperty<ColorRGBA> BorderColor { get; }


    public readonly struct Character(uint index, char @char, FloatRect geometry)
    {
        public uint Index { get; } = index;
        public char Char { get; } = @char;

        public FloatRect Geometry { get; } = geometry;
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
            SizePolicy.Set(Behavior.SizePolicy.FitParent);

        SizePolicyMargin.Set(new Vec2f(3f, 3f));

        Text = new Property<string>(this, nameof(Text), text);
        Style = new Property<Text.Styles>(this, nameof(Style), SFML.Graphics.Text.Styles.Regular);

        Size = new Property<uint>(this, nameof(Size), size ?? 7);
        LetterSpacing = new AnimatableProperty<Float>(this, nameof(LetterSpacing), 1f);
        LineSpacing = new AnimatableProperty<Float>(this, nameof(LineSpacing), 1f);

        BorderSize = new AnimatableProperty<Float>(this, nameof(BorderSize), 0f);

        Color = new AnimatableProperty<ColorRGBA>(this, nameof(Color), SFML.Graphics.Color.White);
        BorderColor = new AnimatableProperty<ColorRGBA>(this, nameof(BorderColor), SFML.Graphics.Color.Black);
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
        SfmlTransformable.Origin = new Vec2f(Origin.Value.X, Origin.Value.Y).Round();

        SfmlText.DisplayedString = Text;
        SfmlText.Style = Style.Value;

        SfmlText.CharacterSize = Size.Value;
        SfmlText.LetterSpacing = LetterSpacing.Value;
        SfmlText.LineSpacing = LineSpacing.Value;

        SfmlText.FillColor = Color.Value;
        SfmlText.OutlineColor = BorderColor.Value;
    }


    public override void BorderLessSimpleDraw(RenderTarget target)
    {
        SfmlText.OutlineThickness = 0f;
        SimpleDraw(target);
        SfmlText.OutlineThickness = BorderSize.Value;
    }


    // use SFML's bound API instead of calculating it manually...
    // may not be the best option.

    public override FloatRect GetBounds()
        => SfmlText.GetGlobalBounds();

    public override FloatRect GetRelativeBounds()
        => SfmlText.GetLocalBounds();

    public override FloatRect GetBorderLessBounds()
        => GetBounds().ShrinkRect(BorderSize.Value);

    public override FloatRect GetBorderLessRelativeBounds()
        => GetRelativeBounds().ShrinkRect(BorderSize.Value);


    public override Vec2f GetAlignmentPosition(Alignment alignment)
        => AlignmentCalculator.GetTextAlignedPositionOfChild(SfmlText, GetParentBorderLessBounds(), alignment);

    public override Vec2f GetAlignmentRelativePosition(Alignment alignment)
        => AlignmentCalculator.GetTextAlignedRelativePositionOfChild(SfmlText, GetParentBorderLessBounds(), alignment);


    public override void ApplySizePolicy()
    {
        var (floatFitSize, fitSize) = CalculateFitSize(GetSizePolicyRect(), GetBounds());

        if (MathF.Abs(_lastFitSize - floatFitSize) > 0.5f)
            Size.Set(fitSize);

        _lastFitSize = floatFitSize;
    }

    private (float, uint) CalculateFitSize(FloatRect targetRect, FloatRect bounds)
    {
        // first find the size based on the height...

        var floatFitSize = CalculateSizePolicyTextSize(targetRect.Height, bounds.Height);
        var fitSize = (uint)Math.Round(floatFitSize);

        // if the calculated text size (bounds) width is greater than the target width, then
        // calculates using the width instead
        if (CalculateBoundsOfTextWithSize(SfmlText, fitSize).Width > targetRect.Width)
        {
            floatFitSize = CalculateSizePolicyTextSize(targetRect.Width, bounds.Width);
            fitSize = (uint)Math.Round(floatFitSize);
        }

        return (floatFitSize, fitSize);
    }

    // https://math.stackexchange.com/questions/857073/formula-for-adjusting-font-height
    private float CalculateSizePolicyTextSize(float targetSize, float currentSize)
        => targetSize * (Size.Value / currentSize);


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
        if (!IsPointOverBounds(point))
            return null;

        for (var i = 0u; i < Text.Value.Length; i++)
        {
            var character = Text.Value[(int)i];

            var position = GetAbsolutePositionOfCharacter(i);
            var size = new Vec2f(GetWidthOfCharacter(i), GetBounds().Height);

            var rect = new FloatRect(position, size);

            if (point.IsPointOverRect(rect))
                return new Character(i, character, rect);
        }

        return null;
    }


    public Character? CharacterAtMousePosition()
        => CharacterAtPoint(MouseInput.PositionInElementView);


    private Vec2f GetAbsolutePositionOfCharacter(uint index)
    {
        if (index >= Text.Value.Length)
            return SfmlText.FindCharacterPos(index);

        var positionX = MapToAbsolute(SfmlText.FindCharacterPos(index)).X;
        var positionY = GetBounds().Top;

        return new Vec2f(positionX, positionY);
    }


    private float GetWidthOfCharacter(uint index)
    {
        if (index >= Text.Value.Length)
            return 0f;

        var character = Text.Value[(int)index];
        var glyph = SfmlText.Font.GetGlyph(character, Size, false, BorderSize.Value);

        return glyph.Advance;
    }


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


    public bool IsPointOver(Vec2f point)
        => IsPointOverBounds(point);
}
