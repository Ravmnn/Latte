namespace Latte.Animation;



/// <summary>
/// Describes an object that can be animated.
/// </summary>
public interface IAnimatable
{
    FloatAnimation AnimateThis(object to, double time, Easing easing = Easing.Linear);


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
    T Get();


    FloatAnimation AnimateThis(T to, double time, Easing easing = Easing.Linear);

    FloatAnimation IAnimatable.AnimateThis(object to, double time, Easing easing)
        => AnimateThis((T)to, time, easing);


    new T AnimationValuesToThis(float[] values)
        => (T)(this as IAnimatable).AnimationValuesToThis(values);
}
