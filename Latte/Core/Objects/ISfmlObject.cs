using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Core.Objects;


public interface ISfmlObject
{
    Transformable SfmlTransformable { get; }
    Drawable SfmlDrawable { get; }

    Vec2f Position { get; set; }
    Vec2f Origin { get; set; }
    float Rotation { get; set; }
    Vec2f Scale { get; set; }


    void UpdateSfmlProperties();
}
