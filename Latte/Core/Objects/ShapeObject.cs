using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Core.Objects;




public abstract class ShapeObject(Shape shape) : BaseObject, IShape
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
}
