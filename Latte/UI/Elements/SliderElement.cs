using System;

using Latte.Core.Type;


namespace Latte.UI.Elements;


public class SliderElement : ButtonElement
{
    private float _value;
    public SliderHandleElement Handle { get; protected set; }

    public Orientation Orientation => Handle.Orientation;

    // TODO: these two cannot be equal, launch exception if it's the case
    public float Minimum { get; set; }
    public float Maximum { get; set; }
    public float Value
    {
        get => _value;
        set => _value = Math.Clamp(value, Minimum, Maximum);
    }

    public float NormalizedValue => CalculateNormalizedValue();

    public float Proportion => CalculateProportion();


    public SliderElement(Element? parent, Vec2f? position, float min, float max, Orientation orientation = Orientation.Horizontal)
        : base(parent, position, new Vec2f(), null)
    {
        Minimum = min;
        Maximum = max;

        Handle = new SliderHandleElement(this, orientation);

        Size = Orientation == Orientation.Horizontal ? new Vec2f(200, 5) : new Vec2f(5, 200);
    }


    private float CalculateProportion()
        => (Maximum - Minimum) / (Orientation == Orientation.Horizontal ? Size.X : Size.Y);

    private float CalculateNormalizedValue()
        => (Value - Minimum) / (Maximum - Minimum);
}
