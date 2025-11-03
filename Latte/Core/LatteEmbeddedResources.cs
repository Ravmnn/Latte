using System.Reflection;

using SFML.Graphics;


namespace Latte.Core;




public static class LatteEmbeddedResources
{
    public static Assembly LatteAssembly => typeof(LatteEmbeddedResources).Assembly;
    public static string ResourcesPath => "Latte.Resources";


    public static string DefaultFontPath => $"{ResourcesPath}.Fonts.Roboto-Regular.ttf";




    public static Font DefaultFont()
        => EmbeddedResourceLoader.LoadFont(DefaultFontPath, LatteAssembly);
}
