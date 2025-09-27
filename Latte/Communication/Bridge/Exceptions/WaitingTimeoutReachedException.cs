using System;

using Latte.Exceptions;


namespace Latte.Communication.Bridge.Exceptions;




public class WaitingTimeoutReachedException : BridgeException
{
    private const string MessageLiteral = "Max waiting time has been reached.";



    public WaitingTimeoutReachedException() : base(MessageLiteral)
    {
    }

    public WaitingTimeoutReachedException(Exception inner) : base(MessageLiteral, inner)
    {
    }
}
