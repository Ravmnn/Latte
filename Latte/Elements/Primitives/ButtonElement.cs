using System;

using Latte.Core;
using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


public class ButtonElement : RectangleElement, IDefaultClickable
{
    public TextElement Text { get; protected set; }

    public MouseClickState MouseState { get; }
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
            Alignment = { Value = Alignments.Center },
            Color = { Value = SFML.Graphics.Color.Black }
        };
        
        BorderColor.Set(new(100, 100, 100));
        BorderSize.Set(1f);
        
        MouseState = new();

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
        (this as IDefaultClickable).UpdateMouseState();
        (this as IDefaultClickable).ProcessMouseEvents();
        
        base.Update();
    }
    
    
    private void SetDefaultKeyframeAnimation()
    {
        const byte ColorDecreaseAmount = 25;
        const float RadiusIncreaseAmount = 3.5f;
        
        Normal.SetIfNotDefined("Radius", Radius.Value);
        Hover.SetIfNotDefined("Radius", new Float(Radius.Value + RadiusIncreaseAmount));
        Down.SetIfNotDefined("Radius", new Float(Radius.Value + RadiusIncreaseAmount * 2f));
        
        ColorRGBA color = Color;
        
        Normal.SetIfNotDefined("Color", color);
        Hover.SetIfNotDefined("Color", color -= ColorDecreaseAmount);
        Down.SetIfNotDefined("Color", color - ColorDecreaseAmount);
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
        Animator.Animate(MouseState.IsMouseHover ? Hover : Normal);
        MouseUpEvent?.Invoke(this, EventArgs.Empty);
    }


    public virtual bool IsPointOver(Vec2f point)
        => IsPointOverClipArea(point) && IsPointOverThis(point);
    
    protected bool IsPointOverClipArea(Vec2f point)
        => point.IsPointOverRect(GetFinalClipArea().ToWorldCoordinates());
    
    protected bool IsPointOverThis(Vec2f point)
        => point.IsPointOverRoundedRect(AbsolutePosition, Size, Radius.Value);
}