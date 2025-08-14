using System;


namespace Latte.Exceptions.Element;


public class EmptyGridException : LatteException
{
    public EmptyGridException() : base("Grid is empty.")
    {
    }

    public EmptyGridException(Exception inner) : base("Grid is empty.", inner)
    {
    }
}
