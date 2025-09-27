using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;


namespace Latte.UI.Elements;




public class SpriteElement : Element
{
    public override Transformable SfmlTransformable => SfmlSprite;
    public override Drawable SfmlDrawable => SfmlSprite;




    public Sprite SfmlSprite { get; }
    public Texture SfmlTexture => SfmlSprite.Texture;


    public Texture Texture { get; set; }
    public bool Smooth { get; set; }
    public bool Repeat { get; set; }

    public Vec2f Size { get; set; }




    public SpriteElement(Element? parent, string imagePath, Vec2f? position, Vec2f size) : base(parent)
    {
        SfmlSprite = new Sprite(new Texture(imagePath));

        Texture = SfmlTexture;
        Smooth = true;

        Size = size;

        SetRelativePositionOrAlignment(position);
    }




    public override void ConstantUpdate()
    {
        Scale = CalculateScaleBasedOnSize(Size);

        base.ConstantUpdate();
    }




    public override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        SfmlSprite.Texture = Texture;
        SfmlTexture.Smooth = Smooth;
        SfmlTexture.Repeated = Repeat;
    }




    private Vec2f CalculateScaleBasedOnSize(Vec2f targetSize)
        => Scale * targetSize / (Vec2f)GetBounds().Size;

    // currentScale = currentSize
    // targetScale = targetSize

    // currentScale * targetSize = targetScale * currentSize

    // targetScale = (currentScale * targetSize) / currentSize




    public override void BorderLessSimpleDraw(IRenderer renderer) => SimpleDraw(renderer);




    public override FloatRect GetBounds()
        => new FloatRect(AbsolutePosition, Size);

    public override FloatRect GetRelativeBounds()
        => new FloatRect(RelativePosition, Size);




    public override void ApplySizePolicy()
    {
        var rect = GetSizePolicyRect();
        AbsolutePosition = rect.Position;
        Size = rect.Size;
    }
}
