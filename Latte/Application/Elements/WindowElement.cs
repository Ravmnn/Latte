using System;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Application.Elements.Behavior;
using Latte.Application.Elements.Primitives;
using Latte.Application.Elements.Primitives.Shapes;


namespace Latte.Application.Elements;


[Flags]
public enum WindowElementStyles
{
    None = 0,

    Closeable = 1 << 0,
    Resizable = 1 << 1,
    Moveable = 1 << 2,

    Default = Closeable | Resizable | Moveable
}


public class WindowCloseButtonElement : ButtonElement
{
    public new WindowElement Parent => (base.Parent as WindowElement)!;


    public WindowCloseButtonElement(WindowElement parent) : base(parent, new Vec2f(), new Vec2f(15, 15), null)
    {
        Color = new ColorRGBA(255, 100, 100);

        Alignment = Alignment.TopRight;
        AlignmentMargin = new Vec2f(-7, 8);
    }


    public override void OnMouseClick()
    {
        Parent.Close();
        base.OnMouseClick();
    }
}


public class WindowElement : RectangleElement, IDraggable, IResizable
{
    protected IFocusable ThisFocusable => this;
    protected IClickable ThisClickable => this;
    protected IDraggable ThisDraggable => this;
    protected IResizable ThisResizable => this;

    public TextElement Title { get; protected set; }

    public WindowCloseButtonElement CloseButton { get; protected set; }

    public WindowElementStyles Styles { get; set; }

    public bool Dragging { get; protected set; }
    public bool WasDragging { get; protected set; }

    public bool Resizing { get; protected set; }
    public bool WasResizing { get; protected set; }

    public bool Focused { get; set; }
    public bool DisableFocus { get; set; }

    public event EventHandler? FocusEvent;
    public event EventHandler? UnfocusEvent;

    public bool FocusOnMouseDown { get; set; }
    public bool UnfocusOnMouseDownOutside { get; set; }

    public MouseClickState MouseState { get; }
    public bool DisableTruePressOnlyWhenMouseIsUp { get; protected set; }

    public Corner CornerToResize { get; set; }
    public FloatRect Rect => new FloatRect(RelativePosition, Size);
    public float CornerResizeAreaSize { get; protected set; }

    public Vec2f? MinSize { get; set; }
    public Vec2f? MaxSize { get; set; }

    public bool IsCloseable => Styles.HasFlag(WindowElementStyles.Closeable);
    public bool IsResizable => Styles.HasFlag(WindowElementStyles.Resizable);
    public bool IsMoveable => Styles.HasFlag(WindowElementStyles.Moveable);

    public event EventHandler? OpenedEvent;
    public event EventHandler? ClosedEvent;

    public event EventHandler? MouseEnterEvent;
    public event EventHandler? MouseLeaveEvent;
    public event EventHandler? MouseDownEvent;
    public event EventHandler? MouseUpEvent;

    public event EventHandler? MouseHoverEvent;

    public event EventHandler? MouseClickEvent;

    public event EventHandler? DragBeginEvent;
    public event EventHandler? DragEndEvent;
    public event EventHandler? DraggingEvent;

    public event EventHandler? ResizeBeginEvent;
    public event EventHandler? ResizeEndEvent;
    public event EventHandler? ResizingEvent;


    public WindowElement(string title, Vec2f position, Vec2f size, WindowElementStyles styles = WindowElementStyles.Default)
        : base(null, position, size)
    {
        Title = new TextElement(this, new Vec2f(), 20, title)
        {
            IgnoreMouseInput = true,

            Color = SFML.Graphics.Color.Black,

            Alignment = Alignment.HorizontalCenter | Alignment.Top,
            AlignmentMargin = new Vec2f(0, 10)
        };

        CloseButton = new WindowCloseButtonElement(this);

        Styles = styles;

        DisableFocus = true;

        MouseState = new MouseClickState();
        DisableTruePressOnlyWhenMouseIsUp = true;

        CornerToResize = new Corner();
        CornerResizeAreaSize = 10f;

        MinSize = new Vec2f(50, 50);
    }


    public override void Update()
    {
        ThisClickable.UpdateMouseState();
        ThisClickable.ProcessMouseEvents();

        if (IsResizable)
        {
            ThisResizable.UpdateCornersToResize();
            ThisResizable.ProcessResizingEvents();
        }

        if (IsMoveable)
            ThisDraggable.ProcessDraggingEvents();

        CloseButton.Visible = IsCloseable;

        WasDragging = Dragging;
        WasResizing = Resizing;

        base.Update();
    }


