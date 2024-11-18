using Latte.Sfml;
using Latte.Core.Type;


namespace Latte.Elements.Primitives.Shapes;


public class RectangleElement : ShapeElement
{
    public new RoundedRectangleShape SfmlShape => (base.SfmlShape as RoundedRectangleShape)!;

    public Property<Vec2f> Size { get; }
    
    public Property<Float> Radius { get; }
    
    protected const uint DefaultPointCount = 16;
    

    public RectangleElement(Element? parent, Vec2f position, Vec2f size, float radius = 0f)
        : base(parent, new RoundedRectangleShape(size, radius, DefaultPointCount))
    {
        Position.Set(position);
        
        Size = new(this, size);
        Radius = new(this, radius);
    }


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();
        
        SfmlShape.Size = Size.Value;
        SfmlShape.Radius = Radius.Value;
    }
}