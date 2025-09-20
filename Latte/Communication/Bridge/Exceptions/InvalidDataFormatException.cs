using System;

using Latte.Exceptions;


namespace Latte.Communication.Bridge.Exceptions;


public class InvalidDataFormatException : BridgeException
{
    private const string MessageLiteral = "The data format does not meet the protocol.";

    
    public string DataFormatFormat { get; }
    

    public InvalidDataFormatException(string dataFormat) : base(MessageLiteral)
    {
        DataFormatFormat = dataFormat;
    }

    public InvalidDataFormatException(string dataFormat, Exception inner) : base(MessageLiteral, inner)
    {
        DataFormatFormat = dataFormat;
    }
}
