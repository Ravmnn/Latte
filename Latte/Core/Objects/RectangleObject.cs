using SFML.Graphics;

using Latte.Core.Type;
using Latte.Sfml;


namespace Latte.Core.Objects;




public class RectangleObject : ShapeObject
{
    protected const uint DefaultPointCount = 16;




    public new RoundedRectangleShape SfmlShape => (base.SfmlShape as RoundedRectangleShape)!;




    public Vec2f Size { get; set; }
    public float Radius { get; set; }




    public RectangleObject(Vec2f position, Vec2f size, float radius = 0f)
        : base(new RoundedRectangleShape(size, radius, DefaultPointCount))
    {
        Position = position;

        Size = size;
        Radius = radius;
    }




    public override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        SfmlShape.Size = Size;
        SfmlShape.Radius = Radius;
    }




    public override FloatRect GetBounds()
        => new FloatRect(Position, Size * Scale).ExpandRect(BorderSize);
}
