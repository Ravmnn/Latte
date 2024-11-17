using SFML.System;

using Latte.SFML;


namespace Latte.Elements.Primitives.Shapes;


public class RectangleElement : ShapeElement
{
    public new RoundedRectangleShape SfmlShape => (base.SfmlShape as RoundedRectangleShape)!;

    public Vector2f Size { get; set; }
    
    public float Radius { get; set; }
    
    protected const uint DefaultPointCount = 16;
    

    public RectangleElement(Element? parent, Vector2f position, Vector2f size, float radius = 0f)
        : base(parent, new RoundedRectangleShape(size, radius, DefaultPointCount))
    {
        Position = position;
        Size = size;
        
        Radius = radius;
    }


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();
        
        SfmlShape.Size = Size;
        SfmlShape.Radius = Radius;
    }
}