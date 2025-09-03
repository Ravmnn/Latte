using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;


namespace Latte.UI.Elements;


public abstract class ShapeElement : Element
{
    public override Transformable SfmlTransformable => SfmlShape;
    public override Drawable SfmlDrawable => SfmlShape;

    public Shape SfmlShape { get; }

    public float BorderSize { get; set; }

    public ColorRGBA Color { get; set; }
    public ColorRGBA BorderColor { get; set; }


    protected ShapeElement(Element? parent, Shape shape) : base(parent)
    {
        SfmlShape = shape;

        Color = SFML.Graphics.Color.White;
        BorderColor = SFML.Graphics.Color.White;
    }


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        SfmlShape.OutlineThickness = BorderSize;
        SfmlShape.FillColor = Color;
        SfmlShape.OutlineColor = BorderColor;
    }


    public override void BorderLessSimpleDraw(RenderTarget target)
    {
        SfmlShape.OutlineThickness = 0f;
        SimpleDraw(target);
        SfmlShape.OutlineThickness = BorderSize;
    }


    public override FloatRect GetBorderLessBounds()
        => GetBounds().ShrinkRect(BorderSize);

    public override FloatRect GetBorderLessRelativeBounds()
        => GetRelativeBounds().ShrinkRect(BorderSize);


    public override Vec2f GetAlignmentPosition(Alignment alignment)
        => AlignmentCalculator.ApplyBorderOffset(base.GetAlignmentPosition(alignment), BorderSize, alignment);

    public override Vec2f GetAlignmentRelativePosition(Alignment alignment)
        => AlignmentCalculator.ApplyBorderOffset(base.GetAlignmentRelativePosition(alignment), BorderSize, alignment);
}
