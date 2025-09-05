using SFML.Graphics;


namespace Latte.Core;


public interface IRenderer
{
    RenderTarget RenderTarget { get; }


    void Render(Drawable drawable);
}
