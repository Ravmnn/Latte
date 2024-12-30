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

// TODO: add graphic scroll handler


public class ScrollAreaHandleElement : ButtonElement, IDefaultDraggable
{
    public bool Dragging { get; set; }
    public bool WasDragging { get; set; }

    public event EventHandler? DragBeginEvent;
    public event EventHandler? DragEndEvent;
    public event EventHandler? DraggingEvent;


    public ScrollAreaHandleElement(ScrollAreaElement? parent) : base(parent, new(0, 100), new(10, 30), null)
    {
        Radius.Set(0f);
        Alignment.Set(Alignments.Right);
        Color.Set(SFML.Graphics.Color.Red);
        BorderSize.Set(0f);

        DisableTruePressOnlyWhenMouseIsUp = true;
    }


    public override void Update()
    {
        (this as IDefaultDraggable).ProcessDraggingEvents();

        WasDragging = Dragging;

        base.Update();
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


public class ScrollAreaElement : ButtonElement
{
    public ScrollAreaHandleElement? ScrollHandle { get; set; }

    public float ScrollStep { get; set; }
    public ScrollDirection Direction { get; set; }

    public event EventHandler<Vec2f>? ScrollEvent;


    public ScrollAreaElement(Element? parent, Vec2f position, Vec2f size) : base(parent, position, size, null)
    {
        ScrollHandle = new(this);

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
