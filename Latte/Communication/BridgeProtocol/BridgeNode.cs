using System;
using System.Linq;

using Latte.Communication.BridgeProtocol.Exceptions;


namespace Latte.Communication.BridgeProtocol;


public class BridgeNode : IDisposable
{
    public const uint MinPortId = 32000;
    public const uint MaxPortId = 33000;


    private bool _disposed;


    public BridgeNodeData Data { get; }


    public BridgeNode(string processName, uint? port = null)
    {
        port ??= GenerateAvailablePort() ?? throw new NoAvailablePortsException();
        Data = new BridgeNodeData(processName, port.Value);

        ProcessesFile.AddNode(Data);
    }


    private static uint? GenerateAvailablePort()
    {
        var connectionNodes = ProcessesFile.ReadAllNodes().ToArray();

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
        {}

        // TODO: removal not working
        ProcessesFile.RemoveNode(Data.ProcessName);

        _disposed = true;
    }
}
