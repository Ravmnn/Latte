using System;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements.Behavior;
using Latte.Elements.Properties;


using Math = System.Math;


namespace Latte.Elements.Primitives;


public class TextElement : Element
{
    private float _lastFitSize;


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

        SfmlText = new Text(text, font ?? App.DefaultFont);

        RelativePosition.Set(position);

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
}
