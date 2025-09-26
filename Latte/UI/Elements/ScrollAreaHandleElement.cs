using System;

using Latte.Core.Type;
using Latte.UI.Elements.Attributes;
using Latte.Application;


using Math = System.Math;


namespace Latte.UI.Elements;


[IgnoreScroll]
public class ScrollAreaHandleElement : ButtonElement, IDraggable
{
    protected IDraggable ThisDraggable => this;

    public new ScrollAreaElement Parent => (base.Parent as ScrollAreaElement)!;

    public Orientation Orientation { get; }

    public bool Dragging { get; set; }
    public bool WasDragging { get; set; }

    public event EventHandler? DragBeginEvent;
    public event EventHandler? DragEndEvent;
    public event EventHandler? DraggingEvent;


    public ScrollAreaHandleElement(ScrollAreaElement parent, Orientation orientation) : base(parent, new Vec2f(), new Vec2f(), null)
    {
        Orientation = orientation;

        PrioritySnap = PrioritySnap.AlwaysOnParentTop;
        Radius = 2f;
        BorderSize = 0f;

        InitFromOrientation(Orientation);

        DisableFocus = true;
        DisableTruePressOnlyWhenMouseIsUp = true;
    }


    private void InitFromOrientation(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.Vertical:
                Alignment = Alignment.Right;
                Size.X = 10;
                break;

            case Orientation.Horizontal:
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
        var size = parentSize * sizeRatio;

        if (Orientation == Orientation.Vertical)
            Size.Y = size.Y;
        else
            Size.X = size.X;
    }

    protected void UpdateScrollAreaScrollOffset()
    {
        Vec2f scrollOffset = ((Vec2f)Parent.GetClampedChildrenBounds().Size - Parent.Size) * GetProgress();

        if (Orientation == Orientation.Vertical)
            Parent.ScrollOffset.Y = scrollOffset.Y;
        else
            Parent.ScrollOffset.X = scrollOffset.X;
    }

    protected void ClampPosition()
    {
        if (Orientation == Orientation.Vertical)
            RelativePosition.Y = Math.Clamp(RelativePosition.Y, 0, Parent.Size.Y - Size.Y);
        else
            RelativePosition.X = Math.Clamp(RelativePosition.X, 0, Parent.Size.X - Size.X);
    }


    public void ProcessDragging()
        => RelativePosition += MouseInput.PositionDeltaInView;


    public void OnDragBegin() => DragBeginEvent?.Invoke(this, EventArgs.Empty);

    public void OnDragEnd() => DragEndEvent?.Invoke(this, EventArgs.Empty);

    public void OnDragging()
    {
        ProcessDragging();
        DraggingEvent?.Invoke(this, EventArgs.Empty);
    }


    public override void OnMouseDown()
    {
        Dragging = true;
        base.OnMouseDown();
    }

    public override void OnMouseUp()
    {
        Dragging = false;
        base.OnMouseUp();
    }


    public Vec2f GetProgress()
        => RelativePosition / (Parent.Size - Size);
}
