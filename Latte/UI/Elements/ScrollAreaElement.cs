using System;
using System.Linq;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Application;
using Latte.UI.Elements.Attributes;


using Math = System.Math;


namespace Latte.UI.Elements;




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


    public Orientation Orientation { get; set; }


    public event EventHandler<Vec2f>? ScrollEvent;




    public ScrollAreaElement(Element? parent, Vec2f? position, Vec2f size, Orientation orientation = Orientation.Vertical)
            : base(parent, position, size, null)
    {
        if (orientation.HasFlag(Orientation.Vertical))
            VerticalScrollHandle = new ScrollAreaHandleElement(this, Orientation.Vertical);

        if (orientation.HasFlag(Orientation.Horizontal))
            HorizontalScrollHandle = new ScrollAreaHandleElement(this, Orientation.Horizontal);

        ScrollOffset = new Vec2f();
        LastScrollOffset = new Vec2f();

        ScrollOffsetStep = 10f;
        Orientation = Orientation.Vertical;

        DisableFocus = true;
    }




    public override void Update()
    {
        UpdateScrollHandlesVisibility();

        var element = MouseInput.TrueClickableWhichCaughtMouseInput as Element;
        var isMouseHover = element?.IsChildOf(this) ?? false;

        if (!IsAnyHandlePressed && isMouseHover)
            MouseDragScroll();

        ClampScrollOffset();
        MouseWheelScroll();

        LastScrollOffset = ScrollOffset.Copy();

        base.Update();
    }


    private void UpdateScrollHandlesVisibility()
    {
        var childrenBounds = GetClampedChildrenBounds();

        if (VerticalScrollHandle is not null)
            VerticalScrollHandle.Visible = childrenBounds.Size.Y > Size.Y;

        if (HorizontalScrollHandle is not null)
            HorizontalScrollHandle.Visible = childrenBounds.Size.X > Size.X;
    }


    protected void MouseDragScroll()
    {
        var step = ScrollOffsetStep * -MouseInput.ScrollDelta;

        switch (Orientation)
        {
            case Orientation.Vertical:
                ScrollOffset.Y += step;
                break;

            case Orientation.Horizontal:
                ScrollOffset.X += step;
                break;
        }

        SyncScrollHandlesPositionToScrollOffset();
    }


    protected void MouseWheelScroll()
    {
        if (ScrollOffsetDelta.X != 0)
            ScrollHorizontally(-ScrollOffsetDelta.X);

        if (ScrollOffsetDelta.Y != 0)
            ScrollVertically(-ScrollOffsetDelta.Y);
    }


    protected void SyncScrollHandlesPositionToScrollOffset()
    {
        var childrenBounds = GetClampedChildrenBounds();
        Vec2f progress = ScrollOffset / ((Vec2f)childrenBounds.Size - Size);

        if (VerticalScrollHandle is not null)
            VerticalScrollHandle.RelativePosition.Y = (Size.Y - VerticalScrollHandle.Size.Y) * progress.Y;

        if (HorizontalScrollHandle is not null)
            HorizontalScrollHandle.RelativePosition.X = (Size.X - HorizontalScrollHandle.Size.X) * progress.X;
    }


    protected void ClampScrollOffset()
    {
        var bounds = GetClampedChildrenBounds();

        ScrollOffset.X = Math.Clamp(ScrollOffset.X, 0, bounds.Width - Size.X);
        ScrollOffset.Y = Math.Clamp(ScrollOffset.Y, 0, bounds.Height - Size.Y);
    }




    public FloatRect GetClampedChildrenBounds()
    {
        var bounds = (from child in Children where !child.HasAttribute<IgnoreScrollAttribute>()
            select new FloatRect(child.RelativePosition, child.GetBounds().Size)).ToArray().GetBoundsOfRects();

        if (bounds.Width < Size.X)
            bounds.Width = Size.X;

        if (bounds.Height < Size.Y)
            bounds.Height = Size.Y;

        return bounds;
    }




    protected void Scroll(Vec2f offset)
    {
        foreach (var child in Children)
            if (!child.HasAttribute<IgnoreScrollAttribute>())
                child.RelativePosition += offset;

        OnScroll(offset);
    }


    protected void ScrollVertically(float offset) => Scroll(new Vec2f(y: offset));
    protected void ScrollHorizontally(float offset) => Scroll(new Vec2f(x: offset));




    protected virtual void OnScroll(Vec2f offset)
        => ScrollEvent?.Invoke(this, offset);
}
