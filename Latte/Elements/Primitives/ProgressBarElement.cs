using System;

using SFML.Graphics;

using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


public class ProgressBarElement : Element
{
    private bool _wasCompleted;


    public override Transformable Transformable => Foreground.Transformable;

    protected RectangleElement Foreground { get; }
    protected RectangleElement Background { get; }

    public AnimatableProperty<Float> Progress { get; }

    public Property<Float> MinValue { get; }
    public Property<Float> MaxValue { get; }

    public bool IsAtMax => Progress.Value >= MaxValue.Value;
    public bool IsAtMin => Progress.Value <= MinValue.Value;

    public bool Completed => IsAtMax;

    public event EventHandler? CompletedEvent;


    public ProgressBarElement(Element? parent, Vec2f position, Vec2f size, float minValue = 0f, float maxValue = 1f) : base(parent)
    {
        MinValue = new(this, nameof(MinValue), minValue);
        MaxValue = new(this, nameof(MaxValue), maxValue);;

        Progress = new(this, nameof(Progress), 0f);

        RelativePosition.Set(position);

        Background = new(this, new(), size);
        Background.Color.Set(Color.Black);

        Foreground = new(this, new(), size);
        Foreground.Color.Set(Color.White);
    }


    public override void Update()
    {
        KeepProgressBetweenLimits();
        UpdateSizeBasedOnProgress();

        if (!_wasCompleted && Completed)
            CompletedEvent?.Invoke(this, EventArgs.Empty);

        _wasCompleted = Completed;

        base.Update();
    }

    private void UpdateSizeBasedOnProgress()
        => Foreground.Size.Set(new(Background.Size.Value.X * CalculateNormalizedProgress(), Background.Size.Value.Y));


    private float CalculateNormalizedProgress()
        => (Progress.Value - MinValue.Value) / (MaxValue.Value - MinValue.Value);


    private void KeepProgressBetweenLimits()
        => Progress.Set(Math.Clamp(Progress.Value, MinValue.Value, MaxValue.Value));


    public override FloatRect GetBounds()
        => Background.GetBounds(); // only foreground change its size; use background then

    public override FloatRect GetRelativeBounds()
        => Background.GetRelativeBounds();

    public override FloatRect GetBorderLessBounds()
        => Background.GetBorderLessBounds();

    public override FloatRect GetBorderLessRelativeBounds()
        => Background.GetBorderLessRelativeBounds();


    public override void ApplySizePolicy()
        => Background.ApplySizePolicy();
}
