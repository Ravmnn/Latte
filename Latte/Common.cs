using SFML.Graphics;


namespace Latte;


public interface IUpdateable
{
    void Update();
}


public interface IDrawable
{
    void Draw(RenderTarget target);
}