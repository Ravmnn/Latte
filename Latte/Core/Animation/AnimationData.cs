using System;

using Latte.Core.Application;


namespace Latte.Core.Animation;


public class AnimationUpdatedEventArgs(float[] currentValues) : EventArgs
{
    public float[] CurrentValues { get; set; } = currentValues;
}


/// <summary>
/// Represents an animation. <br/>
/// Note that this class works with an array of floats. The reason for that
/// it's because it's an approach for animating structures that have more than one
/// data property. For example, animating a single float number isn't a problem... but
/// what about a Color or a Vector2f? these have more than one value inside of them.
/// By using an array of floats, animating structures like these becomes easier and
/// more efficient that other ways. Each array index represents a property member of the
/// structure: <br/><br/>
/// 
/// In case of Vector2f: new AnimationState(startVec.X, startVec.Y], [endVec.X, endVec.Y], 1f); <br/>
/// In case of Color: new AnimationState([startColor.R, startColor.G, startColor.B], [endColor.R, endColor.G, endColor.B], 1f);
/// 
/// And so on...
/// </summary>
/// <param name="startValues"> The start values. </param>
/// <param name="endValues"> The final values. </param>
/// <param name="time"> The time the animation will take to finish. </param>
public class AnimationData(float[] startValues, float[] endValues, double time, Easing easing = Easing.Linear) : IUpdateable
{
    public float[] StartValues { get; } = startValues;
    public float[] EndValues { get; } = endValues;
    public float[] CurrentValues { get; private set; } = new float[startValues.Length];

    public double Time { get; } = time;
    public double ElapsedTime { get; private set; }

    public Easing Easing { get; } = easing;

    /// <summary>
    /// How much the animation has progressed from 0 to 1.
    /// </summary>
    public float Progress { get; private set; }
    
    /// <summary>
    /// Same as this.Progress, but with eased by this.Easing.
    /// </summary>
    public float EasedProgress { get; private set; }

    public bool HasFinished => ElapsedTime >= Time;
    public bool HasAborted { get; private set; }
    public bool Paused { get; set; }

    private bool ShouldIgnoreUpdate => HasFinished || HasAborted || Paused;


    public event EventHandler<AnimationUpdatedEventArgs>? UpdatedEvent;
    public event EventHandler? FinishedEvent;
    public event EventHandler? AbortedEvent;

    
    /// <summary>
    /// Updates the animation.
    /// </summary>
    public void Update()
    {
        if (ShouldIgnoreUpdate)
            return;

        OnUpdated(new(CurrentValues));

        if (HasFinished)
            OnFinished();
    }
    

    private void UpdateProgress()
    {
        ElapsedTime += App.DeltaTimeInSeconds;
        Progress = (float)(ElapsedTime / Time);

        EasedProgress = EasingFunctions.Ease(Progress, Easing);
    }


    public void Abort()
    {
        HasAborted = true;
        OnAborted();
    }
    
    
    private void OnUpdated(AnimationUpdatedEventArgs eventArgs)
    {
        UpdateProgress();
        UpdateCurrentValues();

        if (HasFinished)
            eventArgs.CurrentValues = CurrentValues = EndValues;

        UpdatedEvent?.Invoke(this, eventArgs);
    }

    private void UpdateCurrentValues()
    {
        for (uint i = 0; i < StartValues.Length; i++)
            CurrentValues[i] = StartValues[i] + (EndValues[i] - StartValues[i]) * EasedProgress;
    }
    

    private void OnFinished()
        => FinishedEvent?.Invoke(this, EventArgs.Empty);
    
    private void OnAborted()
        => AbortedEvent?.Invoke(this, EventArgs.Empty);
}