using System;


namespace Latte.Exceptions;




public class LatteException : Exception
{
    public LatteException(string message) : base(message)
    {
    }

    public LatteException(string message, Exception inner) : base(message, inner)
    {
    }
}
