using System;

using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements;


public abstract class PopupElement : RectangleElement
{
    public TextElement Title { get; protected set; }
    
    public bool IsClosed { get; protected set; }
    
    public event EventHandler? ClosedEvent;
    
    
    public PopupElement(string title) : base(null, new(), new())
    {
        Size = new(300, 200);
        Color = new(50, 50, 50, 200);
        Alignment = AlignmentType.Center;

        Title = new(this, new(), 20, title)
        {
            Alignment = AlignmentType.HorizontalCenter | AlignmentType.Top,
            AlignmentMargin = new(0, 10)
        };
        
        // not visible by default
        Hide();
    }


    public void Close() => OnClosed();


    protected virtual void OnClosed()
    {
        Hide();
        ClosedEvent?.Invoke(this, EventArgs.Empty);
    }
}