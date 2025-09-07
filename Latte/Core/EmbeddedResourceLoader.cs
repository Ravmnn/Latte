using System.Reflection;
using System.Text;
using SFML.Graphics;

using Latte.Exceptions.Core;


namespace Latte.Core;


public static class EmbeddedResourceLoader
{
    private static string? s_resourcesPath;


    public static string ResourcesPath
    {
        get => s_resourcesPath ?? throw new EmbeddedResourcesPathNotSet();
        set => s_resourcesPath = value;
    }

    public static Assembly SourceAssembly { get; set; }


    static EmbeddedResourceLoader()
    {
        ResourcesPath = "Latte.Resources";
        SourceAssembly = typeof(EmbeddedResourceLoader).Assembly;
    }


    private static string Prefix(this string resourceName)
        => $"{ResourcesPath}.{resourceName}";


    public static byte[] Load(string resourceName, Assembly? sourceAssembly = null)
    {
        var stream = (sourceAssembly ?? SourceAssembly).GetManifestResourceStream(resourceName.Prefix());
        var bytes = new byte[stream!.Length];
        stream.ReadExactly(bytes, 0, bytes.Length);

        return bytes;
    }


    public static string LoadText(string resourceName, Assembly? sourceAssembly = null)
        => Encoding.UTF8.GetString(Load(resourceName, sourceAssembly));

    public static Font LoadFont(string resourceName, Assembly? sourceAssembly = null)
        => new Font(Load(resourceName, sourceAssembly));

    public static Image LoadImage(string resourceName, Assembly? sourceAssembly = null)
        => new Image(Load(resourceName, sourceAssembly));

    public static Texture LoadTexture(string resourceName, Assembly? sourceAssembly = null)
        => new Texture(Load(resourceName, sourceAssembly));
}
