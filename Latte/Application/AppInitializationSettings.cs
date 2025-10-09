using SFML.Window;
using SFML.Graphics;

using Latte.Core;


namespace Latte.Application;




public readonly struct AppInitializationSettings
{
    public static ContextSettings DefaultContextSettings => new ContextSettings
    {
        AntialiasingLevel = 2,
        DepthBits = 24,
        StencilBits = 8,
        MinorVersion = 3
    };


    public static AppInitializationSettings Default => new AppInitializationSettings
    {
        DefaultFont = EmbeddedResources.DefaultFont(),
        WindowStyle = Styles.Default,
        ContextSettings = DefaultContextSettings
    };




    public required Font DefaultFont { get; init; }
    public required Styles WindowStyle { get; init; }
    public required ContextSettings ContextSettings { get; init; }
}
