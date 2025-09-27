using System;

using Latte.Core;
using Latte.Application;


using Math = System.Math;


namespace Latte.Tweening;




public abstract class TweenAnimation(double time, Easing easing = Easing.Linear, bool start = true) : IUpdateable
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




    public event EventHandler? UpdateEvent;
    public event EventHandler? FinishEvent;
    public event EventHandler? AbortEvent;




    public virtual void Update()
    {
        if (ShouldIgnoreUpdate)
            return;

        ElapsedTime += DeltaTime.Seconds;
        ElapsedTime = Math.Min(ElapsedTime, Time);

        UpdateProgress();

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




    protected virtual void OnFinished()
        => FinishEvent?.Invoke(this, EventArgs.Empty);

    protected virtual void OnAborted()
        => AbortEvent?.Invoke(this, EventArgs.Empty);
}
