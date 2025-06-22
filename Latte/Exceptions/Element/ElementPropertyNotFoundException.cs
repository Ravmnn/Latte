using System;


namespace Latte.Exceptions.Element;


public class ElementPropertyNotFoundException : Exception
{
    public string PropertyName { get; }


    public ElementPropertyNotFoundException(string propertyName) : base("Element property not found.")
    {
        PropertyName = propertyName;
    }

    public ElementPropertyNotFoundException(string propertyName, Exception inner) : base("Element property not found.", inner)
    {
        PropertyName = propertyName;
    }


    public override string ToString()
        => $"{Message} (\"{PropertyName}\")";
}
