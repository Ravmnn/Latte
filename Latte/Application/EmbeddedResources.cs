using System.Reflection;

using SFML.Graphics;

using Latte.Core;


namespace Latte.Application;


public static partial class EmbeddedResources
{
    public static string FontPath => "Latte.Resources.Fonts";

    public static string DefaultFontName => "Roboto-Regular.ttf";

    public static Assembly LatteAssembly => typeof(EmbeddedResourceLoader).Assembly;


    public static Font DefaultFont()
        => LatteAssembly.LoadFont($"{FontPath}.{DefaultFontName}");
}
