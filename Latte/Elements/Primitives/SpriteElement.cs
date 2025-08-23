using SFML.Graphics;

using Latte.Core.Type;
using Latte.Elements.Properties;


namespace Latte.Elements.Primitives;


public class SpriteElement : Element
{
    public override Transformable SfmlTransformable => SfmlSprite;
    public override Drawable SfmlDrawable => SfmlSprite;

    public Sprite SfmlSprite { get; }
    public Texture SfmlTexture => SfmlSprite.Texture;

    public Property<Texture> Texture { get; }
    public Property<bool> Smooth { get; }
    public Property<bool> Repeat { get; }

    public AnimatableProperty<Vec2f> Size { get; }


    public SpriteElement(Element? parent, string imagePath, Vec2f? position, Vec2f size) : base(parent)
    {
        SfmlSprite = new Sprite(new Texture(imagePath));

        Texture = new Property<Texture>(this, nameof(Texture), SfmlTexture);
        Smooth = new Property<bool>(this, nameof(Smooth), true);
        Repeat = new Property<bool>(this, nameof(Repeat), false);

        Size = new AnimatableProperty<Vec2f>(this, nameof(Size), size);

        SetRelativePositionOrAlignment(position);
    }


    public override void ConstantUpdate()
    {
        Scale.Set(CalculateScaleBasedOnSize(Size));

        base.ConstantUpdate();
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


    public override void BorderLessSimpleDraw(RenderTarget target) => SimpleDraw(target);


    public override FloatRect GetBounds()
        => new FloatRect(AbsolutePosition, Size.Value);

    public override FloatRect GetRelativeBounds()
        => new FloatRect(RelativePosition.Value, Size.Value);


    public override void ApplySizePolicy()
    {
        var rect = GetSizePolicyRect();
        AbsolutePosition = rect.Position;
        Size.Set(rect.Size);
    }
}
