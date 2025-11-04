using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Window;

using Latte.Core;
using Latte.Core.Type;
using Latte.Application;


namespace Latte.UI.Elements;




internal class RadialButtonSelectedIndicatorElement : CircleElement
{
    public new RadialButton Parent => (base.Parent as RadialButton)!;




    public RadialButtonSelectedIndicatorElement(RadialButton parent) : base(parent, null, 0)
    {
        Color = SFML.Graphics.Color.Black;
    }


    public override void UnconditionalUpdate()
    {
        Visible = Parent.Selected;
        Radius = Parent.Radius - 3;

        base.UnconditionalUpdate();
    }
}




public class RadialButton : CircleElement, IClickable, INavigationTarget
{
    public bool Focused { get; set; }
    public bool DisableFocus { get; set; }

    public event EventHandler? FocusEvent;
    public event EventHandler? UnfocusEvent;


    protected IClickable ThisClickable => this;

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


    public bool IgnoreKeyboardInput { get; set; }

    public event EventHandler<KeyEventArgs>? KeyDownEvent;
    public event EventHandler<KeyEventArgs>? KeyUpEvent;
    public event EventHandler<TextEventArgs>? TextEnteredEvent;
    public event EventHandler<KeyEventArgs>? SubmitKeyDownEvent;
    public event EventHandler<KeyEventArgs>? SubmitKeyUpEvent;


    public int NavigationPriority { get; set; }




    protected Element SelectedIndicator { get; set; }


    public List<RadialButton> Chain { get; set; }


    private bool _selected;
    public bool Selected
    {
        get => _selected;
        set
        {
            _selected = value;
            OnSelectedChanged();
        }
    }


    public event EventHandler? SelectedChangedEvent;




    public RadialButton(Element? parent, Vec2f? position, float radius, params IEnumerable<RadialButton> chain)
        : base(parent, position, radius)
    {
        UnfocusOnMouseDownOutside = true;

        MouseState = new MouseClickState();


        BorderSize = 1f;
        SelectedIndicator = new RadialButtonSelectedIndicatorElement(this);

        BorderColor = SFML.Graphics.Color.Black;


        Chain = chain.ToList();
    }




    public override void Update()
    {
        ThisClickable.UpdateMouseState();
        ThisClickable.ProcessMouseEvents();

        base.Update();
    }




    public virtual void OnFocus()
    {
        FocusManager.FocusOn(this);
        FocusEvent?.Invoke(this, EventArgs.Empty);
    }


    public virtual void OnUnfocus()
        => UnfocusEvent?.Invoke(this, EventArgs.Empty);




    public virtual bool IsPointOver(Vec2f point)
        => point.IsPointOverElementClipArea(this) && point.IsPointOverCircle(AbsolutePosition + new Vec2f(Radius, Radius), Radius);


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
    {
        Selected = !Selected;
        MouseClickEvent?.Invoke(this, EventArgs.Empty);
    }




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




    protected virtual void OnSelectedChanged()
    {
        if (Selected)
            foreach (var chainButton in Chain)
                if (chainButton != this && chainButton.Selected)
                    chainButton.Selected = false;

        SelectedChangedEvent?.Invoke(this, EventArgs.Empty);
    }
}
