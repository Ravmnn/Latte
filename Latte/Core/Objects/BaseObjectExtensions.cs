using Latte.Rendering;
using Latte.Application;


namespace Latte.Core.Objects;




public static class BaseObjectExtensions
{
    public static void UpdateObject(this BaseObject @object, bool mainUpdate = true)
        => Section.GlobalObjectHandler.Update(@object, mainUpdate);


    public static void DrawObject(this BaseObject @object, IRenderer renderer)
        => Section.GlobalObjectHandler.Draw(@object, renderer);
}
