using System;
using System.Diagnostics;


namespace Latte.Application;




public static class DeltaTime
{
    private static TimeSpan s_lastTime = TimeSpan.Zero;
    private static TimeSpan s_currentTime = TimeSpan.Zero;

    private static TimeSpan RawDeltaTime => s_currentTime - s_lastTime;


    public static Stopwatch TimeCounter { get; } = new Stopwatch();


    public static TimeSpan FromStart => TimeCounter.Elapsed;
    public static double FromStartSeconds => FromStart.TotalSeconds;
    public static double FromStartMilliseconds => FromStart.TotalMilliseconds;




    public static double MaxDeltaTime { get; set; } = DeltaTimeFromFPS(20);


    public static TimeSpan Span
    {
        get
        {
            var dt = RawDeltaTime;

            if (dt.TotalSeconds > MaxDeltaTime)
                dt = TimeSpan.FromSeconds(MaxDeltaTime);

            return dt;
        }
    }

    public static double Seconds => Span.TotalSeconds;
    public static double Milliseconds => Span.TotalMilliseconds;


    public static double FramesPerSecond => FPSFromDeltaTime(RawDeltaTime.TotalSeconds);




    public static void Start()
        => TimeCounter.Start();

    public static void Stop()
        => TimeCounter.Start();




    public static void Update()
    {
        s_lastTime = s_currentTime;
        s_currentTime = TimeCounter.Elapsed;
    }




    public static double FPSFromDeltaTime(double dt)
        => 1 / dt;

    public static double DeltaTimeFromFPS(double fps)
        => 1 / fps;
}
