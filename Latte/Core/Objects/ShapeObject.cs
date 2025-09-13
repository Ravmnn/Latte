using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Core.Objects;


// TODO: use primary constructor whenever possible
public abstract class ShapeObject(Shape shape) : BaseObject
{
    public override Transformable SfmlTransformable => SfmlShape;
    public override Drawable SfmlDrawable => SfmlShape;

    // TODO: the below properties can be moved to an interface IShape
    // TODO: also consider the creation of ISfmlObject, implementing the two props above and UpdateSfmlProperties method
    public Shape SfmlShape { get; } = shape;

    public float BorderSize { get; set; }

    public ColorRGBA Color { get; set; } = SFML.Graphics.Color.White;
    public ColorRGBA BorderColor { get; set; } = SFML.Graphics.Color.White;


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        SfmlShape.OutlineThickness = BorderSize;
        SfmlShape.FillColor = Color;
        SfmlShape.OutlineColor = BorderColor;
    }
}
