using System.IO;
using System.Text;
using System.Reflection;

using SFML.Graphics;

using Latte.Rendering;


namespace Latte.Core;




public static class EmbeddedResourceLoader
{
    public static Assembly DefaultSourceAssembly { get; set; }
    public static string DefaultResourcesPath { get; set; }




    static EmbeddedResourceLoader()
    {
        DefaultSourceAssembly = typeof(EmbeddedResourceLoader).Assembly;
        DefaultResourcesPath = "Latte.Resources";
    }




    private static string Prefix(this string resourceName)
        => $"{DefaultResourcesPath}.{resourceName}";




    public static byte[] Load(string resourceName, Assembly? sourceAssembly = null)
    {
        resourceName = sourceAssembly is not null ? resourceName : resourceName.Prefix();

        var stream = (sourceAssembly ?? DefaultSourceAssembly).GetManifestResourceStream(resourceName);
        var bytes = new byte[stream!.Length];
        stream.ReadExactly(bytes, 0, bytes.Length);

        return bytes;
    }


    public static MemoryStream LoadAsStream(string resourceName, Assembly? sourceAssembly = null)
        => new MemoryStream(Load(resourceName, sourceAssembly));




    public static string LoadText(string resourceName, Assembly? sourceAssembly = null)
        => Encoding.UTF8.GetString(Load(resourceName, sourceAssembly));


    public static Font LoadFont(string resourceName, Assembly? sourceAssembly = null)
        => new Font(Load(resourceName, sourceAssembly));


    public static Image LoadImage(string resourceName, Assembly? sourceAssembly = null)
        => new Image(Load(resourceName, sourceAssembly));


    public static Texture LoadTexture(string resourceName, Assembly? sourceAssembly = null)
        => new Texture(Load(resourceName, sourceAssembly));


    public static Effect LoadEffect(string fragmentResourceName, string vertexResourceName, Assembly? sourceAssembly = null)
        => new Effect(LoadAsStream(fragmentResourceName, sourceAssembly), LoadAsStream(vertexResourceName, sourceAssembly));


    public static Effect LoadFragmentEffect(string resourceName, Assembly? sourceAssembly = null)
        => new Effect(LoadAsStream(resourceName, sourceAssembly));


    public static Effect LoadVertexEffect(string resourceName, Assembly? sourceAssembly = null)
        => new Effect(null, LoadAsStream(resourceName, sourceAssembly));
}
