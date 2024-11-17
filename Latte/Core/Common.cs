using SFML.Graphics;


namespace Latte.Core;


public interface IUpdateable
{
    void Update();
}


public interface IDrawable
{
    void Draw(RenderTarget target);
}