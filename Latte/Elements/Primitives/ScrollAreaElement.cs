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

// TODO: add graphic scroll handler


[IgnoreScroll]
public class ScrollAreaHandleElement : ButtonElement, IDefaultDraggable
{
    public new ScrollAreaElement Parent => (base.Parent as ScrollAreaElement)!;

    public bool Dragging { get; set; }
    public bool WasDragging { get; set; }

    public event EventHandler? DragBeginEvent;
    public event EventHandler? DragEndEvent;
    public event EventHandler? DraggingEvent;


    public ScrollAreaHandleElement(ScrollAreaElement parent) : base(parent, new(), new(10, 10), null)
    {
        Radius.Set(2f);
        Alignment.Set(Alignments.Right);
        BorderSize.Set(0f);

        DisableTruePressOnlyWhenMouseIsUp = true;
    }


    public override void Update()
    {
        (this as IDefaultDraggable).ProcessDraggingEvents();

        UpdateSize();

        ClampPosition();
        UpdateScrollArea();

        WasDragging = Dragging;

        base.Update();
    }

    protected void ClampPosition()
    {
        RelativePosition.Value.Y = Math.Clamp(RelativePosition.Value.Y, 0, Parent.Size.Value.Y - Size.Value.Y);
    }

    protected void UpdateScrollArea()
    {
        FloatRect childrenBounds = Parent.GetChildrenBounds();
        Vec2f parentSize = Parent.Size.Value;

        float verticalProgress = RelativePosition.Value.Y / (parentSize.Y - Size.Value.Y);

        Parent.ScrollOffset.Y = (childrenBounds.Height - parentSize.Y) * verticalProgress;
    }

    protected void UpdateSize()
    {
        FloatRect bounds = Parent.GetChildrenBounds();
        Vec2f parentSize = Parent.Size.Value;

        float heightRatio = parentSize.Y / bounds.Height;

        Size.Value.Y = heightRatio * parentSize.Y;
    }


    public void ProcessDragging()
    {
        RelativePosition.Value.Y += App.ElementViewMousePositionDelta.Y;
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
}

// TODO: when scrolling using mouse wheel, sync scroll handle position too

public class ScrollAreaElement : ButtonElement
{
    public ScrollAreaHandleElement? ScrollHandle { get; set; }
    public bool IsHandlePressed => ScrollHandle?.MouseState.IsTruePressed ?? false;

    public Vec2f ScrollOffset { get; set; }
    protected Vec2f LastScrollOffset { get; set; }
    public Vec2f ScrollOffsetDelta => ScrollOffset - LastScrollOffset;
    public float ScrollOffsetStep { get; set; }

    public ScrollDirection Direction { get; set; }

    public event EventHandler<Vec2f>? ScrollEvent;


    public ScrollAreaElement(Element? parent, Vec2f position, Vec2f size) : base(parent, position, size, null)
    {
        ScrollHandle = new(this);

        ScrollOffset = new();
        LastScrollOffset = new();

        ScrollOffsetStep = 10f;
        Direction = ScrollDirection.Vertical;

        UseDefaultAnimation = false;
    }


    public override void Update()
    {
        if (ScrollHandle is not null)
            ScrollHandle.Visible = GetChildrenBounds().Size.Y > Size.Value.Y;

        ClampScrollOffset();

        if (IsHandlePressed)
            ScrollBasedOnScrollDelta();

        else if (MouseState.IsMouseOver)
            ScrollBasedOnAppMouseScrollDelta();

        LastScrollOffset = ScrollOffset.Copy();

        base.Update();
    }

    protected void ScrollBasedOnAppMouseScrollDelta()
    {
        switch (Direction)
        {
            case ScrollDirection.Vertical:
                ScrollVertically(ScrollOffsetStep * App.MouseScrollDelta);
                break;

            case ScrollDirection.Horizontal:
                ScrollHorizontally(ScrollOffsetStep * App.MouseScrollDelta);
                break;
        }
    }

    protected void ScrollBasedOnScrollDelta()
    {
        if (ScrollOffsetDelta.X != 0)
            ScrollHorizontally(-ScrollOffsetDelta.X);

        if (ScrollOffsetDelta.Y != 0)
            ScrollVertically(-ScrollOffsetDelta.Y);
    }

    protected void ClampScrollOffset()
    {
        FloatRect bounds = GetChildrenBounds();

        ScrollOffset.X = Math.Clamp(ScrollOffset.X, 0, bounds.Width);
        ScrollOffset.Y = Math.Clamp(ScrollOffset.Y, 0, bounds.Height);
    }

    public FloatRect GetChildrenBounds()
    {
        FloatRect bounds = (from child in Children select new FloatRect(child.RelativePosition.Value, child.GetBounds().Size)).ToArray().GetBoundsOfRects();

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

        if (IsRelativeBoundsOutsideChildrenBounds())
            Scroll(-offset);

        OnScroll(offset);
    }

    protected void ScrollVertically(float offset) => Scroll(new(y: offset));
    protected void ScrollHorizontally(float offset) => Scroll(new(x: offset));


    protected bool IsRelativeBoundsOutsideChildrenBounds()
    {
        FloatRect childrenBounds = GetChildrenBounds();

        return childrenBounds.Left > 0 || childrenBounds.Top > 0 ||
               childrenBounds.Left + childrenBounds.Width < Size.Value.X ||
               childrenBounds.Top + childrenBounds.Height < Size.Value.Y;
    }


    protected virtual void OnScroll(Vec2f offset)
        => ScrollEvent?.Invoke(this, offset);
}
