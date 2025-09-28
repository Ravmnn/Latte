using SFML.Graphics;


namespace Latte.Rendering;



// TODO:
// add post-processing support, which will be applied at the end of all drawing operations...
// consequently, it may be controlled by App

public interface IRenderer
{
    RenderTarget RenderTarget { get; }
    Effect? GlobalEffect { get; }
    Effect? PostEffect { get; }
    int PostEffectPasses { get; }




    void Render(Drawable drawable, Effect? drawableEffect = null);




    void ApplyPostEffect();




    Texture GetContent();
}
