using System;

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


    public void Scroll(Vec2f offset)
    {
        CurrentScrollOffset += offset;

        foreach (Element child in Children)
            child.RelativePosition.Value += offset;

        OnScroll(offset);
    }

    public void ScrollVertically(float offset) => Scroll(new(y: offset));
    public void ScrollHorizontally(float offset) => Scroll(new(x: offset));


    protected virtual void OnScroll(Vec2f offset)
        => ScrollEvent?.Invoke(this, offset);
}
