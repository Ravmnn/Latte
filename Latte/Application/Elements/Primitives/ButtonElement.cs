using System;

using SFML.Window;

using Latte.Core;
using Latte.Core.Type;
using Latte.Application.Elements.Behavior;
using Latte.Application.Elements.Primitives.Shapes;


namespace Latte.Application.Elements.Primitives;


public class ButtonElement : RectangleElement, IClickable, INavigationTarget
{
    protected IClickable ThisClickable => this;
    protected IFocusable ThisFocusable => this;

    public TextElement? Text { get; set; }

    public bool Focused { get; set; }
    public bool DisableFocus { get; set; }

    public event EventHandler? FocusEvent;
    public event EventHandler? UnfocusEvent;

    public bool FocusOnMouseDown { get; set; }
    public bool UnfocusOnMouseDownOutside { get; set; }

    public MouseClickState MouseState { get; }
    public bool DisableTruePressOnlyWhenMouseIsUp { get; protected set; }

    public event EventHandler? MouseEnterEvent;
    public event EventHandler? MouseLeaveEvent;
    public event EventHandler? MouseDownEvent;
    public event EventHandler? MouseUpEvent;

    public event EventHandler? MouseHoverEvent;

    public event EventHandler? MouseClickEvent;

    public int NavigationPriority { get; set; }

    public event EventHandler<KeyEventArgs>? KeyDownEvent;
    public event EventHandler<KeyEventArgs>? KeyUpEvent;
    public event EventHandler<TextEventArgs>? TextEnteredEvent;
    public event EventHandler<KeyEventArgs>? SubmitKeyDownEvent;
    public event EventHandler<KeyEventArgs>? SubmitKeyUpEvent;

    public bool IgnoreKeyboardInput { get; set; }


    public ButtonElement(Element? parent, Vec2f? position, Vec2f size, string? text) : base(parent, position, size)
    {
        if (text is not null)
            Text = new TextElement(this, new Vec2f(), null, text)
            {
                IgnoreMouseInput = true,

                Alignment = { Value = Behavior.Alignment.Center },

                Color = { Value = SFML.Graphics.Color.Black }
            };

        UnfocusOnMouseDownOutside = true;

        BorderColor.Set(new ColorRGBA(100, 100, 100));
        BorderSize.Set(1f);

        MouseState = new MouseClickState();
    }


    public override void Update()
    {
        ThisClickable.UpdateMouseState();
        ThisClickable.ProcessMouseEvents();

        base.Update();
    }


    public virtual bool IsPointOver(Vec2f point)
        => IsPointOverClipArea(point) && IsPointOverThis(point);

    protected bool IsPointOverThis(Vec2f point)
        => point.IsPointOverRoundedRect(AbsolutePosition, Size, Radius.Value);


    public virtual void OnMouseEnter()
        => MouseEnterEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnMouseLeave()
        => MouseLeaveEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnMouseDown()
        => MouseDownEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnMouseUp()
        => MouseUpEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnMouseHover()
        => MouseHoverEvent?.Invoke(this, EventArgs.Empty);


    public virtual void OnMouseClick()
        => MouseClickEvent?.Invoke(this, EventArgs.Empty);


    public virtual void OnFocus()
    {
        FocusManager.FocusOn(this);
        FocusEvent?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnUnfocus()
        => UnfocusEvent?.Invoke(this, EventArgs.Empty);


    public virtual void OnKeyDown(KeyEventArgs key)
    {
        if (key.Scancode == Keyboard.Scancode.Enter)
            OnSubmitKeyDown(key);

        KeyDownEvent?.Invoke(this, key);
    }

    public virtual void OnKeyUp(KeyEventArgs key)
    {
        if (key.Scancode == Keyboard.Scancode.Enter)
            OnSubmitKeyUp(key);

        KeyUpEvent?.Invoke(this, key);
    }


    public virtual void OnTextEntered(TextEventArgs text)
        => TextEnteredEvent?.Invoke(this, text);


    public virtual void OnSubmitKeyDown(KeyEventArgs key)
        => SubmitKeyDownEvent?.Invoke(this, key);

    public virtual void OnSubmitKeyUp(KeyEventArgs key)
    {
        OnMouseClick();
        SubmitKeyUpEvent?.Invoke(this, key);
    }
}
