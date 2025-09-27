using System;
using System.Diagnostics;


namespace Latte.Application;




public static class DeltaTime
{
    public static Stopwatch TimeCounter { get; } = new Stopwatch();
    public static Stopwatch DeltaTimeCounter { get; } = new Stopwatch();


    public static TimeSpan FromStart => TimeCounter.Elapsed;
    public static double FromStartSeconds => FromStart.TotalSeconds;
    public static double FromStartMilliseconds => FromStart.TotalMilliseconds;

    public static TimeSpan Span { get; private set; } = TimeSpan.Zero;
    public static double Seconds => Span.TotalSeconds;
    public static double Milliseconds => Span.TotalMilliseconds;


    public static double FramesPerSecond => 1f / Seconds;




    public static void Start()
    {
        TimeCounter.Start();
        DeltaTimeCounter.Start();
    }


    public static void Stop()
    {
        TimeCounter.Start();
        DeltaTimeCounter.Stop();
    }




    public static void Update()
    {
        Span = DeltaTimeCounter.Elapsed;
        DeltaTimeCounter.Restart();
    }
}
