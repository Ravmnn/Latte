using SFML.Graphics;

using Latte.Sfml;
using Latte.Core;
using Latte.Core.Type;
using Latte.Elements.Properties;


namespace Latte.Elements.Primitives.Shapes;


public class RectangleElement : ShapeElement
{
    protected const uint DefaultPointCount = 16;


    public new RoundedRectangleShape SfmlShape => (base.SfmlShape as RoundedRectangleShape)!;

    public AnimatableProperty<Vec2f> Size { get; }

    public AnimatableProperty<Float> Radius { get; }


    // TODO: when position or size is null in the constructor, use alignment (center horizontal or vertical) as default

    public RectangleElement(Element? parent, Vec2f? position, Vec2f size, float radius = 0f)
        : base(parent, new RoundedRectangleShape(size, radius, DefaultPointCount))
    {
        SetRelativePositionOrAlignment(position);

        Size = new AnimatableProperty<Vec2f>(this, nameof(Size), size);
        Radius = new AnimatableProperty<Float>(this, nameof(Radius), radius);
    }


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        SfmlShape.Size = Size.Value;
        SfmlShape.Radius = Radius.Value;
    }


    public override FloatRect GetBounds()
        => new FloatRect(AbsolutePosition, Size.Value * Scale.Value).ExpandRect(BorderSize.Value);

    public override FloatRect GetRelativeBounds()
        => new FloatRect(RelativePosition.Value, Size.Value * Scale.Value).ExpandRect(BorderSize.Value);


    public override void ApplySizePolicy()
    {
        var rect = GetSizePolicyRect();
        AbsolutePosition = rect.Position;
        Size.Set(rect.Size);
    }
}
