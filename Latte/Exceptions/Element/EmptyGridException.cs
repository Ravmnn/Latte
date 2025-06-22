using System;


namespace Latte.Exceptions.Element;


public class EmptyGridException : Exception
{
    public EmptyGridException() : base("Grid is empty.")
    {
    }

    public EmptyGridException(Exception inner) : base("Grid is empty.", inner)
    {
    }
}
