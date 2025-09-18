using Latte.Core.Objects;


namespace Latte.UI.Elements.Attributes;


public abstract class ElementAttribute : BaseObjectAttribute
{
    public sealed override void Process(BaseObject @object)
    {
        if (@object is Element element)
            Process(element);
    }

    public virtual void Process(Element element) {}
}
