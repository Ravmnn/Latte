using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


namespace Latte.Communication.BridgeProtocol;


public static class ProcessesFile
{
    public const string ProcessesFileName = "BridgeNodes";


    public static string ApplicationData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    public static string BridgePath { get; } = Path.Combine(ApplicationData, "Latte/Bridge");
    public static string ProcessesFilePath { get; } = Path.Combine(BridgePath, ProcessesFileName);


    static ProcessesFile()
    {
        Directory.CreateDirectory(BridgePath);
    }


    public static void AddNode(BridgeNodeData bridgeNode)
    {
        var connectionNodes = ReadAllNodes().ToList();
        connectionNodes.Add(bridgeNode);

        WriteNodes(connectionNodes);
    }


    public static void RemoveNode(string processName)
    {
        var connectionNodes = ReadAllNodes().ToList();
        connectionNodes.RemoveAll(connectionNode => connectionNode.ProcessName == processName);

        WriteNodes(connectionNodes);
    }


    public static void WriteNodes(IEnumerable<BridgeNodeData> connectionNodes)
    {
        using var writer = new BinaryWriter(File.Open(ProcessesFilePath, FileMode.OpenOrCreate));

        foreach (var connectionNode in connectionNodes)
            WriteNodeToBinary(writer, connectionNode);
    }


    public static IEnumerable<BridgeNodeData> ReadAllNodes()
    {
        using var reader = new BinaryReader(new FileStream(ProcessesFilePath, FileMode.OpenOrCreate));
        var connectionNodes = new List<BridgeNodeData>();

        while (reader.BaseStream.Position < reader.BaseStream.Length)
            connectionNodes.Add(ReadNodeFromBinary(reader));

        return connectionNodes;
    }


    private static void WriteNodeToBinary(BinaryWriter writer, BridgeNodeData bridgeNode)
    {
        writer.Write(bridgeNode.ProcessName);
        writer.Write(bridgeNode.Port);
    }


    private static BridgeNodeData ReadNodeFromBinary(BinaryReader reader)
    {
        var processName = reader.ReadString();
        var port = reader.ReadUInt32();

        return new BridgeNodeData(processName, port);
    }
}
