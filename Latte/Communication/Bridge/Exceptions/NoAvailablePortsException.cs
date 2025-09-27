using System;

using Latte.Exceptions;


namespace Latte.Communication.Bridge.Exceptions;




public class NoAvailablePortsException : BridgeException
{
    private const string MessageLiteral = "No available ports, max number of bridge nodes reached.";




    public NoAvailablePortsException() : base(MessageLiteral)
    {
    }

    public NoAvailablePortsException(Exception inner) : base(MessageLiteral, inner)
    {
    }
}
