using System;
using System.Linq;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Core.Type;


using Math = System.Math;


namespace Latte.Elements.Primitives;


[AttributeUsage(AttributeTargets.Class)]
public class IgnoreScrollAttribute : Attribute;


public enum ScrollDirection
{
    Vertical,
    Horizontal
}


[IgnoreScroll]
public class ScrollAreaHandleElement : ButtonElement, IDefaultDraggable
{
    public new ScrollAreaElement Parent => (base.Parent as ScrollAreaElement)!;

    public ScrollDirection Orientation { get; }

    public bool Dragging { get; set; }
    public bool WasDragging { get; set; }

    public event EventHandler? DragBeginEvent;
    public event EventHandler? DragEndEvent;
    public event EventHandler? DraggingEvent;


    public ScrollAreaHandleElement(ScrollAreaElement parent, ScrollDirection orientation) : base(parent, new(), new(), null)
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
                Alignment.Set(Elements.Alignment.Right);
                Size.Value.X = 10;
                break;

            case ScrollDirection.Horizontal:
                Alignment.Set(Elements.Alignment.Bottom);
                Size.Value.Y = 10;
                break;
        }
    }


    public override void Update()
    {
        (this as IDefaultDraggable).ProcessDraggingEvents();

        UpdateSize();

        ClampPosition();
        UpdateScrollAreaScrollOffset();

        WasDragging = Dragging;

        base.Update();
    }

    protected void UpdateSize()
    {
        FloatRect bounds = Parent.GetChildrenBounds();
        Vec2f parentSize = Parent.Size.Value;

        Vec2f sizeRatio = new(parentSize.X / bounds.Width, parentSize.Y / bounds.Height);
        Vec2f size = parentSize * sizeRatio;

        if (Orientation == ScrollDirection.Vertical)
            Size.Value.Y = size.Y;
        else
            Size.Value.X = size.X;
    }

    protected void UpdateScrollAreaScrollOffset()
    {
        Vec2f scrollOffset = ((Vec2f)Parent.GetChildrenBounds().Size - Parent.Size.Value) * GetProgress();

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
            RelativePosition.Value.Y += App.ElementViewMousePositionDelta.Y;
        else
            RelativePosition.Value.X += App.ElementViewMousePositionDelta.X;
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


public class ScrollAreaElement : ButtonElement
{
    public ScrollAreaHandleElement? VerticalScrollHandle { get; set; }
    public ScrollAreaHandleElement? HorizontalScrollHandle { get; set; }
    private bool IsAnyHandlePressed => (VerticalScrollHandle?.MouseState.IsTruePressed ?? false) ||
                                       (HorizontalScrollHandle?.MouseState.IsTruePressed ?? false);

    public Vec2f ScrollOffset { get; set; }
    protected Vec2f LastScrollOffset { get; set; }
    public Vec2f ScrollOffsetDelta => ScrollOffset - LastScrollOffset;
    public float ScrollOffsetStep { get; set; }

    public ScrollDirection Direction { get; set; }

    public event EventHandler<Vec2f>? ScrollEvent;


    public ScrollAreaElement(Element? parent, Vec2f position, Vec2f size, bool verticalScrollHandle = true, bool horizontalScrollHandle = false)
            : base(parent, position, size, null)
    {
        if (verticalScrollHandle)
            VerticalScrollHandle = new(this, ScrollDirection.Vertical);

        if (horizontalScrollHandle)
            HorizontalScrollHandle = new(this, ScrollDirection.Horizontal);

        ScrollOffset = new();
        LastScrollOffset = new();

        ScrollOffsetStep = 10f;
        Direction = ScrollDirection.Vertical;

        UseDefaultAnimation = false;
    }


    public override void Update()
    {
        UpdateScrollHandlesVisibility();

        if (!IsAnyHandlePressed)
            AddAppMouseScrollDeltaToScrollOffset();

        ClampScrollOffset();
        ScrollBasedOnScrollDelta();

        LastScrollOffset = ScrollOffset.Copy();

        base.Update();
    }

    private void UpdateScrollHandlesVisibility()
    {
        FloatRect childrenBounds = GetChildrenBounds();

        if (VerticalScrollHandle is not null)
            VerticalScrollHandle.Visible = childrenBounds.Size.Y > Size.Value.Y;

        if (HorizontalScrollHandle is not null)
            HorizontalScrollHandle.Visible = childrenBounds.Size.X > Size.Value.X;
    }

    protected void AddAppMouseScrollDeltaToScrollOffset()
    {
        float step = ScrollOffsetStep * -App.MouseScrollDelta;

        switch (Direction)
        {
            case ScrollDirection.Vertical:
                ScrollOffset.Y += step;
                break;

            case ScrollDirection.Horizontal:
                ScrollOffset.X += step;
                break;
        }

        SyncScrollHandlesPositionToScrollOffset();
    }

    protected void ScrollBasedOnScrollDelta()
    {
        if (ScrollOffsetDelta.X != 0)
            ScrollHorizontally(-ScrollOffsetDelta.X);

        if (ScrollOffsetDelta.Y != 0)
            ScrollVertically(-ScrollOffsetDelta.Y);
    }

    protected void SyncScrollHandlesPositionToScrollOffset()
    {
        FloatRect childrenBounds = GetChildrenBounds();
        Vec2f progress = ScrollOffset / ((Vec2f)childrenBounds.Size - Size.Value);

        if (VerticalScrollHandle is not null)
            VerticalScrollHandle.RelativePosition.Value.Y = (Size.Value.Y - VerticalScrollHandle.Size.Value.Y) * progress.Y;

        if (HorizontalScrollHandle is not null)
            HorizontalScrollHandle.RelativePosition.Value.X = (Size.Value.X - HorizontalScrollHandle.Size.Value.X) * progress.X;
    }


    protected void ClampScrollOffset()
    {
        FloatRect bounds = GetChildrenBounds();

        ScrollOffset.X = Math.Clamp(ScrollOffset.X, 0, bounds.Width - Size.Value.X);
        ScrollOffset.Y = Math.Clamp(ScrollOffset.Y, 0, bounds.Height - Size.Value.Y);
    }


    public FloatRect GetChildrenBounds()
    {
        FloatRect bounds = (from child in Children where !child.HasAttribute<IgnoreScrollAttribute>()
            select new FloatRect(child.RelativePosition.Value, child.GetBounds().Size)).ToArray().GetBoundsOfRects();

        if (bounds.Width < Size.Value.X)
            bounds.Width = Size.Value.X;

        if (bounds.Height < Size.Value.Y)
            bounds.Height = Size.Value.Y;

        return bounds;
    }


    protected void Scroll(Vec2f offset)
    {
        foreach (Element child in Children)
            if (!child.HasAttribute<IgnoreScrollAttribute>())
                child.RelativePosition.Value += offset;

        OnScroll(offset);
    }

    protected void ScrollVertically(float offset) => Scroll(new(y: offset));
    protected void ScrollHorizontally(float offset) => Scroll(new(x: offset));


    protected virtual void OnScroll(Vec2f offset)
        => ScrollEvent?.Invoke(this, offset);
}
