using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;


namespace Latte.Core.Application.Debugging.Inspection;


// TODO: code clean-up


public record InspectionData(string Name, string Data);


public static class Inspector
{
    public static IEnumerable<InspectionData> Inspect(object @object)
    {
        var inspectionDatas = new List<InspectionData>();
        var properties = GetOrganizedProperties(@object);

        foreach (var (type, typeProperties) in properties)
            inspectionDatas.Add(new InspectionData(type.Name, PropertiesToString(@object, typeProperties)));

        return inspectionDatas;
    }


    private static string PropertiesToString(object @object, IEnumerable<PropertyInfo> properties, string? offset = null)
    {
        var builder = new StringBuilder();

        foreach (var property in properties)
            builder.AppendLine($"{offset ?? ""}{property.Name}: {property.GetValue(@object)}"); // TODO: non-primitive objects shows the type only, without the values

        return builder.ToString();
    }


    public static Dictionary<System.Type, List<PropertyInfo>> GetOrganizedProperties(object @object)
    {
        var objectType = @object.GetType();

        var allBaseTypes = GetAllBaseClassesOf(objectType);
        var allInterfaces = objectType.GetInterfaces();

        var allTypes = allBaseTypes.Concat(allInterfaces);
        var allProperties = GetPropertiesWithNoParameters(objectType);

        return OrganizePropertiesAccordingToTheirDeclaringTypes(allTypes, allProperties);
    }


    private static Dictionary<System.Type, List<PropertyInfo>> OrganizePropertiesAccordingToTheirDeclaringTypes(IEnumerable<System.Type> types, IEnumerable<PropertyInfo> properties)
    {
        var organizedPropertyContainer = InitializeOrganizedPropertyContainer(types);

        foreach (var property in properties)
        {
            var typeWhichDeclared = GetTypeWhichDeclaresProperty(property);

            if (typeWhichDeclared is null)
                continue;

            if (organizedPropertyContainer.TryGetValue(typeWhichDeclared, out var propertyInfos))
                propertyInfos.Add(property);
        }

        return (from item in organizedPropertyContainer where item.Value.Count > 0 select item).ToDictionary();
    }


    private static System.Type? GetTypeWhichDeclaresProperty(PropertyInfo property)
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


    private static Dictionary<System.Type, List<PropertyInfo>> InitializeOrganizedPropertyContainer(IEnumerable<System.Type> types)
    {
        var organizedPropertyContainer = new Dictionary<System.Type, List<PropertyInfo>>();

        foreach (var type in types)
            organizedPropertyContainer[type] = [];

        return organizedPropertyContainer;
    }


    private static IEnumerable<PropertyInfo> GetPropertiesWithNoParameters(System.Type type)
        => from property in type.GetProperties()
            where property.GetIndexParameters().Length == 0
            select property;


    private static IEnumerable<System.Type> GetAllBaseClassesOf(System.Type type)
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
}
