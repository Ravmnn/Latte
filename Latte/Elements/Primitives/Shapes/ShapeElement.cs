using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Elements.Behavior;
using Latte.Elements.Properties;


namespace Latte.Elements.Primitives.Shapes;


public abstract class ShapeElement : Element
{
    public override Transformable Transformable => SfmlShape;

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


    public override void Draw(RenderTarget target)
    {
        BeginDraw();
        target.Draw(SfmlShape);
        EndDraw();

        base.Draw(target);
    }


    public override FloatRect GetBorderLessBounds()
        => GetBounds().ShrinkRect(BorderSize.Value);

    public override FloatRect GetBorderLessRelativeBounds()
        => GetRelativeBounds().ShrinkRect(BorderSize.Value);


    // using bounds with borders causes a bug
    public override FloatRect GetSizePolicyRect(SizePolicyType policyType)
        => SizePolicyCalculator.CalculateChildRect(GetBorderLessBounds(), GetParentBounds(), policyType);
}
