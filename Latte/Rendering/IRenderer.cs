using SFML.Graphics;


namespace Latte.Rendering;




public interface IRenderer
{
    RenderTarget RenderTarget { get; }
    Effect? GlobalEffect { get; }
    Effect? PostEffect { get; }
    int PostEffectPasses { get; }




    void Render(Drawable drawable, Effect? drawableEffect = null, Texture? texture = null);




    void ApplyPostEffect();




    Texture GetContent();
}
