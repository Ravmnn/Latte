namespace Latte.Core.Animation;


/// <summary>
/// Describes an object that can be animated.
/// </summary>
public interface IAnimatable<T>
{
    AnimationState AnimateThis(T to, float time, EasingType easingType = EasingType.Linear);

    /// <summary>
    /// Convert a float[] to this type.
    /// </summary>
    /// <param name="values"> The float[]. </param>
    T AnimationValuesToThis(float[] values);
}