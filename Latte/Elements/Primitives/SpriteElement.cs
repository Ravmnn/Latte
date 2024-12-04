using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Elements.Primitives;


public class SpriteElement : Element
{
    public override Transformable Transformable => SfmlSprite;
    
    public Sprite SfmlSprite { get; }
    public Texture SfmlTexture => SfmlSprite.Texture;
    
    public Property<Texture> Texture { get; }
    public Property<bool> Smooth { get; }
    public Property<bool> Repeat { get; }
    
    public AnimatableProperty<Vec2f> Size { get; }
    

    public SpriteElement(Element? parent, string imagePath, Vec2f position, Vec2f size) : base(parent)
    {
        SfmlSprite = new(new Texture(imagePath));
        
        Texture = new(this, nameof(Texture), SfmlTexture);
        Smooth = new(this, nameof(Smooth), true);
        Repeat = new(this, nameof(Repeat), false);

        Size = new(this, nameof(Size), size); 
        
        RelativePosition.Set(position);
    }


    public override void Update()
    {
        Scale.Set(CalculateScaleBasedOnSize(Size));
        
        base.Update();
    }


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();
        
        SfmlSprite.Texture = Texture.Value;
        SfmlTexture.Smooth = Smooth.Value;
        SfmlTexture.Repeated = Repeat.Value;
    }


    private Vec2f CalculateScaleBasedOnSize(Vec2f targetSize)
        => Scale.Value * targetSize / (Vec2f)GetBounds().Size;
    
    // currentScale = currentSize
    // targetScale = targetSize
    
    // currentScale * targetSize = targetScale * currentSize
    
    // targetScale = (currentScale * targetSize) / currentSize
    
    
    public override void Draw(RenderTarget target)
    {
        BeginDraw();
        target.Draw(SfmlSprite);
        EndDraw();
        
        base.Draw(target);
    }


    public override FloatRect GetBounds()
        => SfmlSprite.GetGlobalBounds();


    public override void ApplySizePolicy()
    {
        FloatRect rect = GetSizePolicyRect();
        AbsolutePosition = rect.Position;
        Size.Set(rect.Size);
    }
}