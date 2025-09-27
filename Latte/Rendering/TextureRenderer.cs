using SFML.Graphics;


using SfSprite = SFML.Graphics.Sprite;


namespace Latte.Rendering;




public class TextureRenderer : DefaultRenderer
{
    public RenderTexture RenderTexture => (RenderTarget as RenderTexture)!;
    public SfSprite RenderTextureSprite { get; }




    public TextureRenderer(RenderTexture renderTexture) : base(renderTexture)
    {
        RenderTextureSprite = new SfSprite(RenderTexture.Texture);
    }


    public override Texture GetContent()
        => RenderTexture.Texture;
}
