using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;


namespace Latte.Core.Application.Debugging.Inspection;


public static class ReflectionExtensions
{
    public static System.Type? GetTypeWhichDeclaresProperty(this PropertyInfo property)
    {
        // use this method instead of only PropertyInfo.DeclaringType, since it doesn't know
        // when an interface is the declaring type.

        if (property.DeclaringType is null)
            return null;

        var baseInterfaces = property.DeclaringType.GetInterfaces();

        foreach (var baseInterface in baseInterfaces)
            foreach (var baseInterfaceProperty in baseInterface.GetProperties())
                if (baseInterfaceProperty.Name == property.Name)
                    return baseInterface;

        return property.DeclaringType;
    }


    public static IEnumerable<PropertyInfo> GetPropertiesWithNoParameters(this System.Type type)
        => from property in type.GetProperties()
            where property.GetIndexParameters().Length == 0
            select property;


    public static IEnumerable<System.Type> GetAllBaseClassesOf(this System.Type type)
    {
        var baseClasses = new List<System.Type> { type };
        var current = type;

        while (current.BaseType is not null)
        {
            current = current.BaseType;
            baseClasses.Add(current);
        }

        return baseClasses;
    }


    public static void ForeachProperty(this System.Type type, Action<PropertyInfo> action)
    {
        var properties = type.GetPropertiesWithNoParameters();

        foreach (var property in properties)
            action(property);
    }
}
