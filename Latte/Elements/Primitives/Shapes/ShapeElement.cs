using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Elements.Behavior;
using Latte.Elements.Properties;


namespace Latte.Elements.Primitives.Shapes;


public abstract class ShapeElement : Element
{
    public override Transformable SfmlTransformable => SfmlShape;
    public override Drawable SfmlDrawable => SfmlShape;

    public Shape SfmlShape { get; }

    public AnimatableProperty<Float> BorderSize { get; }

    public AnimatableProperty<ColorRGBA> Color { get; }
    public AnimatableProperty<ColorRGBA> BorderColor { get; }


    protected ShapeElement(Element? parent, Shape shape) : base(parent)
    {
        SfmlShape = shape;

        BorderSize = new AnimatableProperty<Float>(this, nameof(BorderSize), 0f);

        Color = new AnimatableProperty<ColorRGBA>(this, nameof(Color), SFML.Graphics.Color.White);
        BorderColor = new AnimatableProperty<ColorRGBA>(this, nameof(BorderColor), SFML.Graphics.Color.White);
    }


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        SfmlShape.OutlineThickness = BorderSize.Value;
        SfmlShape.FillColor = Color.Value;
        SfmlShape.OutlineColor = BorderColor.Value;
    }


    public override void BorderLessSimpleDraw(RenderTarget target)
    {
        SfmlShape.OutlineThickness = 0f;
        SimpleDraw(target);
        SfmlShape.OutlineThickness = BorderSize.Value;
    }


    public override FloatRect GetBorderLessBounds()
        => GetBounds().ShrinkRect(BorderSize.Value);

    public override FloatRect GetBorderLessRelativeBounds()
        => GetRelativeBounds().ShrinkRect(BorderSize.Value);
}
