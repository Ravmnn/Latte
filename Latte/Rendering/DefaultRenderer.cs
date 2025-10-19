using SFML.Graphics;

using Latte.Rendering.Exceptions;


namespace Latte.Rendering;




public class DefaultRenderer(RenderTarget renderTarget) : IRenderer
{
    public RenderTarget RenderTarget { get; set; } = renderTarget;
    public Effect? GlobalEffect { get; set; }
    public Effect? PostEffect { get; set; }
    public int PostEffectPasses { get; set; } = 1;




    public virtual void Render(Drawable drawable, Effect? drawableEffect = null, Texture? texture = null)
    {
        GlobalEffect?.UpdateUniforms(this);
        drawableEffect?.UpdateUniforms(this);

        RenderWithStates(drawable, drawableEffect ?? GlobalEffect, texture);
    }


    protected void RenderWithStates(Drawable drawable, Effect? drawableEffect = null, Texture? texture = null)
    {
        if (drawableEffect is null && texture is null)
        {
            RenderTarget.Draw(drawable);
            return;
        }

        var states = RenderStates.Default;
        states.Shader = drawableEffect ?? RenderStates.Default.Shader;
        states.Texture = texture ?? RenderStates.Default.Texture;

        RenderTarget.Draw(drawable, states);
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
