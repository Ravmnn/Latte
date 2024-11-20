using System;

using Latte.Sfml;
using Latte.Core;
using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


using Math = Latte.Core.Math;


namespace Latte.Elements.Primitives;


public class ButtonElement : RectangleElement, IDefaultClickable
{
    public TextElement Text { get; protected set; }

    public MouseClickState ClickState { get; }
    public bool DisableTruePressOnlyWhenMouseIsUp { get; protected set; }
    
    public event EventHandler? MouseEnterEvent;
    public event EventHandler? MouseLeaveEvent;
    public event EventHandler? MouseDownEvent;
    public event EventHandler? MouseUpEvent;
    
    public Keyframe Normal { get; set; }
    public Keyframe Hover { get; set; }
    public Keyframe Down { get; set; }
    
    
    public ButtonElement(Element? parent, Vec2f position, Vec2f size, string text) : base(parent, position, size)
    {
        Text = new(this, new(), 32, text)
        {
            Alignment = { Value = AlignmentType.Center },
            Color = { Value = SFML.Graphics.Color.Black }
        };
        
        BorderColor.Set(new(100, 100, 100));
        BorderSize.Set(1f);

        ClickState = new();

        Normal = new();
        Hover = new();
        Down = new();
    }


    protected override void Setup()
    {
        SetDefaultKeyframeAnimation();
        
        base.Setup();
    }


    public override void Update()
    {
        (this as IDefaultClickable).UpdateClickStateProperties();
        (this as IDefaultClickable).ProcessMouseEvents();
        
        base.Update();
    }
    
    
    protected void SetDefaultKeyframeAnimation()
    {
        const byte colorDecreaseAmount = 25;
        const float radiusIncreaseAmount = 3.5f;
        
        Normal.SetIfNotDefined("Radius", Radius.Value);
        Hover.SetIfNotDefined("Radius", new Float(Radius.Value + radiusIncreaseAmount));
        Down.SetIfNotDefined("Radius", new Float(Radius.Value + radiusIncreaseAmount * 2f));
        
        ColorRGBA color = Color;
        
        Normal.SetIfNotDefined("Color", color);
        
        color.R -= colorDecreaseAmount;
        color.G -= colorDecreaseAmount;
        color.B -= colorDecreaseAmount;

        Hover.SetIfNotDefined("Color", color);
        
        color.R -= colorDecreaseAmount;
        color.G -= colorDecreaseAmount;
        color.B -= colorDecreaseAmount;
        
        Down.SetIfNotDefined("Color", color);
    }
    
    
    public virtual void OnMouseEnter()
    {
        Animator.Animate(Hover);
        MouseEnterEvent?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnMouseLeave()
    {
        Animator.Animate(Normal);
        MouseLeaveEvent?.Invoke(this, EventArgs.Empty);
    }
    
    public virtual void OnMouseDown()
    {
        Animator.Animate(Down);
        MouseDownEvent?.Invoke(this, EventArgs.Empty);
    }
    
    public virtual void OnMouseUp()
    {
        Animator.Animate(ClickState.IsMouseHover ? Hover : Normal);
        MouseUpEvent?.Invoke(this, EventArgs.Empty);
    }


    public virtual bool IsPointOver(Vec2f point)
        => IsPointOverClipArea(point) && IsPointOverThis(point);
    
    protected bool IsPointOverClipArea(Vec2f point)
        => Math.IsPointOverRect(point, GetFinalClipArea().ToWorldCoordinates());
    
    protected bool IsPointOverThis(Vec2f point)
        => Math.IsPointOverRoundedRect(point, AbsolutePosition, Size, Radius.Value);
}