using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;

namespace Latte.UI.Elements;




public class CircleElement : ShapeElement
{
    protected const uint DefaultPointCount = 32;




    public new CircleShape SfmlShape => (base.SfmlShape as CircleShape)!;


    public float Radius { get; set; }




    public CircleElement(Element? parent, Vec2f? position, float radius)
        : base(parent, new CircleShape(radius, DefaultPointCount))
    {
        SetRelativePositionOrAlignment(position);

        Radius = radius;
    }




    public override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        SfmlShape.Radius = Radius;
    }




    public override FloatRect GetBounds()
        => new FloatRect(AbsolutePosition, new Vec2f(Radius * 2, Radius * 2)).ExpandRect(BorderSize);


    public override FloatRect GetRelativeBounds()
        => new FloatRect(RelativePosition, new Vec2f(Radius * 2, Radius * 2)).ExpandRect(BorderSize);




    public override void ApplySizePolicy()
    {
        var rect = GetSizePolicyRect();
        AbsolutePosition = rect.Position;
        Radius = rect.Size.X / 2f;
    }
}
