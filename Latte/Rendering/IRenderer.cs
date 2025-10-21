using SFML.Graphics;


namespace Latte.Rendering;




public interface IRenderer
{
    RenderTarget RenderTarget { get; set; }
    Effect? GlobalEffect { get; set; }
    Effect? PostEffect { get; set; }
    int PostEffectPasses { get; set; }




    void Render(Drawable drawable, Effect? drawableEffect = null, Texture? texture = null);




    void ApplyPostEffect();
    void ApplyEffect(Effect effect);




    Texture GetContent();
}
