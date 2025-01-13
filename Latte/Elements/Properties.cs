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

    public FloatAnimation? Animation { get; protected set; }


    public FloatAnimation? Animate(object to, double time, Easing easing = Easing.Linear)
    {
        Animation?.Abort();

        Animation = Value.AnimateThis(to, time, easing);
        Animation.UpdatedEvent += (_, _) => Value = Value.AnimationValuesToThis(Animation.CurrentValues);
        Animation.FinishedEvent += (_, _) => Animation = null;

        return Animation;
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


    public FloatAnimation? Animate(T to, double time, Easing easing = Easing.Linear)
        => base.Animate(to, time, easing);

}
