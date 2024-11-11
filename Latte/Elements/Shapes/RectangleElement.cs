using SFML.System;
using SFML.Graphics;


namespace Latte.Elements.Shapes;


public class RectangleElement : ShapeElement
{
    public new RectangleShape SfmlShape => (base.SfmlShape as RectangleShape)!;

    public Vector2f Size { get; set; }
    

    public RectangleElement(Element? parent, Vector2f position, Vector2f size) : base(parent, new RectangleShape(size))
    {
        Position = position;
        Size = size;
    }


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();
        
        SfmlShape.Size = Size;
    }
}