using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Core.Type;


using Math = System.Math;


namespace Latte.Elements.Primitives;


public class TextElement : Element
{
    private uint _lastFitSize;


    public override Transformable Transformable => SfmlText;

    public Text SfmlText { get; }

    public Property<string> Text { get; }
    public Property<Text.Styles> Style { get; }

    public Property<uint> Size { get; }
    public AnimatableProperty<Float> LetterSpacing { get; }
    public AnimatableProperty<Float> LineSpacing { get; }

    public AnimatableProperty<Float> BorderSize { get; }

    public AnimatableProperty<ColorRGBA> Color { get; }
    public AnimatableProperty<ColorRGBA> BorderColor { get; }


    public TextElement(Element? parent, Vec2f position, uint? size, string text, Font? font = null) : base(parent)
    {
        IgnoreMouseInput = true;

        SfmlText = new(text, font ?? App.DefaultFont);

        RelativePosition.Set(position);

        if (size is null)
            SizePolicy.Set(SizePolicyType.FitParent);

        Text = new(this, nameof(Text), text);
        Style = new(this, nameof(Style), SFML.Graphics.Text.Styles.Regular);

        Size = new(this, nameof(Size), size ?? 7);
        LetterSpacing = new(this, nameof(LetterSpacing), 1f);
        LineSpacing = new(this, nameof(LineSpacing), 1f);

        BorderSize = new(this, nameof(BorderSize), 0f);

        Color = new(this, nameof(Color), SFML.Graphics.Color.White);
        BorderColor = new(this, nameof(BorderColor), SFML.Graphics.Color.Black);
    }


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        // round to avoid blurry text
        Transformable.Position = AbsolutePosition.Round();
        Transformable.Origin = new Vec2f(Origin.Value.X, Origin.Value.Y).Round();

        SfmlText.DisplayedString = Text;
        SfmlText.Style = Style.Value;

        SfmlText.CharacterSize = Size.Value;
        SfmlText.LetterSpacing = LetterSpacing.Value;
        SfmlText.LineSpacing = LineSpacing.Value;

        SfmlText.FillColor = Color.Value;
        SfmlText.OutlineColor = BorderColor.Value;
    }


    public override void Draw(RenderTarget target)
    {
        BeginDraw();
        target.Draw(SfmlText);
        EndDraw();

        base.Draw(target);
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
        FloatRect targetRect = GetSizePolicyRect();
        FloatRect bounds = GetBounds();

        uint size = CalculateSizePolicyTextSize(targetRect.Height, bounds.Height);

        if (CalculateBoundsOfTextWithSize(SfmlText, size).Width > targetRect.Width)
            size = CalculateSizePolicyTextSize(targetRect.Width, bounds.Width);

        if (Math.Abs((int)_lastFitSize - size) > 2)
            Size.Set(size);

        _lastFitSize = size;
    }

    // https://math.stackexchange.com/questions/857073/formula-for-adjusting-font-height
    private uint CalculateSizePolicyTextSize(float targetSize, float currentSize)
        => (uint)Math.Round(targetSize * (Size.Value / currentSize));


    private static FloatRect CalculateBoundsOfTextWithSize(Text text, uint size)
    {
        uint oldSize = text.CharacterSize;
        text.CharacterSize = size;

        FloatRect bounds = text.GetGlobalBounds();
        text.CharacterSize = oldSize;

        return bounds;
    }
}
