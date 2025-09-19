using System;

using Latte.Exceptions;


namespace Latte.Communication.BridgeProtocol.Exceptions;


public class NoAvailablePortsException : BridgeException
{
    private const string MessageLiteral = "No available ports, max number of Latte processes reached.";


    public NoAvailablePortsException() : base(MessageLiteral)
    {
    }

    public NoAvailablePortsException(Exception inner) : base(MessageLiteral, inner)
    {
    }
}
