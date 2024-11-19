using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Elements.Primitives.Shapes;


public abstract class ShapeElement : Element
{
    public override Transformable Transformable => SfmlShape;
    
    public Shape SfmlShape { get; protected set; }

    public AnimatableProperty<Float> BorderSize { get; }

    public AnimatableProperty<ColorRGBA> Color { get; }
    public AnimatableProperty<ColorRGBA> BorderColor { get; }
    
    
    protected ShapeElement(Element? parent, Shape shape) : base(parent)
    {
        SfmlShape = shape;

        BorderSize = new(this, 0f);
        
        Color = new(this, SFML.Graphics.Color.White);
        BorderColor = new(this, SFML.Graphics.Color.White);
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


    public override IntRect GetThisClipArea()
    {
        // ignore border when clipping elements
        
        FloatRect bounds = GetBounds();
        bounds.Top += BorderSize.Value;
        bounds.Left += BorderSize.Value;
        bounds.Width -= BorderSize.Value * 2;
        bounds.Height -= BorderSize.Value * 2;

        return ClipArea.WorldFloatRectToClipArea(bounds);
    }
    
    
    public override FloatRect GetBounds()
        => SfmlShape.GetGlobalBounds();


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        SfmlShape.OutlineThickness = BorderSize.Value;
        SfmlShape.FillColor = Color.Value;
        SfmlShape.OutlineColor = BorderColor.Value;
    }
}