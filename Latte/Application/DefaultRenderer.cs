using SFML.Graphics;

using Latte.Core;


namespace Latte.Application;




public class DefaultRenderer(RenderTarget renderTarget) : IRenderer
{
    public RenderTarget RenderTarget { get; set; } = renderTarget;
    public Effect? GlobalEffect { get; set; }




    public virtual void Render(Drawable drawable, Effect? drawableEffect = null)
    {
        GlobalEffect?.UpdateUniforms(this);
        drawableEffect?.UpdateUniforms(this);

        RenderWithEffect(drawable, drawableEffect ?? GlobalEffect);
    }


    protected void RenderWithEffect(Drawable drawable, Effect? drawableEffect = null)
    {
        if (drawableEffect is not null)
            RenderTarget.Draw(drawable, new RenderStates(drawableEffect));
        else
            RenderTarget.Draw(drawable);
    }
}
