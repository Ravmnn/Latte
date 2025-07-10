using System;
using System.Collections.Generic;


namespace Latte.Elements.Primitives;


public static class ElementExtensions
{
    public static void ForeachElement(this IEnumerable<Element> elements, Action<Element> action)
    {
        foreach (var element in elements)
        {
            action(element);
            ForeachElement(element.Children, action);
        }
    }


    public static void ForeachParent(this Element element, Action<Element> action)
    {
        if (element.Parent is null)
            return;

        action(element.Parent);
        ForeachParent(element.Parent, action);
    }
}
