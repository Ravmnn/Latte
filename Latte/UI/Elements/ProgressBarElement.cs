using System;

using SFML.Graphics;

using Latte.Core.Type;
using Latte.Rendering;


namespace Latte.UI.Elements;




public class ProgressBarElement : Element
{
    private bool _wasCompleted;




    public override Transformable SfmlTransformable => Foreground.SfmlTransformable;
    public override Drawable SfmlDrawable => Foreground.SfmlDrawable;




    protected RectangleElement Foreground { get; }
    protected RectangleElement Background { get; }




    public float Progress { get; set; }

    public bool IsAtMax => Progress >= MaxValue;
    public bool IsAtMin => Progress <= MinValue;

    public float MinValue { get; set; }
    public float MaxValue { get; set; }

    public bool Completed => IsAtMax;


    public event EventHandler? CompletedEvent;




    public ProgressBarElement(Element? parent, Vec2f? position, Vec2f size, float minValue = 0f, float maxValue = 1f) : base(parent)
    {
        MinValue = minValue;
        MaxValue = maxValue;

        SetRelativePositionOrAlignment(position);

        Background = new RectangleElement(this, new Vec2f(), size)
        {
            Color = Color.Black
        };

        Foreground = new RectangleElement(this, new Vec2f(), size)
        {
            Color = Color.White
        };
    }




    public override void ConstantUpdate()
    {
        ClampProgress();
        UpdateSizeBasedOnProgress();

        if (!_wasCompleted && Completed)
            CompletedEvent?.Invoke(this, EventArgs.Empty);

        _wasCompleted = Completed;

        base.ConstantUpdate();
    }


    private void UpdateSizeBasedOnProgress()
        => Foreground.Size = new Vec2f(Background.Size.X * CalculateNormalizedProgress(), Background.Size.Y);


    private float CalculateNormalizedProgress()
        => ProgressBarMath.CalculateNormalizedProgress(Progress, MinValue, MaxValue);


    private void ClampProgress()
        => Progress = Math.Clamp(Progress, MinValue, MaxValue);




    public override void BorderLessSimpleDraw(IRenderer renderer)
        => Foreground.BorderLessSimpleDraw(renderer);




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
