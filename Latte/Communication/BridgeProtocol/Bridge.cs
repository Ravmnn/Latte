using System.IO;


namespace Latte.Communication.BridgeProtocol;


public static class Bridge
{
    public const int MinPortId = 32000;
    public const int MaxPortId = 33000;

    public const string ProcessesFileName = "Processes.json";

    public static string BridgeTempPath { get; } = Path.Combine(Path.GetTempPath(), "Bridge");
    public static string ProcessesFilePath { get; } = Path.Combine(BridgeTempPath, ProcessesFileName);


    public static void Init(string processName)
    {
        // TODO: finish
    }
}
