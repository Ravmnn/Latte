using Latte.Core.Animation;


namespace Latte.Core.Type;


public struct Float(float value) : IAnimatable<Float>
{
    public float Value { get; set; } = value;


    public static implicit operator float(Float @float) => @float.Value;
    public static implicit operator Float(float @float) => new(@float);

    
    public readonly AnimationState AnimateThis(Float to, float time, EasingType easingType = EasingType.Linear)
        => Animate.Value(Value, to, time, easingType);

    public readonly Float AnimationValuesToThis(float[] values)
        => values.ToValue();
    

    public readonly override string ToString() => $"{Value}";
}