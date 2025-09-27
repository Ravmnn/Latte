using SFML.Graphics;


namespace Latte.Rendering;




public interface IRenderer
{
    RenderTarget RenderTarget { get; }
    Effect? GlobalEffect { get; }




    void Render(Drawable drawable, Effect? drawableEffect = null);




    Texture GetContent();
}
