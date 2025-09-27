using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Latte.Exceptions;
using Latte.Communication.Bridge.Exceptions;


namespace Latte.Communication.Bridge;




public readonly record struct BridgeNodeData(string Name, int Port);




public class BridgeConnectionEventArgs(BridgeConnection connection) : EventArgs
{
    public BridgeConnection Connection { get; } = connection;
}

public class TcpClientEventArgs(TcpClient client) : EventArgs
{
    public TcpClient Client { get; } = client;
}




public class BridgeNode : IDisposable
{
    public const int MinPortId = 32000;
    public const int MaxPortId = 33000;




    private bool _disposed;


    private readonly Thread _listenForConnectionsThread;
    private readonly CancellationTokenSource _listenForConnectionsThreadCancellation;




    public BridgeNodeData Data { get; }


    protected TcpListener Server { get; }




    public event EventHandler<TcpClientEventArgs>? ConnectionRequestedEvent;
    public event EventHandler<BridgeConnectionEventArgs>? ConnectionAcceptedEvent;
    public event EventHandler<BridgeConnectionEventArgs>? ConnectionRejectedEvent;
    public event EventHandler<TcpClientEventArgs>? ConnectionFailedEvent;




    public BridgeNode(string name, int? port = null)
    {
        port ??= GenerateAvailablePort() ?? throw new NoAvailablePortsException();
        Data = new BridgeNodeData(name, port.Value);

        BridgeNodesFile.AddBridgeNode(Data);

        Server = new TcpListener(IPAddress.Loopback, Data.Port);
        Server.Start();


        _listenForConnectionsThreadCancellation = new CancellationTokenSource();

        _listenForConnectionsThread = new Thread(ListenForConnectionRequests) { IsBackground = true };
        _listenForConnectionsThread.Start();
    }




    public void ConnectTo(string targetNodeName)
    {
        var connection = BridgeConnection.ToTarget(Data, targetNodeName);
    }




    private void ListenForConnectionRequests()
    {
        while (true)
        {
            try
            {
                ListenForConnectionRequest();
            }
            catch (AggregateException)
            {
                break;
            }
        }
    }


    private void ListenForConnectionRequest()
    {
        var task = Server.AcceptTcpClientAsync(_listenForConnectionsThreadCancellation.Token).AsTask();
        task.Wait();

        OnConnectionRequested(task.Result);
    }




    private void ValidateConnectionRequest(TcpClient client)
    {
        try
        {
            var connection = BridgeConnection.FromOrigin(Data, client);

            // TODO: connection rejection logic is not developed yet
            // OnConnectionRejected(connection);

            // TODO:
            // create logic of disconnection.
            // if client cannot write to server, then the server is disconnected
            // if server cannot read from client, then the client is disconnected

            OnConnectionAccepted(connection);
        }
        catch (BridgeException)
        {
            OnConnectionFailed(client);
        }
    }




    protected virtual void OnConnectionRequested(TcpClient client)
    {
        ConnectionRequestedEvent?.Invoke(this, new TcpClientEventArgs(client));

        ValidateConnectionRequest(client);
    }


    protected virtual void OnConnectionAccepted(BridgeConnection connection)
    {
        var response = new ConnectionResponseObject(ConnectionResponse.Accepted).ToJsonString();
        connection.TrySendString(response);

        ConnectionAcceptedEvent?.Invoke(this, new BridgeConnectionEventArgs(connection));
    }


    protected virtual void OnConnectionRejected(BridgeConnection connection)
    {
        var response = new ConnectionResponseObject(ConnectionResponse.Rejected).ToJsonString();
        connection.TrySendString(response);

        ConnectionRejectedEvent?.Invoke(this, new BridgeConnectionEventArgs(connection));
    }


    protected virtual void OnConnectionFailed(TcpClient client)
    {
        ConnectionFailedEvent?.Invoke(this, new TcpClientEventArgs(client));
    }




    private static int? GenerateAvailablePort()
    {
        var connectionNodes = BridgeNodesFile.ReadAllBridgeNodes().ToArray();

        for (var port = MinPortId; port <= MaxPortId; port++)
            if (connectionNodes.All(connectionNode => connectionNode.Port != port))
                return port;

        return null;
    }




    ~BridgeNode()
    {
        Dispose(false);
    }




    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            Server.Dispose();
        }

        // TODO: remove invalid bridge nodes (those that doesn't exist anymore) from BridgeNodes file

        BridgeNodesFile.RemoveBridgeNode(Data.Name);

        _listenForConnectionsThreadCancellation.Cancel();
        _listenForConnectionsThread.Join();

        _disposed = true;
    }
}
