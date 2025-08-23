using Latte.Application.Elements.Primitives;


namespace Latte.Application.Elements.Attributes;


public abstract class ElementAttribute(bool inherit = false) : System.Attribute
{
    public bool Inherit { get; } = inherit;


    public virtual void Process(Element element) {}
}
