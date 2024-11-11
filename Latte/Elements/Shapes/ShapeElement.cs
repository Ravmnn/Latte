using SFML.Graphics;


namespace Latte.Elements.Shapes;


public abstract class ShapeElement : Element
{
    public override Transformable Transformable => SfmlShape;
    
    public Shape SfmlShape { get; protected set; }
    
    public float BorderSize { get; set; }

    public Color Color { get; set; }
    public Color BorderColor { get; set; }
    
    
    protected ShapeElement(Element? parent, Shape shape) : base(parent)
    {
        SfmlShape = shape;
        
        Color = Color.White;
    }
    

    public override void Draw(RenderTarget target)
    {
        if (!Visible)
            return;
        
        target.Draw(SfmlShape);
        
        base.Draw(target);
    }


    public override FloatRect GetBounds()
        => SfmlShape.GetGlobalBounds();


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        SfmlShape.OutlineThickness = BorderSize;
        SfmlShape.FillColor = Color;
        SfmlShape.OutlineColor = BorderColor;
    }
}