using SFML.Graphics;


namespace Latte.Application;


public interface IDrawable
{
    void Draw(RenderTarget target);
}
