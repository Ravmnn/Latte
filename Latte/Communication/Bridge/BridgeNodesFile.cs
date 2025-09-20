using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Latte.Communication.Bridge.Exceptions;


namespace Latte.Communication.Bridge;


public static class BridgeNodesFile
{
    public const string ProcessesFileName = "BridgeNodes";


    public static string ApplicationData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    public static string BridgePath { get; } = Path.Combine(ApplicationData, "Latte/Bridge");
    public static string ProcessesFilePath { get; } = Path.Combine(BridgePath, ProcessesFileName);


    static BridgeNodesFile()
    {
        Directory.CreateDirectory(BridgePath);
    }


    public static BridgeNodeData GetBridgeNode(string name)
    {
        if (!BridgeNodeExists(name))
            throw new BridgeNodeDoesNotExistException(name);

        var bridgeNodes = ReadAllBridgeNodes();

        return bridgeNodes.First(bridgeNode => bridgeNode.Name == name);
    }


    public static void AddBridgeNode(BridgeNodeData bridgeNode)
    {
        var bridgeNodes = ReadAllBridgeNodes().ToList();

        if (BridgeNodeExists(bridgeNode.Name))
            throw new BridgeNodeAlreadyExistsException(bridgeNode.Name);

        bridgeNodes.Add(bridgeNode);

        WriteBridgeNodes(bridgeNodes);
    }


    public static void RemoveBridgeNode(string processName)
    {
        var bridgeNodes = ReadAllBridgeNodes().ToList();
        bridgeNodes.RemoveAll(bridgeNode => bridgeNode.Name == processName);

        WriteBridgeNodes(bridgeNodes);
    }


    public static bool BridgeNodeExists(string name)
        => ReadAllBridgeNodes().Any(bridgeNode => bridgeNode.Name == name);


    public static void WriteBridgeNodes(IEnumerable<BridgeNodeData> bridgeNodes)
    {
        using var writer = new BinaryWriter(File.Open(ProcessesFilePath, FileMode.OpenOrCreate));

        // clear content
        writer.BaseStream.SetLength(0);
        writer.BaseStream.Position = 0;

        foreach (var bridgeNode in bridgeNodes)
            WriteBridgeNodeToBinary(writer, bridgeNode);
    }


    public static IEnumerable<BridgeNodeData> ReadAllBridgeNodes()
    {
        using var reader = new BinaryReader(new FileStream(ProcessesFilePath, FileMode.OpenOrCreate));
        var bridgeNodes = new List<BridgeNodeData>();

        while (reader.BaseStream.Position < reader.BaseStream.Length)
            bridgeNodes.Add(ReadBridgeNodeFromBinary(reader));

        return bridgeNodes;
    }


    private static void WriteBridgeNodeToBinary(BinaryWriter writer, BridgeNodeData bridgeNode)
    {
        writer.Write(bridgeNode.Name);
        writer.Write(bridgeNode.Port);
    }


    private static BridgeNodeData ReadBridgeNodeFromBinary(BinaryReader reader)
    {
        var processName = reader.ReadString();
        var port = reader.ReadInt32();

        return new BridgeNodeData(processName, port);
    }
}
