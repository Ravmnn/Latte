using System.Linq;
using System.Collections.Generic;

using Latte.Core;


namespace Latte.Elements.Primitives;


public class ElementAttributeManager
{
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

        foreach (ElementAttribute attribute in GetCachedElementAttributes())
        {
            if (attribute.Inherit)
                InheritedAttributes.Add(attribute);
            else
                attribute.Process(Element);
        }

        foreach (ElementAttribute attribute in InheritedAttributes)
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


public static class ElementAttributeExtensions
{
    public static T? GetCachedElementAttribute<T>(this Element element) where T : ElementAttribute
        => element.Attributes.GetCachedElementAttribute<T>();

    public static IEnumerable<ElementAttribute> GetCachedElementAttributes(this Element element)
        => element.Attributes.GetCachedElementAttributes();

    public static bool HasCachedElementAttribute<T>(this Element element) where T : ElementAttribute
        => element.Attributes.HasCachedElementAttribute<T>();


    public static T? GetElementAttribute<T>(this Element element) where T : ElementAttribute
        => element.Attributes.GetElementAttribute<T>();

    public static IEnumerable<ElementAttribute> GetElementAttributes(this Element element)
        => element.Attributes.GetElementAttributes();

    public static bool HasElementAttribute<T>(this Element element) where T : ElementAttribute
        => element.Attributes.HasElementAttribute<T>();
}
