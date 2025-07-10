using System;

using SFML.Window;

using Latte.Core;
using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements.Behavior;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


public class ButtonElement : RectangleElement, IDefaultClickable, INavigationTarget
{
    private bool _focused;


    public TextElement? Text { get; set; }

    public MouseClickState MouseState { get; }
    public bool DisableTruePressOnlyWhenMouseIsUp { get; protected set; }

    public event EventHandler? MouseEnterEvent;
    public event EventHandler? MouseLeaveEvent;
    public event EventHandler? MouseDownEvent;
    public event EventHandler? MouseUpEvent;

    public event EventHandler? MouseClickEvent;

    public int NavigationPriority { get; set; }

    public event EventHandler<KeyEventArgs>? KeyboardInputReceivedEvent;

    public bool IgnoreKeyboardInput { get; set; }
    public bool CaughtKeyboardInput { get; set; }

    public event EventHandler? FocusEvent;
    public event EventHandler? UnfocusEvent;

    public bool Focused
    {
        get => _focused;
        set
        {
            if (!CanFocus)
            {
                _focused = false;
                return;
            }

            var oldFocused = _focused;
            _focused = value;

            if (value == oldFocused)
                return;

            if (value)
                OnFocus();
            else
                OnUnfocus();
        }
    }

    public bool CanFocus => Visible && !DisableFocus;
    public bool DisableFocus { get; set; }

    public bool FocusOnClick { get; set; }

    public ButtonElement(Element? parent, Vec2f position, Vec2f size, string? text) : base(parent, position, size)
    {
        if (text is not null)
            Text = new TextElement(this, new Vec2f(), null, text)
            {
                Alignment = { Value = Behavior.Alignment.Center },

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
    {
        if (FocusOnClick)
            Focused = true;

        MouseClickEvent?.Invoke(this, EventArgs.Empty);
    }


    public virtual bool IsPointOver(Vec2f point)
        => IsPointOverClipArea(point) && IsPointOverThis(point);

    protected bool IsPointOverThis(Vec2f point)
        => point.IsPointOverRoundedRect(AbsolutePosition, Size, Radius.Value);


    public void OnFocus()
    {
        FocusManager.FocusOn(this);
        FocusEvent?.Invoke(this, EventArgs.Empty);
    }

    public void OnUnfocus()
        => UnfocusEvent?.Invoke(this, EventArgs.Empty);


    public void OnKeyboardInputReceived(KeyEventArgs key)
    {
        if (key.Scancode == Keyboard.Scancode.Enter)
            OnMouseClick();

        KeyboardInputReceivedEvent?.Invoke(this, key);
    }
}