    public void ProcessDragging()
    {
        RelativePosition += MouseInput.PositionDeltaInElementView;
    }


    public void ProcessResizing()
    {
        var delta = MouseInput.PositionDeltaInElementView;

        if (CornerToResize.HasFlag(Corner.Top))
            ResizeCorners(top: delta.Y, bottom: -delta.Y);

        else if (CornerToResize.HasFlag(Corner.Bottom))
            ResizeCorners(bottom: delta.Y);

        if (CornerToResize.HasFlag(Corner.Left))
            ResizeCorners(left: delta.X, right: -delta.X);

        else if (CornerToResize.HasFlag(Corner.Right))
            ResizeCorners(right: delta.X);
    }

    private void ResizeCorners(float left = 0f, float top = 0f, float right = 0f, float bottom = 0f)
    {
        ResizeCornersBy(left, top, right, bottom);

        if (ShouldResizeCornersToMinSize())
            ResizeCornersToSizeLimit(MinSize! - Size, left != 0f, top != 0f, right != 0f, bottom != 0f);

        if (ShouldResizeCornersToMaxSize())
            ResizeCornersToSizeLimit(MaxSize! - Size, left != 0f, top != 0f, right != 0f, bottom != 0f);
    }

    private void ResizeCornersBy(float left = 0f, float top = 0f, float right = 0f, float bottom = 0f)
    {
        RelativePosition.X += left;
        RelativePosition.Y += top;
        Size.X += right;
        Size.Y += bottom;
    }

    private void ResizeCornersToSizeLimit(Vec2f value, bool left = false, bool top = false, bool right = false, bool bottom = false)
        => ResizeCornersBy(left ? -value.X : 0f, top ? -value.Y : 0f, right ? value.X : 0f, bottom ? value.Y : 0f);

    private bool ShouldResizeCornersToMinSize()
        => MinSize is not null && (Size.X < MinSize.X || Size.Y < MinSize.Y);

    private bool ShouldResizeCornersToMaxSize()
        => MaxSize is not null && (Size.X > MaxSize.X || Size.Y > MaxSize.Y);


    public void Open() => OnOpen();
    public void Close() => OnClose();


    protected virtual void OnOpen()
    {
        if (Visible)
            return;

        Show();
        OpenedEvent?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnClose()
    {
        if (!Visible)
            return;

        Hide();
        ClosedEvent?.Invoke(this, EventArgs.Empty);
    }


    public void OnFocus()
        => FocusEvent?.Invoke(this, EventArgs.Empty);

    public void OnUnfocus()
        => UnfocusEvent?.Invoke(this, EventArgs.Empty);


    public virtual void OnMouseEnter()
        => MouseEnterEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnMouseLeave()
        => MouseLeaveEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnMouseDown()
    {
        Resizing = CornerToResize != Corner.None;
        Dragging = !Resizing; // don't drag while resizing

        MouseDownEvent?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnMouseUp()
    {
        Dragging = false;
        Resizing = false;

        MouseUpEvent?.Invoke(this, EventArgs.Empty);
    }


    public virtual void OnMouseHover()
    {
        if (IsResizable && Window.GetCursorTypeFromCorners(CornerToResize) is { } cursorType)
            App.Window.Cursor.Type = cursorType;

        MouseHoverEvent?.Invoke(this, EventArgs.Empty);
    }


    public virtual void OnMouseClick()
        => MouseClickEvent?.Invoke(this, EventArgs.Empty);


    public virtual void OnDragBegin() => DragBeginEvent?.Invoke(this, EventArgs.Empty);
    public virtual void OnDragEnd() => DragEndEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnDragging()
    {
        ProcessDragging();
        DraggingEvent?.Invoke(this, EventArgs.Empty);
    }


    public virtual void OnResizeBegin() => ResizeBeginEvent?.Invoke(this, EventArgs.Empty);
    public virtual void OnResizeEnd() => ResizeEndEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnResizing()
    {
        ProcessResizing();
        ResizingEvent?.Invoke(this, EventArgs.Empty);
    }


    public virtual bool IsPointOver(Vec2f point)
        => IsPointOverClipArea(point) && IsPointOverThis(point);

    protected bool IsPointOverThis(Vec2f point)
        => point.IsPointOverRoundedRect(AbsolutePosition, Size, Radius);
}
