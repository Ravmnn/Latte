using System;
using System.Linq;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements.Attributes;


using Math = System.Math;


namespace Latte.Elements.Primitives;


public enum ScrollDirection
{
    Vertical,
    Horizontal
}


// BUG: scrolling is not 100% accurate.


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


    public ScrollAreaElement(Element? parent, Vec2f? position, Vec2f size, bool verticalScrollHandle = true, bool horizontalScrollHandle = false)
            : base(parent, position, size, null)
    {
        if (verticalScrollHandle)
            VerticalScrollHandle = new ScrollAreaHandleElement(this, ScrollDirection.Vertical);

        if (horizontalScrollHandle)
            HorizontalScrollHandle = new ScrollAreaHandleElement(this, ScrollDirection.Horizontal);

        ScrollOffset = new Vec2f();
        LastScrollOffset = new Vec2f();

        ScrollOffsetStep = 10f;
        Direction = ScrollDirection.Vertical;

        DisableFocus = true;
    }


    public override void Update()
    {
        UpdateScrollHandlesVisibility();

        var isMouseHover = MouseInput.TrueElementWhichCaughtMouseInput?.IsChildOf(this) ?? false;

        if (!IsAnyHandlePressed && isMouseHover)
            AddAppMouseScrollDeltaToScrollOffset();

        ClampScrollOffset();
        ScrollBasedOnScrollDelta();

        LastScrollOffset = ScrollOffset.Copy();

        base.Update();
    }

    private void UpdateScrollHandlesVisibility()
    {
        var childrenBounds = GetClampedChildrenBounds();

        if (VerticalScrollHandle is not null)
            VerticalScrollHandle.Visible = childrenBounds.Size.Y > Size.Value.Y;

        if (HorizontalScrollHandle is not null)
            HorizontalScrollHandle.Visible = childrenBounds.Size.X > Size.Value.X;
    }

    protected void AddAppMouseScrollDeltaToScrollOffset()
    {
        var step = ScrollOffsetStep * -MouseInput.ScrollDelta;

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
        var childrenBounds = GetClampedChildrenBounds();
        Vec2f progress = ScrollOffset / ((Vec2f)childrenBounds.Size - Size.Value);

        if (VerticalScrollHandle is not null)
            VerticalScrollHandle.RelativePosition.Value.Y = (Size.Value.Y - VerticalScrollHandle.Size.Value.Y) * progress.Y;

        if (HorizontalScrollHandle is not null)
            HorizontalScrollHandle.RelativePosition.Value.X = (Size.Value.X - HorizontalScrollHandle.Size.Value.X) * progress.X;
    }


    protected void ClampScrollOffset()
    {
        var bounds = GetClampedChildrenBounds();

        ScrollOffset.X = Math.Clamp(ScrollOffset.X, 0, bounds.Width - Size.Value.X);
        ScrollOffset.Y = Math.Clamp(ScrollOffset.Y, 0, bounds.Height - Size.Value.Y);
    }


    public FloatRect GetClampedChildrenBounds()
    {
        var bounds = (from child in Children where !child.HasAttribute<IgnoreScrollAttribute>()
            select new FloatRect(child.RelativePosition.Value, child.GetBounds().Size)).ToArray().GetBoundsOfRects();

        if (bounds.Width < Size.Value.X)
            bounds.Width = Size.Value.X;

        if (bounds.Height < Size.Value.Y)
            bounds.Height = Size.Value.Y;

        return bounds;
    }


    protected void Scroll(Vec2f offset)
    {
        foreach (var child in Children)
            if (!child.HasAttribute<IgnoreScrollAttribute>())
                child.RelativePosition.Value += offset;

        OnScroll(offset);
    }

    protected void ScrollVertically(float offset) => Scroll(new Vec2f(y: offset));
    protected void ScrollHorizontally(float offset) => Scroll(new Vec2f(x: offset));


    protected virtual void OnScroll(Vec2f offset)
        => ScrollEvent?.Invoke(this, offset);
}
