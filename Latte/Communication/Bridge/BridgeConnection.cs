using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Latte.Communication.Bridge.Exceptions;


namespace Latte.Communication.Bridge;




public enum ConnectionResponse
{
    Accepted,
    Rejected
}


public readonly record struct ConnectionResponseObject(ConnectionResponse Response);




public class DataEventArgs(string data) : EventArgs
{
    public string Data { get; } = data;
}




// TODO: encapsulate TcpClient instead of using inheritance
public class BridgeConnection : TcpClient
{
    public static TimeSpan MaxWaitingTimeout { get; } = TimeSpan.FromSeconds(6);




    public StreamWriter Writer { get; private set; } = null!;
    public StreamReader Reader { get; private set; } = null!;


    public bool IsOrigin { get; }

    public BridgeNodeData Origin { get; private set; }
    public BridgeNodeData Target { get; private set; }

    public BridgeNodeData Sender => IsOrigin ? Origin : Target;
    public BridgeNodeData Receiver => IsOrigin ? Target : Origin;




    public EventHandler<DataEventArgs>? SentEvent;
    public EventHandler<DataEventArgs>? ReceivedEvent;




    // creating to receive an external connection
    private BridgeConnection(BridgeNodeData target, TcpClient origin)
    {
        ConstructStreamsFrom(origin.GetStream());

        Target = target;
        Origin = GetValidationData();
    }


    // creating to connect with a bridge node
    private BridgeConnection(BridgeNodeData origin, string targetName)
    {
        IsOrigin = true;

        Origin = origin;
        Target = BridgeNodesFile.GetBridgeNode(targetName);

        ConnectTo(Target);
    }




    public static BridgeConnection ToTarget(BridgeNodeData origin, string targetName)
        => new BridgeConnection(origin, targetName);

    public static BridgeConnection FromOrigin(BridgeNodeData target, TcpClient origin)
        => new BridgeConnection(target, origin);




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




    private void ConnectTo(BridgeNodeData target)
    {
        ConnectAndConstructStreams(target.Port);

        SendValidationData();
        var response = ReceiveValidationResponse();

        if (response is null || response.Value.Response == ConnectionResponse.Rejected)
            throw new BridgeConnectionRejectedException();
    }




    private BridgeNodeData GetValidationData()
    {
        var dataString = ReceiveString();
        var origin = dataString.JsonStringAs<BridgeNodeData?>();

        if (origin is null)
            throw new InvalidValidationDataFormatException();

        // TODO: check if both name and port are valid... like BridgeNodesFile.IsBridgeNodeValid(name)
        if (!BridgeNodesFile.BridgeNodeExists(origin.Value.Name))
            throw new BridgeNodeDoesNotExistException(origin.Value.Name);

        return origin.Value;
    }


    private void SendValidationData()
    {
        var data = Origin.ToJsonObject()!;
        SendString(data.ToJsonString());
    }



    private ConnectionResponseObject? ReceiveValidationResponse()
    {
        var responseData = ReceiveString();
        return responseData.JsonStringAs<ConnectionResponseObject?>();
    }




    public DataTransferObject? TryReceiveData()
    {
        try
        {
            return ReceiveData();
        }
        catch
        {
            return null;
        }
    }


    public DataTransferObject ReceiveData()
        => ReceiveDataAsync().Result;


    public async Task<DataTransferObject> ReceiveDataAsync()
    {
        var task = Reader.ReadLineAsync();
        var jsonString = await WaitForTaskWithDefaultTimeoutOrThrow(task);

        if (jsonString is null)
            throw new InvalidDataTransferFormatException(null);

        return new DataTransferObject(jsonString);
    }




    public string? TryReceiveString()
    {
        try
        {
            return ReceiveString();
        }
        catch
        {
            return null;
        }
    }


    public string ReceiveString()
        => ReceiveStringAsync().Result;


    public async Task<string> ReceiveStringAsync()
    {
        var task = Reader.ReadLineAsync();
        var data = await WaitForTaskWithDefaultTimeoutOrThrow(task);

        OnReceived(data ?? string.Empty);

        if (data is null)
            throw new InvalidDataTransferFormatException();

        return data;
    }




    public bool TrySendData(DataTransferObject data)
    {
        try
        {
            SendData(data);
            return true;
        }
        catch
        {
            return false;
        }
    }


    public void SendData(DataTransferObject data)
        => SendDataAsync(data).Wait();


    public async Task SendDataAsync(DataTransferObject data)
    {
        var json = data.ToJsonString();
        await SendStringAsync(json);
    }




    public bool TrySendString(string data)
    {
        try
        {
            SendString(data);
            return true;
        }
        catch
        {
            return false;
        }
    }


    public void SendString(string data)
        => SendStringAsync(data).Wait();


    public async Task SendStringAsync(string data)
    {
        OnSent(data);

        var task = Writer.WriteLineAsync(data);
        await WaitForTaskWithDefaultTimeoutOrThrow(task);
    }




    protected virtual void OnSent(string data)
    {
        SentEvent?.Invoke(this, new DataEventArgs(data));
    }


    protected virtual void OnReceived(string data)
    {
        ReceivedEvent?.Invoke(this, new DataEventArgs(data));
    }




    public void Ping()
    {

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
}
