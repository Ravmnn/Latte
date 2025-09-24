using System.Reflection;
using System.Collections.Generic;


namespace Latte.Core;


public static class AttributeExtensions
{
    public static T? GetAttribute<T>(this object obj) where T : System.Attribute
        => obj.GetType().GetCustomAttribute<T>();

    public static IEnumerable<object> GetAttributes(this object obj, bool inherit = true)
        => obj.GetType().GetCustomAttributes(inherit);

    public static bool HasAttribute<T>(this object obj) where T : System.Attribute
        => obj.GetAttribute<T>() is not null;


    public static T? GetAttribute<T>(this MemberInfo member) where T : System.Attribute
        => member.GetCustomAttribute<T>();

    public static IEnumerable<object> GetAttributes(this MemberInfo member, bool inherit = true)
        => member.GetCustomAttributes(inherit);

    public static bool HasAttribute<T>(this MemberInfo member) where T : System.Attribute
        => member.GetAttribute<T>() is not null;
}
