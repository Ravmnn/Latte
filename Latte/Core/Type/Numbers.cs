using Latte.Core.Animation;


namespace Latte.Core.Type;


public struct Float(float value) : IAnimatable<Float>
{
    public float Value { get; set; } = value;


    public Float Get() => this;


    public static implicit operator float(Float @float) => @float.Value;
    public static implicit operator Float(float @float) => new(@float);


    public readonly FloatAnimation AnimateThis(Float to, double time, Easing easing = Easing.Linear)
        => Animate.Value(Value, to, time, easing);

    public readonly IAnimatable AnimationValuesToThis(float[] values)
        => (Float)values.ToValue();


    public readonly override string ToString() => $"{Value}";
}
