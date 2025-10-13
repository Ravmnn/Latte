using Latte.Rendering;


namespace Latte.Core.Objects;




public static class BaseObjectExtensions
{
    public static void UpdateObject(this BaseObject @object, bool unconditionalUpdateOnly = false)
    {
        if (@object.CanUpdate && !unconditionalUpdateOnly)
            @object.Update();

        @object.UnconditionalUpdate();
    }




    public static void DrawObject(this BaseObject @object, IRenderer renderer)
    {
        if (@object.CanDraw)
            @object.Draw(renderer);
    }
}
