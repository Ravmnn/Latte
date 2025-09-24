using SFML.Graphics;


namespace Latte.Core;


public interface IRenderer
{
    RenderTarget RenderTarget { get; }
    Effect? GlobalEffect { get; }


    void Render(Drawable drawable, Effect? drawableEffect = null);
}
