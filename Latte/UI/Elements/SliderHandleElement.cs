using System;

using Latte.Core.Type;
using Latte.Application;


namespace Latte.UI.Elements;


// TODO: a slider's behavior is very similar to scroll handle... isn't it possible to reutilize one of them?
// TODO: progress bar element also is very similar. Try to extern progress bar's behavior to a calculation static class

public class SliderHandleElement : ButtonElement, IDraggable
{
    public new SliderElement Parent => (base.Parent as SliderElement)!;

    public IDraggable ThisDraggable => this;

    // TODO:
    // decide how to organize this section globally in the project... Separate by interface implementation?
    // all events in one part?

    public Orientation Orientation { get; }

    public bool Dragging { get; protected set; }
    public bool WasDragging { get; protected set; }

    public event EventHandler? DragBeginEvent;
    public event EventHandler? DragEndEvent;
    public event EventHandler? DraggingEvent;


    public SliderHandleElement(SliderElement parent, Orientation orientation)
        : base(parent,new Vec2f(), new Vec2f(20, 20), null)
    {
        Orientation = orientation;
        Alignment = Orientation == Orientation.Horizontal ? Alignment.VerticalCenter : Alignment.HorizontalCenter;
        ClipLayerIndexOffset = -1;
        Color = SFML.Graphics.Color.Red;
    }


    public override void Update()
    {
        ThisDraggable.ProcessDraggingEvents();

        UpdatePositionBasedOnNormalizedValue();
        ClampPosition();

        base.Update();
    }


    public virtual void ProcessDragging()
    {
        if (Orientation == Orientation.Horizontal)
            Parent.Value += MouseInput.PositionDeltaInView.X * Parent.Proportion;
        else
            Parent.Value += MouseInput.PositionDeltaInView.Y * Parent.Proportion;
    }


    protected void ClampPosition()
    {
        if (Orientation == Orientation.Horizontal)
            RelativePosition.X = Math.Clamp(RelativePosition.X, 0, Parent.Size.X - Size.X);
        else
            RelativePosition.Y = Math.Clamp(RelativePosition.Y, 0, Parent.Size.Y - Size.Y);
    }


    private void UpdatePositionBasedOnNormalizedValue()
    {
        if (Orientation == Orientation.Horizontal)
            RelativePosition.X = (Parent.Size.X - Size.X) * Parent.NormalizedValue;
        else
            RelativePosition.Y = (Parent.Size.Y - Size.Y) * Parent.NormalizedValue;
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


    public virtual void OnDragBegin()
        => DragBeginEvent?.Invoke(this, EventArgs.Empty); // TODO: create extension method object.Trigger(event?)

    public virtual void OnDragEnd()
        => DragEndEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnDragging()
    {
        ProcessDragging();
        DraggingEvent?.Invoke(this, EventArgs.Empty);
    }
}
