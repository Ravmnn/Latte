using System;

namespace Latte.Exceptions.Element;


public class ElementException : LatteException
{
    public ElementException(string message) : base(message)
    {
    }

    public ElementException(string message, Exception inner) : base(message, inner)
    {
    }
}
