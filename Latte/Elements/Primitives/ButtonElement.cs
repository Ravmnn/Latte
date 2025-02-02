using System;

using Latte.Core;
using Latte.Core.Animation;
using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


public class ButtonElement : RectangleElement, IDefaultClickable
{
    public TextElement? Text { get; protected set; }

    public MouseClickState MouseState { get; }
    public bool DisableTruePressOnlyWhenMouseIsUp { get; protected set; }

    public event EventHandler? MouseEnterEvent;
    public event EventHandler? MouseLeaveEvent;
    public event EventHandler? MouseDownEvent;
    public event EventHandler? MouseUpEvent;

    public event EventHandler? MouseClickEvent;

    public Keyframe Hover { get; }
    public Keyframe Down { get; }

    public bool UseDefaultAnimation { get; set; }

    // TODO: improve default animation setting

    public ButtonElement(Element? parent, Vec2f position, Vec2f size, string? text) : base(parent, position, size)
    {
        if (text is not null)
            Text = new(this, new(), null, text)
            {
                Alignment = { Value = Elements.Alignment.Center },

                Color = { Value = SFML.Graphics.Color.Black }
            };

        BorderColor.Set(new(100, 100, 100));
        BorderSize.Set(1f);

        MouseState = new();

        Hover = new();
        Down = new();

        UseDefaultAnimation = true;

        Color.ValueChangedEvent += (_, _) =>
        {
            if (UseDefaultAnimation)
                SetDefaultKeyframeAnimation();
        };
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

        if (!Normal.TryGetValue("Color", out IAnimatable? value) || value is not IAnimatable<ColorRGBA> color)
            return;

        Hover["Color"] = color.Get() - ColorDecreaseAmount;
        Down["Color"] = color.Get() - ColorDecreaseAmount * 2;
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


    public virtual void OnMouseClick()
        => MouseClickEvent?.Invoke(this, EventArgs.Empty);


    public virtual bool IsPointOver(Vec2f point)
        => IsPointOverClipArea(point) && IsPointOverThis(point);

    protected bool IsPointOverThis(Vec2f point)
        => point.IsPointOverRoundedRect(AbsolutePosition, Size, Radius.Value);
}
