using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

using Latte.Communication.Bridge.Exceptions;


namespace Latte.Communication.Bridge;


public enum ConnectionResponse
{
    Accepted,
    Rejected
}


public readonly record struct ConnectionResponseObject(ConnectionResponse Response);


public class BridgeConnection : TcpClient
{
    public static TimeSpan MaxWaitingTimeout { get; } = TimeSpan.FromSeconds(10);


    public StreamWriter Writer { get; private set; } = null!;
    public StreamReader Reader { get; private set; } = null!;

    public bool Origin { get; }

    public BridgeNodeData Data { get; private set; }


    // creating a bridge connection with an already existing connection
    private BridgeConnection(TcpClient client)
    {
        ConstructStreamsFrom(client.GetStream());
        Data = GetThisOwnNodeData();
    }


    // creating a connection to connect with a bridge node
    private BridgeConnection(BridgeNodeData nodeData, string targetNodeName)
    {
        Origin = true;
        Data = nodeData;

        var targetNodeData = BridgeNodesFile.GetBridgeNode(targetNodeName);

        ConnectTo(targetNodeData);
    }


    public static BridgeConnection To(BridgeNodeData nodeData, string targetNodeName)
        => new BridgeConnection(nodeData, targetNodeName);

    public static BridgeConnection From(TcpClient client)
        => new BridgeConnection(client);


    private void ConnectAndConstructStreams(int port)
    {
        Connect(IPAddress.Loopback, port);
        ConstructStreamsFrom(GetStream());
    }


    private void ConstructStreamsFrom(Stream stream)
    {
        Writer = new StreamWriter(stream) { AutoFlush = true };
        Reader = new StreamReader(stream);
    }


    private BridgeNodeData GetThisOwnNodeData()
    {
        var nodeData = ReadJsonStringAsObject<BridgeNodeData>();

        if (!BridgeNodesFile.BridgeNodeExists(nodeData.Name))
            throw new BridgeNodeDoesNotExistException(nodeData.Name);

        return nodeData;
    }


    private void ConnectTo(BridgeNodeData node)
    {
        ConnectAndConstructStreams(node.Port);

        WriteObjectAsJson(Data);
        var responseObject = ReadJsonStringAsObject<ConnectionResponseObject>();

        if (responseObject.Response == ConnectionResponse.Rejected)
            throw new BridgeConnectionRejectedException();
    }


    public T? TryReadJsonStringAsObject<T>()
    {
        try
        {
            return ReadJsonStringAsObject<T>();
        }
        catch
        {
            return default;
        }
    }

    public T ReadJsonStringAsObject<T>()
        => ReadJsonStringAsObjectAsync<T>().Result;

    public async Task<T> ReadJsonStringAsObjectAsync<T>()
    {
        var task = Reader.ReadLineAsync();
        var jsonString = await WaitForTaskWithDefaultTimeoutOrThrow(task);

        if (jsonString is null)
            throw new InvalidDataFormatException(string.Empty);

        return JsonStringAsDataFormatOrThrow<T>(jsonString);
    }


    public bool TryWriteObjectAsJson(object @object)
    {
        try
        {
            WriteObjectAsJson(@object);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void WriteObjectAsJson(object @object)
        => WriteObjectAsJsonAsync(@object).Wait();

    public async Task WriteObjectAsJsonAsync(object @object)
    {
        var json = JsonSerializer.Serialize(@object);
        var task = Writer.WriteAsync(json);

        await WaitForTaskWithDefaultTimeoutOrThrow(task);
    }


    private static async Task<T> WaitForTaskWithDefaultTimeoutOrThrow<T>(Task<T> task)
    {
        try
        {
            return await task.WaitAsync(MaxWaitingTimeout);
        }
        catch (TimeoutException)
        {
            throw new WaitingTimeoutReachedException();
        }
    }


    private static async Task WaitForTaskWithDefaultTimeoutOrThrow(Task task)
    {
        try
        {
            await task.WaitAsync(MaxWaitingTimeout);
        }
        catch (TimeoutException)
        {
            throw new WaitingTimeoutReachedException();
        }
    }


    private static T JsonStringAsDataFormatOrThrow<T>(string jsonString)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(jsonString) ?? throw new InvalidDataFormatException(jsonString);
        }
        catch (JsonException)
        {
            throw new InvalidDataFormatException(jsonString);
        }
    }
}
