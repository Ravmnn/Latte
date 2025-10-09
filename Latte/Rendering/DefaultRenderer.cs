using SFML.Graphics;

using Latte.Rendering.Exceptions;


namespace Latte.Rendering;




public class DefaultRenderer(RenderTarget renderTarget) : IRenderer
{
    public RenderTarget RenderTarget { get; set; } = renderTarget;
    public Effect? GlobalEffect { get; set; }
    public Effect? PostEffect { get; set; }
    public int PostEffectPasses { get; set; } = 1;




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




    public void ApplyPostEffect()
    {
        if (PostEffect is null)
            return;

        PostEffect.UpdateUniforms(this);

        for (var i = 0; i < PostEffectPasses; i++)
        {
            var content = GetContent();
            var sprite = new Sprite(content);

            Render(sprite, PostEffect);
        }
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
