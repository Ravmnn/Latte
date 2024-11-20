namespace Latte.Core.Animation;



/// <summary>
/// Describes an object that can be animated.
/// </summary>
public interface IAnimatable
{
    AnimationState AnimateThis(object to, double time, EasingType easingType = EasingType.Linear);

    
    /// <summary>
    /// Convert a float[] to this type.
    /// </summary>
    /// <param name="values"> The float[]. </param>
    IAnimatable AnimationValuesToThis(float[] values);
}


/// <summary>
/// Generic version of IAnimatable.
/// </summary>
public interface IAnimatable<T> : IAnimatable where T : notnull
{
    AnimationState AnimateThis(T to, double time, EasingType easingType = EasingType.Linear)
        => (this as IAnimatable).AnimateThis(to, time, easingType);
    
    
    new T AnimationValuesToThis(float[] values)
        => (T)(this as IAnimatable).AnimationValuesToThis(values);
}