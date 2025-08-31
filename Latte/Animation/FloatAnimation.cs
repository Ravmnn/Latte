namespace Latte.Animation;


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
public class FloatAnimation(float[] startValues, float[] endValues, double time, Easing easing = Easing.Linear, bool start = true)
    : AnimationData(time, easing, start)
{
    public float[] StartValues { get; } = startValues;
    public float[] EndValues { get; } = endValues;
    public float[] CurrentValues { get; private set; } = new float[startValues.Length];


    private void UpdateCurrentValues()
    {
        for (uint i = 0; i < StartValues.Length; i++)
            CurrentValues[i] = StartValues[i] + (EndValues[i] - StartValues[i]) * EasedProgress;
    }


    protected override void OnUpdated()
    {
        UpdateCurrentValues();

        if (HasFinished)
            CurrentValues = EndValues;

        base.OnUpdated();
    }
}
