namespace Latte.Core.Animation;



/// <summary>
/// Describes an object that can be animated.
/// </summary>
public interface IAnimatable
{
    AnimationData AnimateThis(object to, double time, Easing easing = Easing.Linear);

    
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
    AnimationData AnimateThis(T to, double time, Easing easing = Easing.Linear);
    
    AnimationData IAnimatable.AnimateThis(object to, double time, Easing easing)
        => AnimateThis((T)to, time, easing);
    
    
    new T AnimationValuesToThis(float[] values)
        => (T)(this as IAnimatable).AnimationValuesToThis(values);
}