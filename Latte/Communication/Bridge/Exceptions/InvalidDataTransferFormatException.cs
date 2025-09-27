using System;
using System.Text.Json.Nodes;

using Latte.Exceptions;


namespace Latte.Communication.Bridge.Exceptions;




public class InvalidDataTransferFormatException : BridgeException
{
    private const string MessageLiteral = "The data format does not meet the protocol.";




    public JsonObject? DataObject { get; }




    public InvalidDataTransferFormatException(JsonObject? dataFormat = null) : base(MessageLiteral)
    {
        DataObject = dataFormat;
    }

    public InvalidDataTransferFormatException(JsonObject? dataFormat, Exception inner) : base(MessageLiteral, inner)
    {
        DataObject = dataFormat;
    }
}
