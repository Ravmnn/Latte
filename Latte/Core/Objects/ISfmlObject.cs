using SFML.Graphics;

using Latte.Core.Type;
using Latte.UI;


namespace Latte.Core.Objects;


public interface ISfmlObject : IBounds
{
    Transformable SfmlTransformable { get; }
    Drawable SfmlDrawable { get; }

    Vec2f Position { get; set; }
    Vec2f Origin { get; set; }
    float Rotation { get; set; }
    Vec2f Scale { get; set; }


    void UpdateSfmlProperties();
}
