using Latte.Sfml;
using Latte.Core.Type;


namespace Latte.Elements.Primitives.Shapes;


public class RectangleElement : ShapeElement
{
    protected const uint DefaultPointCount = 16;
    
    
    public new RoundedRectangleShape SfmlShape => (base.SfmlShape as RoundedRectangleShape)!;

    public AnimatableProperty<Vec2f> Size { get; }
    
    public AnimatableProperty<Float> Radius { get; }
    

    public RectangleElement(Element? parent, Vec2f position, Vec2f size, float radius = 0f)
        : base(parent, new RoundedRectangleShape(size, radius, DefaultPointCount))
    {
        RelativePosition.Set(position);
        
        Size = new(this, nameof(Size), size) { CanAnimate = false };
        Radius = new(this, nameof(Radius), radius);
    }


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();
        
        SfmlShape.Size = Size.Value;
        SfmlShape.Radius = Radius.Value;
    }
}