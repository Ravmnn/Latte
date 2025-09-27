using System;
using System.Text.Json.Nodes;

using Latte.Exceptions;


namespace Latte.Communication.Bridge.Exceptions;




public class InvalidValidationDataFormatException : BridgeException
{
    private const string MessageLiteral = "The validation data format does not meet the protocol.";




    public JsonObject? DataObject { get; }




    public InvalidValidationDataFormatException(JsonObject? dataFormat = null) : base(MessageLiteral)
    {
        DataObject = dataFormat;
    }

    public InvalidValidationDataFormatException(JsonObject? dataFormat, Exception inner) : base(MessageLiteral, inner)
    {
        DataObject = dataFormat;
    }
}
