using Latte.Application;
using SFML.Graphics;

using OpenTK.Graphics.OpenGL;


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
        
        BeginDraw();
        target.Draw(SfmlShape);
        EndDraw();
        
        base.Draw(target);
    }


    protected override IntRect GetThisClipArea()
    {
        FloatRect bounds = GetBounds();
        bounds.Top += BorderSize;
        bounds.Left += BorderSize;
        bounds.Width -= BorderSize * 2;
        bounds.Height -= BorderSize * 2;

        return (IntRect)bounds;
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