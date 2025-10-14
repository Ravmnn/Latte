using Latte.Rendering;


namespace Latte.Core.Objects;




public interface IObjectHandler
{
    void Update(BaseObject @object, bool mainUpdate = true);
    void Draw(BaseObject @object, IRenderer renderer);
}
