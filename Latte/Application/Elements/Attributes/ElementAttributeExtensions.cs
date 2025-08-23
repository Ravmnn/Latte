using System.Collections.Generic;

using Latte.Application.Elements.Primitives;


namespace Latte.Application.Elements.Attributes;


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
