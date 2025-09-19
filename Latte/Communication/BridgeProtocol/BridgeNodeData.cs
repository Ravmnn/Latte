namespace Latte.Communication.BridgeProtocol;


public struct BridgeNodeData(string processName, uint port)
{
    public string ProcessName { get; } = processName;
    public uint Port { get; } = port;
}
