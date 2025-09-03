using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Sfml;


namespace Latte.UI.Elements;


public class RectangleElement : ShapeElement
{
    protected const uint DefaultPointCount = 16;


    public new RoundedRectangleShape SfmlShape => (base.SfmlShape as RoundedRectangleShape)!;

    public Vec2f Size { get; set; }

    public float Radius { get; set; }


    public RectangleElement(Element? parent, Vec2f? position, Vec2f size, float radius = 0f)
        : base(parent, new RoundedRectangleShape(size, radius, DefaultPointCount))
    {
        SetRelativePositionOrAlignment(position);

        Size = size;
        Radius = radius;
    }


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        SfmlShape.Size = Size;
        SfmlShape.Radius = Radius;
    }


    public override FloatRect GetBounds()
        => new FloatRect(AbsolutePosition, Size * Scale).ExpandRect(BorderSize);

    public override FloatRect GetRelativeBounds()
        => new FloatRect(RelativePosition, Size * Scale).ExpandRect(BorderSize);


    public override void ApplySizePolicy()
    {
        var rect = GetSizePolicyRect();
        AbsolutePosition = rect.Position;
        Size = rect.Size;
    }
}
