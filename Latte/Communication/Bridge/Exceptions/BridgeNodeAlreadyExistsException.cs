using System;

using Latte.Exceptions;


namespace Latte.Communication.Bridge.Exceptions;


public class BridgeNodeAlreadyExistsException : BridgeException
{
    private const string MessageLiteral = "Bridge node of name \"{0}\" already exists.";


    public BridgeNodeAlreadyExistsException(string name)
        : base(string.Format(MessageLiteral, name))
    {
    }

    public BridgeNodeAlreadyExistsException(string name, Exception inner)
        : base(string.Format(MessageLiteral, name), inner)
    {
    }
}
