using System.Collections.Generic;


namespace Latte.Core.Objects;


public static class BaseObjectAttributeExtensions
{
    public static T? GetCachedObjectAttribute<T>(this BaseObject @object) where T : BaseObjectAttribute
        => @object.Attributes.GetCachedObjectAttribute<T>();

    public static IEnumerable<BaseObjectAttribute> GetCachedObjectAttributes(this BaseObject @object)
        => @object.Attributes.GetCachedObjectAttributes();

    public static bool HasCachedObjectAttribute<T>(this BaseObject @object) where T : BaseObjectAttribute
        => @object.Attributes.HasCachedObjectAttribute<T>();


    public static T? GetObjectAttribute<T>(this BaseObject @object) where T : BaseObjectAttribute
        => @object.Attributes.GetObjectAttribute<T>();

    public static IEnumerable<BaseObjectAttribute> GetObjectAttributes(this BaseObject @object)
        => @object.Attributes.GetObjectAttributes();

    public static bool HasObjectAttribute<T>(this BaseObject @object) where T : BaseObjectAttribute
        => @object.Attributes.HasObjectAttribute<T>();
}
