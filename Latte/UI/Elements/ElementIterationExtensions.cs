using System;
using System.Collections.Generic;


namespace Latte.UI.Elements;




public static class ElementIterationExtensions
{
    public static void ForeachElementRecursively(this IEnumerable<Element> elements, Action<Element> action)
    {
        foreach (var element in elements)
        {
            action(element);
            ForeachElementRecursively(element.Children, action);
        }
    }
}
