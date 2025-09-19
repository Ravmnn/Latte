using System;

using Latte.Exceptions;


namespace Latte.Communication.BridgeProtocol.Exceptions;


public class BridgeNodeDoesNotExistException : BridgeException
{
    private const string MessageLiteral = "Bridge node of name \"{0}\" does not exist.";


    public BridgeNodeDoesNotExistException(string name)
        : base(string.Format(MessageLiteral, name))
    {
    }

    public BridgeNodeDoesNotExistException(string name, Exception inner)
        : base(string.Format(MessageLiteral, name), inner)
    {
    }
}
