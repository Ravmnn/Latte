namespace Latte.Tweening;




/// <summary>
/// Describes an object that can be animated.
/// </summary>
public interface IFloatArrayModifiable
{
    /// <summary>
    /// Convert a float[] to this type.
    /// </summary>
    /// <param name="values"> The float[]. </param>
    void ModifyFrom(float[] values);
}
