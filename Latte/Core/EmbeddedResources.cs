using System.Reflection;

using SFML.Graphics;


namespace Latte.Core;


public static class EmbeddedResources
{
    public static Assembly LatteAssembly => typeof(EmbeddedResources).Assembly;

    public static string DefaultFontName => "Roboto-Regular.ttf";


    public static Font DefaultFont()
        => EmbeddedResourceLoader.LoadFont($"Fonts.{DefaultFontName}", LatteAssembly);
}
