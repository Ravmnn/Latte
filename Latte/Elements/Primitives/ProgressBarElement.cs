using System;

using SFML.System;
using SFML.Graphics;

using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


public class ProgressBarElement : Element
{
    public override Transformable Transformable => Foreground.Transformable;

    public RectangleElement Foreground { get; }
    public RectangleElement Background { get; }

    private float _progress;
    public float Progress
    {
        get => _progress;
        set
        {
            if (value >= MinValue && value <= MaxValue)
                _progress = value;
            else
                throw new ArgumentOutOfRangeException(nameof(value), "Progress must be between MinValue and MaxValue.");
        }
    }

    public float MinValue { get; set; }
    public float MaxValue { get; set; }
    
    public bool IsAtMax => Progress >= MaxValue;
    public bool IsAtMin => Progress <= MinValue;

    public bool Completed => IsAtMax;
    private bool _wasCompleted;
    
    public event EventHandler? CompletedEvent;


    public ProgressBarElement(Element? parent, Vector2f position, Vector2f size, float minValue = 0f, float maxValue = 1f) : base(parent)
    {
        MinValue = minValue;
        MaxValue = maxValue;
        
        Progress = minValue;
        
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
        
        if (!_wasCompleted && Completed)
            CompletedEvent?.Invoke(this, EventArgs.Empty);
        
        float normalizedProgress = (Progress - MinValue) / (MaxValue - MinValue); 
        Foreground.Size.Set(new(Background.Size.Value.X * normalizedProgress, Background.Size.Value.Y));
        
        _wasCompleted = Completed;
        
        base.Update();
    }


    public override FloatRect GetBounds()
        => Background.GetBounds(); // only foreground change its size; use background then
}