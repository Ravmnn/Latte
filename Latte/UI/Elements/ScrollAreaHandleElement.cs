using System;

using Latte.Core.Type;
using Latte.Application;
using Latte.UI.Elements.Attributes;


using Math = System.Math;


namespace Latte.UI.Elements;


[IgnoreScroll]
public class ScrollAreaHandleElement : ButtonElement, IDraggable
{
    protected IDraggable ThisDraggable => this;

    public new ScrollAreaElement Parent => (base.Parent as ScrollAreaElement)!;

    public ScrollDirection Orientation { get; }

    public bool Dragging { get; set; }
    public bool WasDragging { get; set; }

    public event EventHandler? DragBeginEvent;
    public event EventHandler? DragEndEvent;
    public event EventHandler? DraggingEvent;


    public ScrollAreaHandleElement(ScrollAreaElement parent, ScrollDirection orientation) : base(parent, new Vec2f(), new Vec2f(), null)
    {
        Orientation = orientation;

        PrioritySnap = PrioritySnap.AlwaysOnParentTop;
        Radius = 2f;
        BorderSize = 0f;

        InitFromOrientation(Orientation);

        DisableFocus = true;
        DisableTruePressOnlyWhenMouseIsUp = true;
    }


    private void InitFromOrientation(ScrollDirection orientation)
    {
        switch (orientation)
        {
            case ScrollDirection.Vertical:
                Alignment = Alignment.Right;
                Size.X = 10;
                break;

            case ScrollDirection.Horizontal:
                Alignment = Alignment.Bottom;
                Size.Y = 10;
                break;
        }
    }


    public override void Update()
    {
        ThisDraggable.ProcessDraggingEvents();

        UpdateSize();

        ClampPosition();
        UpdateScrollAreaScrollOffset();

        WasDragging = Dragging;

        base.Update();
    }

    protected void UpdateSize()
    {
        var bounds = Parent.GetClampedChildrenBounds();
        var parentSize = Parent.Size;

        var sizeRatio = new Vec2f(parentSize.X / bounds.Width, parentSize.Y / bounds.Height);
        Vec2f size = parentSize * sizeRatio;

        if (Orientation == ScrollDirection.Vertical)
            Size.Y = size.Y;
        else
            Size.X = size.X;
    }

    protected void UpdateScrollAreaScrollOffset()
    {
        Vec2f scrollOffset = ((Vec2f)Parent.GetClampedChildrenBounds().Size - Parent.Size) * GetProgress();

        if (Orientation == ScrollDirection.Vertical)
            Parent.ScrollOffset.Y = scrollOffset.Y;
        else
            Parent.ScrollOffset.X = scrollOffset.X;
    }

    protected void ClampPosition()
    {
        if (Orientation == ScrollDirection.Vertical)
            RelativePosition.Y = Math.Clamp(RelativePosition.Y, 0, Parent.Size.Y - Size.Y);
        else
            RelativePosition.X = Math.Clamp(RelativePosition.X, 0, Parent.Size.X - Size.X);
    }


    public void ProcessDragging()
    {
        if (Orientation == ScrollDirection.Vertical)
            RelativePosition.Y += MouseInput.PositionDeltaInView.Y;
        else
            RelativePosition.X += MouseInput.PositionDeltaInView.X;
    }


    public void OnDragBegin() => DragBeginEvent?.Invoke(this, EventArgs.Empty);

    public void OnDragEnd() => DragEndEvent?.Invoke(this, EventArgs.Empty);

    public void OnDragging()
    {
        ProcessDragging();
        DraggingEvent?.Invoke(this, EventArgs.Empty);
    }


    public override void OnMouseDown()
    {
        base.OnMouseDown();
        Dragging = true;
    }

    public override void OnMouseUp()
    {
        base.OnMouseUp();
        Dragging = false;
    }


    public Vec2f GetProgress()
        => RelativePosition / (Parent.Size - Size);
}
