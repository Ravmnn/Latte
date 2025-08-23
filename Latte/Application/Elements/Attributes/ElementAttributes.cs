using System;

using Latte.Application.Elements.Primitives;
using Latte.Exceptions.Element;


namespace Latte.Application.Elements.Attributes;


[AttributeUsage(AttributeTargets.Class)]
public class ChildrenTypeAttribute(Type type) : ElementAttribute
{
    public Type Type { get; } = type;


    public override void Process(Element element)
    {
        foreach (var child in element.Children)
            if (child.GetType() != Type)
                throw new ElementException($"The element \"{element.GetType().Name}\" can only have children of type: \"{Type.Name}\"");
    }
}


[AttributeUsage(AttributeTargets.Class)]
public class ChildrenAmountAttribute(uint amount) : ElementAttribute
{
    public uint Amount { get; } = amount;


    public override void Process(Element element)
    {
        if (element.Children.Count > Amount)
            throw new ElementException($"The element \"{element.GetType().Name}\" can only have {Amount} children.");
    }
}
