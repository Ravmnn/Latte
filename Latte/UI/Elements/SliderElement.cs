using System;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;


namespace Latte.UI.Elements;




public class SliderElement : ButtonElement
{
    public SliderHandleElement Handle { get; protected set; }


    public Orientation Orientation => Handle.Orientation;


    public float Minimum { get; set; }
    public float Maximum { get; set; }


    private float _value;
    public float Value
    {
        get => _value;
        set => _value = Math.Clamp(value, Minimum, Maximum);
    }

    public float NormalizedValue => ProgressBarMath.CalculateNormalizedProgress(Value, Minimum, Maximum);


    public float StepFactor => CalculateStepFactor();




    public SliderElement(Element? parent, Vec2f? position, float min, float max, Orientation orientation = Orientation.Horizontal)
        : base(parent, position, new Vec2f(), null)
    {
        Minimum = min;
        Maximum = max;

        Value = Minimum;

        Handle = new SliderHandleElement(this, orientation);

        Size = Orientation == Orientation.Horizontal ? new Vec2f(200, 5) : new Vec2f(5, 200);
    }




    public override FloatRect GetBounds()
        => Rect.GetBoundsOfRects([base.GetBounds(), Handle.GetBounds()]);




    private float CalculateStepFactor()
        => (Maximum - Minimum) / Math.Max(1, Orientation == Orientation.Horizontal ? Size.X : Size.Y);
}
