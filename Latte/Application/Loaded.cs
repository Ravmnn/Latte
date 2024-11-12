using SFML.Graphics;


namespace Latte.Application;


public static class Loaded
{
    public static Shader ClipShader { get; }


    static Loaded()
    {
        ClipShader = new(null, null, "../../../../resources/shaders/clip.frag");
    }
}