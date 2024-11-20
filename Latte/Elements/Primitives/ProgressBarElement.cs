using System;

using SFML.Graphics;

using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


public class ProgressBarElement : Element
{
    public override Transformable Transformable => Foreground.Transformable;

    public RectangleElement Foreground { get; }
    public RectangleElement Background { get; }
    
    public AnimatableProperty<Float> Progress { get; }

    public Property<Float> MinValue { get; set; }
    public Property<Float> MaxValue { get; set; }
    
    public bool IsAtMax => Progress.Value >= MaxValue.Value;
    public bool IsAtMin => Progress.Value <= MinValue.Value;

    public bool Completed => IsAtMax;
    private bool _wasCompleted;
    
    public event EventHandler? CompletedEvent;


    public ProgressBarElement(Element? parent, Vec2f position, Vec2f size, float minValue = 0f, float maxValue = 1f) : base(parent)
    {
        MinValue = new(this, nameof(MinValue), minValue);
        MaxValue = new(this, nameof(MaxValue), maxValue);;
        
        Progress = new(this, nameof(Progress), 0f);
        
        Position.Set(position);

        Background = new(this, new(), size);
        Background.Color.Set(Color.Black);

        Foreground = new(this, new(), size);
        Foreground.Color.Set(Color.White);
    }


    public override void Update()
    {
        if (!Visible)
            return;
        
        // keeps progress between the value limits
        Progress.Set(Math.Clamp(Progress.Value, MinValue.Value, MaxValue.Value));
        
        if (!_wasCompleted && Completed)
            CompletedEvent?.Invoke(this, EventArgs.Empty);
        
        float normalizedProgress = (Progress.Value - MinValue.Value) / (MaxValue.Value - MinValue.Value); 
        Foreground.Size.Set(new(Background.Size.Value.X * normalizedProgress, Background.Size.Value.Y));
        
        _wasCompleted = Completed;
        
        base.Update();
    }


    public override FloatRect GetBounds()
        => Background.GetBounds(); // only foreground change its size; use background then
}