using Latte.Core.Animation;
using Latte.Elements.Primitives;


namespace Latte.Elements;


public abstract class Property
{
    public Element Owner { get; }
    
    public string Name { get; }
    public object Value { get; set; }
    
    
    public Property(Element owner, string name, object value)
    {
        Owner = owner;
        Name = name;
        Value = value;
        
        Owner.AddProperty(this);
    }
    
    
    public void Set(object value) => Value = value;
    public object Get() => Value;
}


public class Property<T>(Element owner, string name, T value) : Property(owner, name, value) where T : notnull
{
    public new T Value
    {
        get => (T)base.Value;
        set => base.Value = value;
    }
    
    
    public void Set(T value) => Value = value;
    public new T Get() => Value;
    
    
    public static implicit operator T(Property<T> property) => property.Get();
}




public abstract class AnimatableProperty(Element owner, string name, object value) : Property(owner, name, value)
{
    public new IAnimatable Value
    {
        get => (IAnimatable)base.Value;
        set => base.Value = value;
    }
    
    public AnimationData? Animation { get; protected set; }

    public bool CanAnimate { get; set; } = true;
    
    
    public void Animate(object to, double time, Easing easing = Easing.Linear)
    {
        if (!CanAnimate)
            return;
        
        Animation?.Abort();

        Animation = Value.AnimateThis(to, time, easing);
        Animation.UpdatedEvent += (_, args) => Value = Value.AnimationValuesToThis(args.CurrentValues);
        Animation.FinishedEvent += (_, _) => Animation = null;
    }
}


public class AnimatableProperty<T>(Element owner, string name, T value) : AnimatableProperty(owner, name, value)
    where T : IAnimatable<T>
{
    public new T Value
    {
        get => (T)base.Value;
        set => base.Value = value;
    }
    
    
    public void Set(T value) => Value = value;
    public new T Get() => Value;
    
    
    public static implicit operator T(AnimatableProperty<T> property) => property.Get();
    
    
    public void Animate(T to, double time, Easing easing = Easing.Linear)
        => base.Animate(to, time, easing);
    
}