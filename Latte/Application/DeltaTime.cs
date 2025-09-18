using System;
using System.Diagnostics;


namespace Latte.Application;


public static class DeltaTime
{
    public static Stopwatch Counter { get; } = new Stopwatch();

    public static TimeSpan Span { get; private set; } = TimeSpan.Zero;
    public static double Seconds => Span.TotalSeconds;
    public static int Milliseconds => Span.Milliseconds;

    public static double FramesPerSecond => 1f / Seconds;


    public static void Start() => Counter.Start();
    public static void Stop() => Counter.Stop();


    public static void Update()
    {
        Span = Counter.Elapsed;
        Counter.Restart();
    }
}
