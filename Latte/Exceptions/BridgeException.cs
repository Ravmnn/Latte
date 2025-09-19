using System;


namespace Latte.Exceptions;


public class BridgeException : LatteException
{
    public BridgeException(string message) : base(message)
    {
    }

    public BridgeException(string message, Exception inner) : base(message, inner)
    {
    }
}
