using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Core.Objects;




public class CircleObject : ShapeObject
{
    public const uint DefaultPointCount = 32;




    public new CircleShape SfmlShape => (base.SfmlShape as CircleShape)!;




    public float Radius { get; set; }




    public CircleObject(Vec2f position, float radius)
        : base(new CircleShape(radius, DefaultPointCount))
    {
        Position = position;
        Radius = radius;
    }


    public override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        SfmlShape.Radius = Radius;
    }


    public override FloatRect GetBounds()
        => new FloatRect(Position, new Vec2f(Radius * 2, Radius * 2));
}
