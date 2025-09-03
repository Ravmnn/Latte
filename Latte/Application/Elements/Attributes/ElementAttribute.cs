using Latte.Core;
using Latte.Application.Elements.Primitives;


namespace Latte.Application.Elements.Attributes;


public abstract class ElementAttribute : BaseObjectAttribute
{
    public sealed override void Process(BaseObject @object)
    {
        if (@object is Element element)
            Process(element);
    }

    public virtual void Process(Element element) {}
}
