using System;

using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements.Attributes;
using Latte.Elements.Behavior;


namespace Latte.Elements.Primitives;


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
        Radius.Set(2f);
        BorderSize.Set(0f);

        InitFromOrientation(Orientation);

        DisableTruePressOnlyWhenMouseIsUp = true;
    }


    private void InitFromOrientation(ScrollDirection orientation)
    {
        switch (orientation)
        {
            case ScrollDirection.Vertical:
                Alignment.Set(Behavior.Alignment.Right);
                Size.Value.X = 10;
                break;

            case ScrollDirection.Horizontal:
                Alignment.Set(Behavior.Alignment.Bottom);
                Size.Value.Y = 10;
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
        var parentSize = Parent.Size.Value;

        var sizeRatio = new Vec2f(parentSize.X / bounds.Width, parentSize.Y / bounds.Height);
        Vec2f size = parentSize * sizeRatio;

        if (Orientation == ScrollDirection.Vertical)
            Size.Value.Y = size.Y;
        else
            Size.Value.X = size.X;
    }

    protected void UpdateScrollAreaScrollOffset()
    {
        Vec2f scrollOffset = ((Vec2f)Parent.GetClampedChildrenBounds().Size - Parent.Size.Value) * GetProgress();

        if (Orientation == ScrollDirection.Vertical)
            Parent.ScrollOffset.Y = scrollOffset.Y;
        else
            Parent.ScrollOffset.X = scrollOffset.X;
    }

    protected void ClampPosition()
    {
        if (Orientation == ScrollDirection.Vertical)
            RelativePosition.Value.Y = Math.Clamp(RelativePosition.Value.Y, 0, Parent.Size.Value.Y - Size.Value.Y);
        else
            RelativePosition.Value.X = Math.Clamp(RelativePosition.Value.X, 0, Parent.Size.Value.X - Size.Value.X);
    }


    public void ProcessDragging()
    {
        if (Orientation == ScrollDirection.Vertical)
            RelativePosition.Value.Y += MouseInput.PositionDeltaInElementView.Y;
        else
            RelativePosition.Value.X += MouseInput.PositionDeltaInElementView.X;
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


    public Vec2f GetProgress() => RelativePosition.Value / (Parent.Size.Value - Size.Value);
}
