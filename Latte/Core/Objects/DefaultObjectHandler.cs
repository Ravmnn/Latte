using Latte.Rendering;


namespace Latte.Core.Objects;




public class DefaultObjectHandler : IObjectHandler
{
    public virtual void Update(BaseObject @object, bool mainUpdate = true)
    {
        if (@object.CanUpdate && mainUpdate)
            @object.Update();

        @object.UnconditionalUpdate();
    }




    public virtual void Draw(BaseObject @object, IRenderer renderer)
    {
        if (@object.CanDraw)
            @object.Draw(renderer);
    }
}
