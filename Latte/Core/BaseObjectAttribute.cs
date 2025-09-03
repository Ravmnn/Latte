namespace Latte.Core;


public abstract class BaseObjectAttribute : System.Attribute
{
    public virtual void Process(BaseObject @object) {}
}
