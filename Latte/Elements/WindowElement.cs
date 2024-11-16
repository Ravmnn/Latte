using System;

using SFML.System;

using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements;


public abstract class WindowElement : RectangleElement
{
    public TextElement Title { get; protected set; }
    
    public bool IsClosed { get; protected set; }
    
    public event EventHandler? ClosedEvent;
    
    
    public WindowElement(string title, Vector2f position, Vector2f size) : base(null, position, size)
    {
        Color = new(50, 50, 50, 220);

        Title = new(this, new(), 20, title)
        {
            Alignment = AlignmentType.HorizontalCenter | AlignmentType.Top,
            AlignmentMargin = new(0, 10)
        };
    }


    public void Close() => OnClosed();


    protected virtual void OnClosed()
    {
        Hide();
        ClosedEvent?.Invoke(this, EventArgs.Empty);
    }
}