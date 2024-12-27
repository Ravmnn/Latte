using System;
using System.Linq;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Core.Type;


namespace Latte.Elements.Primitives;


public enum ScrollDirection
{
    Vertical,
    Horizontal
}


// TODO: add limits
// TODO: add graphic scroll handler

public class ScrollAreaElement : ButtonElement
{
    public Vec2f CurrentScrollOffset { get; private set; }

    public float ScrollStep { get; set; }

    public ScrollDirection Direction { get; set; }

    public event EventHandler<Vec2f>? ScrollEvent;


    public ScrollAreaElement(Element? parent, Vec2f position, Vec2f size) : base(parent, position, size, null)
    {
        CurrentScrollOffset = new();
        ScrollStep = 10f;

        Direction = ScrollDirection.Vertical;

        UseDefaultAnimation = false;
    }


    public override void Update()
    {
        if (MouseState.IsMouseOver)
            ScrollBasedOnAppMouseScrollDelta();

        base.Update();
    }

    protected void ScrollBasedOnAppMouseScrollDelta()
    {
        switch (Direction)
        {
            case ScrollDirection.Vertical:
                ScrollVertically(ScrollStep * App.MouseScrollDelta);
                break;

            case ScrollDirection.Horizontal:
                ScrollHorizontally(ScrollStep * App.MouseScrollDelta);
                break;
        }
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


    public void Scroll(Vec2f offset)
    {
        CurrentScrollOffset += offset;

        foreach (Element child in Children)
            child.RelativePosition.Value += offset;

        if (IsRelativeBoundsOutsideChildrenBounds())
            Scroll(-offset);

        OnScroll(offset);
    }

    public void ScrollVertically(float offset) => Scroll(new(y: offset));
    public void ScrollHorizontally(float offset) => Scroll(new(x: offset));


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
