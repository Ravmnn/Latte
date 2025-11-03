using SFML.Graphics;

using Latte.Core.Type;
using Latte.Rendering;


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




    public SpriteElement(Element? parent, Texture texture, Vec2f? position, Vec2f size) : base(parent)
    {
        SfmlSprite = new Sprite(texture);

        Texture = SfmlTexture;
        Smooth = true;

        Size = size;

        SetRelativePositionOrAlignment(position);
    }




    public override void UnconditionalUpdate()
    {
        Scale = CalculateScaleBasedOnSize(Size);

        base.UnconditionalUpdate();
    }




    public override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        SfmlSprite.Texture = Texture;
        SfmlTexture.Smooth = Smooth;
        SfmlTexture.Repeated = Repeat;
    }




    private Vec2f CalculateScaleBasedOnSize(Vec2f targetSize)
    {
        var oldScale = SfmlSprite.Scale;

        SfmlSprite.Scale = new Vec2f(1, 1);
        var bounds = GetBounds();
        SfmlSprite.Scale = oldScale;

        return targetSize / bounds.Size;
    }




    public override void BorderLessSimpleDraw(IRenderer renderer) => SimpleDraw(renderer);




    public override FloatRect GetBounds()
        => SfmlSprite.GetGlobalBounds();

    public override FloatRect GetRelativeBounds()
        => GetBounds() with { Left = RelativePosition.X, Top = RelativePosition.Y };




    public override void ApplySizePolicy()
    {
        var rect = GetSizePolicyRect();
        AbsolutePosition = rect.Position;
        Size = rect.Size;
    }
}
