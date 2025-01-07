using System;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements;


[Flags]
public enum WindowElementStyles
{
    None = 0,

    Closeable = 1 << 0,
    Resizable = 1 << 1,
    Moveable = 1 << 2,

    Default = Closeable | Resizable | Moveable
}


public class WindowElement : RectangleElement, IDefaultDraggable, IDefaultResizable
{
    public TextElement Title { get; protected set; }

    public ButtonElement CloseButton { get; protected set; }

    public WindowElementStyles Styles { get; set; }

    public event EventHandler? OpenedEvent;
    public event EventHandler? ClosedEvent;

    public bool Dragging { get; protected set; }
    public bool WasDragging { get; protected set; }

    public bool Resizing { get; protected set; }
    public bool WasResizing { get; protected set; }

    public MouseClickState MouseState { get; }
    public bool DisableTruePressOnlyWhenMouseIsUp { get; protected set; }

    public Corner CornerToResize { get; set; }
    public FloatRect Rect => new(RelativePosition.Value, Size.Value);
    public float CornerResizeAreaSize { get; protected set; }

    public Vec2f? MinSize { get; set; }
    public Vec2f? MaxSize { get; set; }

    public event EventHandler? MouseEnterEvent;
    public event EventHandler? MouseLeaveEvent;
    public event EventHandler? MouseDownEvent;
    public event EventHandler? MouseUpEvent;

    public event EventHandler? DragBeginEvent;
    public event EventHandler? DragEndEvent;
    public event EventHandler? DraggingEvent;

    public event EventHandler? ResizeBeginEvent;
    public event EventHandler? ResizeEndEvent;
    public event EventHandler? ResizingEvent;


    public WindowElement(string title, Vec2f position, Vec2f size, WindowElementStyles styles = WindowElementStyles.Default)
        : base(null, position, size)
    {
        Title = new(this, new(), 20, title);
        Title.Alignment.Set(Alignments.HorizontalCenter | Alignments.Top);
        Title.AlignmentMargin.Set(new(0, 10));

        Color.Set(new(50, 50, 50, 220));

        CloseButton = new(this, new(), new(15, 15), "")
        {
            Color = { Value = new(255, 100, 100) },

            Alignment = { Value = Alignments.TopRight },
            AlignmentMargin = { Value = new(-7, 8) },

            Down =
            {
                { "Scale", new Vec2f(0.95f, 0.95f) }
            }
        };
        CloseButton.MouseUpEvent += (_, _) => Close();

        Styles = styles;

        MouseState = new();
        DisableTruePressOnlyWhenMouseIsUp = true;

        CornerToResize = new();
        CornerResizeAreaSize = 10f;

        MinSize = new(50, 50);
    }


    public override void Update()
    {
        (this as IDefaultClickable).UpdateMouseState();
        (this as IDefaultClickable).ProcessMouseEvents();

        (this as IDefaultResizable).UpdateCornersToResize();

        if (Styles.HasFlag(WindowElementStyles.Resizable))
            (this as IDefaultResizable).ProcessResizingEvents();

        if (Styles.HasFlag(WindowElementStyles.Moveable))
            (this as IDefaultDraggable).ProcessDraggingEvents();

        App.Window.Cursor = Window.GetCursorTypeFromCorners(CornerToResize);

        CloseButton.Visible = Styles.HasFlag(WindowElementStyles.Closeable);

        WasDragging = Dragging;
        WasResizing = Resizing;

        base.Update();
    }


    public void ProcessDragging()
    {
        RelativePosition.Value += App.ElementViewMousePositionDelta;
    }


    public void ProcessResizing()
    {
        Vec2f delta = App.ElementViewMousePositionDelta;

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
            ResizeCornersToSizeLimit(MinSize! - Size.Value, left != 0f, top != 0f, right != 0f, bottom != 0f);

        if (ShouldResizeCornersToMaxSize())
            ResizeCornersToSizeLimit(MaxSize! - Size.Value, left != 0f, top != 0f, right != 0f, bottom != 0f);
    }

    private void ResizeCornersBy(float left = 0f, float top = 0f, float right = 0f, float bottom = 0f)
    {
        RelativePosition.Value.X += left;
        RelativePosition.Value.Y += top;
        Size.Value.X += right;
        Size.Value.Y += bottom;
    }

    private void ResizeCornersToSizeLimit(Vec2f value, bool left = false, bool top = false, bool right = false, bool bottom = false)
        => ResizeCornersBy(left ? -value.X : 0f, top ? -value.Y : 0f, right ? value.X : 0f, bottom ? value.Y : 0f);

    private bool ShouldResizeCornersToMinSize()
        => MinSize is not null && (Size.Value.X < MinSize.X || Size.Value.Y < MinSize.Y);

    private bool ShouldResizeCornersToMaxSize()
        => MaxSize is not null && (Size.Value.X > MaxSize.X || Size.Value.Y > MaxSize.Y);


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
        => point.IsPointOverRoundedRect(AbsolutePosition, Size, Radius.Value);
}
