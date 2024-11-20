using System;

using Latte.Core;
using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


using Math = Latte.Core.Math;


namespace Latte.Elements.Primitives;


public class ButtonElement : RectangleElement, IDefaultClickable
{
    public TextElement Text { get; protected set; }

    public MouseClickState MouseClickState { get; }
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
            Text = { FillColor = SFML.Graphics.Color.Black }
        };
        
        BorderColor.Set(new(100, 100, 100));
        BorderSize.Set(1f);

        MouseClickState = new();

        Normal = new();
        Hover = new();
        Down = new();
        
        KeyframesFromColor(Color);
    }


    public override void Update()
    {
        (this as IDefaultClickable).UpdateClickStateProperties();
        (this as IDefaultClickable).ProcessMouseEvents();
        
        base.Update();
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
        Animator.Animate(MouseClickState.IsMouseHover ? Hover : Normal);
        MouseUpEvent?.Invoke(this, EventArgs.Empty);
    }

    
    public virtual bool IsPointOver(Vec2f point)
        => Math.IsPointOverRoundedRect(point, AbsolutePosition, Size, Radius.Value);
    
    // BUG: clicking is working even if the button is clipped


    public void KeyframesFromColor(ColorRGBA color)
    {
        const byte decreaseAmount = 30;
        
        Normal["Color"] = color;
        
        color.R -= decreaseAmount;
        color.G -= decreaseAmount;
        color.B -= decreaseAmount;

        Hover["Color"] = color;
        
        color.R -= decreaseAmount;
        color.G -= decreaseAmount;
        color.B -= decreaseAmount;
        
        Down["Color"] = color;
    }
}