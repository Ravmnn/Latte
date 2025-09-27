using System.Linq;
using System.Collections.Generic;


namespace Latte.Core.Objects;




public class BaseObjectAttributeManager
{
    // Attributes of the current element. Instead of loading it everytime before using it,
    // store all of them here, which increases performance.
    protected IEnumerable<BaseObjectAttribute> CachedAttributes { get; set; }




    public BaseObject Object { get; }




    public BaseObjectAttributeManager(BaseObject @object)
    {
        CachedAttributes = [];

        Object = @object;

        CacheAttributes();
    }


    public void CacheAttributes()
        => CachedAttributes = GetObjectAttributes();




    public virtual void ProcessAttributes()
    {
        foreach (var attribute in GetCachedObjectAttributes())
            attribute.Process(Object);
    }




    public T? GetCachedObjectAttribute<T>() where T : BaseObjectAttribute
        => CachedAttributes.OfType<T>().FirstOrDefault();

    public IEnumerable<BaseObjectAttribute> GetCachedObjectAttributes()
        => CachedAttributes;

    public bool HasCachedObjectAttribute<T>() where T : BaseObjectAttribute
        => CachedAttributes.Any(attribute => attribute is T);


    public T? GetObjectAttribute<T>() where T : BaseObjectAttribute
        => Object.GetAttribute<T>();

    public IEnumerable<BaseObjectAttribute> GetObjectAttributes()
        => Object.GetAttributes().OfType<BaseObjectAttribute>();

    public bool HasObjectAttribute<T>() where T : BaseObjectAttribute
        => Object.HasAttribute<T>();
}
