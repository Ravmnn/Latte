using System.Text.Json;
using System.Text.Json.Nodes;

using Latte.Communication.Bridge.Exceptions;


namespace Latte.Communication.Bridge;


public enum DataTransferType
{
    None,

    Ping,
    Pong
}


public struct DataTransferObject(DataTransferType type, JsonObject? data = null)
{
    public DataTransferType Type { get; } = type;
    public JsonObject? Data { get; } = data;


    public DataTransferObject(string jsonString) : this(DataTransferType.None)
    {
        var data = JsonSerializer.Deserialize<DataTransferObject>(jsonString);

        Type = data.Type;
        Data = data.Data;
    }


    public T DataAs<T>()
    {
        try
        {
            return Data.Deserialize<T>() ?? throw new InvalidDataTransferFormatException(Data);
        }
        catch (JsonException)
        {
            throw new InvalidDataTransferFormatException(Data);
        }
    }


    public string ToJsonString()
        => JsonSerializer.Serialize(this);
}
