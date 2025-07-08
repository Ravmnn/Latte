using System.Collections.Generic;
using System.Linq;

using Latte.Core;
using Latte.Elements.Primitives;


namespace Latte.Elements.Attributes;


public class ElementAttributeManager
{
    // Attributes of the current element. Instead of loading it everytime before using it,
    // store all of them here, which increases performance.
    private IEnumerable<ElementAttribute> _cachedAttributes;


    public Element Element { get; }
    public List<ElementAttribute> InheritedAttributes { get; private set; }


    public ElementAttributeManager(Element element)
    {
        _cachedAttributes = [];

        Element = element;
        InheritedAttributes = [];

        CacheAttributes();
    }

    public void CacheAttributes()
        => _cachedAttributes = GetElementAttributes();


    public void ProcessAttributes()
    {
        InheritedAttributes = [];

        if (Element.Parent is not null)
            InheritedAttributes = Element.Parent.Attributes.InheritedAttributes;

        foreach (var attribute in GetCachedElementAttributes())
        {
            if (attribute.Inherit)
                InheritedAttributes.Add(attribute);
            else
                attribute.Process(Element);
        }

        foreach (var attribute in InheritedAttributes)
            attribute.Process(Element);
    }


    public T? GetCachedElementAttribute<T>() where T : ElementAttribute
        => _cachedAttributes.OfType<T>().FirstOrDefault();

    public IEnumerable<ElementAttribute> GetCachedElementAttributes()
        => _cachedAttributes;

    public bool HasCachedElementAttribute<T>() where T : ElementAttribute
    {
        if (_cachedAttributes.Any(attribute => attribute is T))
            return true;

        return HasInheritedElementAttribute<T>();
    }


    public T? GetElementAttribute<T>() where T : ElementAttribute
        => Element.GetAttribute<T>();

    public IEnumerable<ElementAttribute> GetElementAttributes()
        => Element.GetAttributes().OfType<ElementAttribute>();

    public bool HasElementAttribute<T>() where T : ElementAttribute
    {
        if (Element.HasAttribute<T>())
            return true;

        return HasInheritedElementAttribute<T>();
    }


    private bool HasInheritedElementAttribute<T>() where T : ElementAttribute
        => InheritedAttributes.Any(attribute => attribute is T);
}
