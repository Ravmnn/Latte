using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Latte.Debugging.Inspection;


public class OrganizedPropertyContainer : Dictionary<System.Type, List<PropertyInfo>>
{
    public OrganizedPropertyContainer(IEnumerable<System.Type> keyTypes)
        => Init(keyTypes);


    public OrganizedPropertyContainer(object @object)
    {
        var objectType = @object.GetType();

        var allBaseTypes = objectType.GetAllBaseClassesOf();
        var allInterfaces = objectType.GetInterfaces();

        var allTypes = allBaseTypes.Concat(allInterfaces).ToArray();
        var allProperties = objectType.GetPropertiesWithNoParameters();

        Init(allTypes);
        Organize(allProperties);
    }


    private void Init(IEnumerable<System.Type> keyTypes)
    {
        foreach (var type in keyTypes)
            this[type] = [];
    }


    public void Organize(IEnumerable<PropertyInfo> properties)
    {
        foreach (var property in properties)
        {
            var typeWhichDeclared = property.GetTypeWhichDeclaresProperty();

            if (typeWhichDeclared is null)
                continue;

            if (TryGetValue(typeWhichDeclared, out var propertyInfos))
                propertyInfos.Add(property);
        }
    }


    public Dictionary<System.Type, List<PropertyInfo>> GetNonEmpty()
        => (from item in this where item.Value.Count > 0 select item).ToDictionary();
}
