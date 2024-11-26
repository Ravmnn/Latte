using System;

using Latte.Core;
using Latte.Elements.Primitives;


namespace Latte.Elements;


[AttributeUsage(AttributeTargets.Class)]
public class CanOnlyHaveChildOfTypeAttribute(Type type) : Attribute
{
    public Type Type { get; } = type;


    public static void Check(Element element)
    {
        if (element.GetAttribute<CanOnlyHaveChildOfTypeAttribute>() is not { } attribute)
            return;
        
        foreach (Element child in element.Children)
            if (child.GetType() != attribute.Type)
                throw new InvalidOperationException($"The element \"{element.GetType().Name}\" can only have children of type: \"{attribute.Type.Name}\"");
    }
}