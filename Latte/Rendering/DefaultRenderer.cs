using SFML.Graphics;

using Latte.Rendering.Exceptions;


namespace Latte.Rendering;




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




    public virtual Texture GetContent()
    {
        switch (RenderTarget)
        {
            case RenderWindow window:
            {
                var content = new Texture(RenderTarget.Size.X, RenderTarget.Size.Y);
                content.Update(window);

                return content;
            }

            case RenderTexture texture:
                return texture.Texture;

            default:
                throw new InvalidRenderTargetException();
        }
    }
}
