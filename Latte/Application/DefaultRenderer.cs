using SFML.Graphics;

using Latte.Core;


namespace Latte.Application;


public class DefaultRenderer(RenderTarget renderTarget) : IRenderer
{
    public RenderTarget RenderTarget { get; set; } = renderTarget;


    public virtual void Render(Drawable drawable)
    {
        RenderTarget.Draw(drawable);
    }
}
