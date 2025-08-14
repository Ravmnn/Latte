using System.Reflection;

using SFML.Graphics;


namespace Latte.Core;


public static class EmbeddedResourceLoader
{
    public static byte[] GetBytes(this Assembly assembly, string resourceName)
    {
        var stream = assembly.GetManifestResourceStream("VanillaTest.Fonts.NationalPark.ttf");
        var bytes = new byte[stream!.Length];
        stream.ReadExactly(bytes, 0, bytes.Length);

        return bytes;
    }


    public static Font LoadFont(this Assembly assembly, string resourceName)
        => new Font(assembly.GetBytes(resourceName));
}
