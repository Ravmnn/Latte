using System;

using Latte.Core;
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

    public ButtonElement(Element? parent, Vec2f position, Vec2f size, string? text) : base(parent, position, size)
    {
        if (text is not null)
            Text = new TextElement(this, new Vec2f(), null, text)
            {
                Alignment = { Value = Elements.Alignment.Center },
                SizePolicy = { Value = SizePolicyType.FitParent },
                SizePolicyMargin = { Value = new Vec2f(3f, 3f) },

                Color = { Value = SFML.Graphics.Color.Black }
            };

        BorderColor.Set(new ColorRGBA(100, 100, 100));
        BorderSize.Set(1f);

        MouseState = new MouseClickState();
    }


    public override void Update()
    {
        (this as IDefaultClickable).UpdateMouseState();
        (this as IDefaultClickable).ProcessMouseEvents();

        base.Update();
    }


    public virtual void OnMouseEnter()
        => MouseEnterEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnMouseLeave()
        => MouseLeaveEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnMouseDown()
        => MouseDownEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnMouseUp()
        => MouseUpEvent?.Invoke(this, EventArgs.Empty);


    public virtual void OnMouseClick()
        => MouseClickEvent?.Invoke(this, EventArgs.Empty);


    public virtual bool IsPointOver(Vec2f point)
        => IsPointOverClipArea(point) && IsPointOverThis(point);

    protected bool IsPointOverThis(Vec2f point)
        => point.IsPointOverRoundedRect(AbsolutePosition, Size, Radius.Value);
}
