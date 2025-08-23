using System;

using Latte.Application.Elements.Primitives;


namespace Latte.Application.Elements.Properties;


public abstract class Property
{
    private object _value;


    public Element Owner { get; }

    public string Name { get; }

    public object Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueChanged();
        }
    }

    public EventHandler? ValueChangedEvent;


    public Property(Element owner, string name, object value)
    {
        Owner = owner;
        Name = name;
        Value = value;

        Owner.AddProperty(this);
    }


    public void Set(object value) => Value = value;
    public object Get() => Value;


    protected virtual void OnValueChanged()
        => ValueChangedEvent?.Invoke(this, EventArgs.Empty);


    public override string ToString() => Value.ToString()!;
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
