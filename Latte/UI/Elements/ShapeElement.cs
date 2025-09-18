using SFML.Graphics;

using Latte.Core;
using Latte.Core.Objects;
using Latte.Core.Type;


namespace Latte.UI.Elements;


public abstract class ShapeElement(Element? parent, Shape shape) : Element(parent), IShape
{
    public override Transformable SfmlTransformable => SfmlShape;
    public override Drawable SfmlDrawable => SfmlShape;

    public Shape SfmlShape { get; } = shape;

    public float BorderSize { get; set; }

    public ColorRGBA Color { get; set; } = SFML.Graphics.Color.White;
    public ColorRGBA BorderColor { get; set; } = SFML.Graphics.Color.White;


    public override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        SfmlShape.OutlineThickness = BorderSize;
        SfmlShape.FillColor = Color;
        SfmlShape.OutlineColor = BorderColor;
    }


    public override void BorderLessSimpleDraw(IRenderer renderer)
    {
        SfmlShape.OutlineThickness = 0f;
        SimpleDraw(renderer);
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
