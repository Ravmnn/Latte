using System;
using Latte.Exceptions;


namespace Latte.Communication.Bridge.Exceptions;


public class BridgeConnectionRejectedException : BridgeException
{
    private const string MessageLiteral = "Connection attempt rejected.";


    public BridgeConnectionRejectedException() : base(MessageLiteral)
    {
    }

    public BridgeConnectionRejectedException(Exception inner) : base(MessageLiteral, inner)
    {
    }
}
