using System;

namespace Latte.Exceptions.Element;


public class ElementException : Exception
{
    public ElementException()
    {
    }

    public ElementException(string message) : base(message)
    {
    }

    public ElementException(string message, Exception inner) : base(message, inner)
    {
    }
}
