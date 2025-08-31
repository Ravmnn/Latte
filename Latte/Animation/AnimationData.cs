using System;

using Latte.Application;


namespace Latte.Animation;


public abstract class AnimationData(double time, Easing easing = Easing.Linear, bool start = true) : IUpdateable
{
    public double Time { get; set; } = time;
    public double ElapsedTime { get; protected set; }

    public Easing Easing { get; set; } = easing;

    public float Progress { get; protected set; }
    public float EasedProgress { get; protected set; }

    public bool IsRunning { get; private set; } = start;
    public bool HasFinished => HasAborted || ElapsedTime >= Time;
    public bool HasAborted { get; protected set; }

    protected bool ShouldIgnoreUpdate => !IsRunning || HasFinished || HasAborted;

    public event EventHandler? UpdatedEvent;
    public event EventHandler? FinishedEvent;
    public event EventHandler? AbortedEvent;


    public virtual void Update()
    {
        if (ShouldIgnoreUpdate)
            return;

        ElapsedTime += App.DeltaTimeInSeconds;
        ElapsedTime = Math.Min(ElapsedTime, Time);

        UpdateProgress();

        OnUpdated();

        if (HasFinished)
            OnFinished();
    }

    private void UpdateProgress()
    {
        Progress = (float)(ElapsedTime / Time);
        EasedProgress = EasingFunctions.Ease(Progress, Easing);
    }

    public void Start() => IsRunning = true;
    public void Stop() => IsRunning = false;

    public void Abort()
    {
        HasAborted = true;
        OnAborted();
    }

    protected virtual void Reset()
    {
        ElapsedTime = 0;
        Progress = 0;
        EasedProgress = 0;
        HasAborted = false;
    }


    public override string ToString()
        => $"{(HasAborted ? "[aborted]" : "")}{ElapsedTime} | {Time} ({Progress * 100}%)";


    protected virtual void OnUpdated()
        => UpdatedEvent?.Invoke(this, EventArgs.Empty);

    protected virtual void OnFinished()
        => FinishedEvent?.Invoke(this, EventArgs.Empty);

    protected virtual void OnAborted()
        => AbortedEvent?.Invoke(this, EventArgs.Empty);
}
