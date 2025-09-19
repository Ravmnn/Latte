using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Latte.Communication.BridgeProtocol.Exceptions;


namespace Latte.Communication.Bridge;


public readonly record struct BridgeNodeData(string Name, uint Port);


public class BridgeNode : IDisposable
{
    public const uint MinPortId = 32000;
    public const uint MaxPortId = 33000;


    private bool _disposed;

    private readonly Thread _listenForConnectionsThread;
    private readonly CancellationTokenSource _listenForConnectionsThreadCancellation;


    public BridgeNodeData Data { get; }

    protected TcpListener Server { get; }


    public BridgeNode(string name, uint? port = null)
    {
        port ??= GenerateAvailablePort() ?? throw new NoAvailablePortsException();
        Data = new BridgeNodeData(name, port.Value);

        BridgeNodesFile.AddBridgeNode(Data);

        Server = new TcpListener(IPAddress.Loopback, (int)Data.Port);
        Server.Start();


        _listenForConnectionsThreadCancellation = new CancellationTokenSource();

        _listenForConnectionsThread = new Thread(ListenForConnections) { IsBackground = true };
        _listenForConnectionsThread.Start();
    }


    public void ConnectTo(string bridgeNodeName)
    {
        var bridgeNode = BridgeNodesFile.GetBridgeNode(bridgeNodeName);

        var client = new TcpClient();
        client.Connect(IPAddress.Loopback, (int)bridgeNode.Port);
    }


    private void ListenForConnections()
    {
        while (true)
        {
            try
            {
                var task = Server.AcceptTcpClientAsync(_listenForConnectionsThreadCancellation.Token).AsTask();
                task.Wait();

                Console.WriteLine("Client connected");
            }
            catch (AggregateException)
            {
                Console.WriteLine("Client connection thread cancelled");
                break;
            }
        }
    }


    private static uint? GenerateAvailablePort()
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
