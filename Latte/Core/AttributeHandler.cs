using System;
using System.Reflection;


namespace Latte.Core;


public static class AttributeHandler
{
    public static T? GetAttribute<T>(this object obj) where T : Attribute
        => obj.GetType().GetCustomAttribute<T>();
}