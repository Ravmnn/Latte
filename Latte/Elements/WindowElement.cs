using System;

using SFML.System;

using Latte.Core;
using Latte.Core.Application;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;


using Math = Latte.Core.Math;


namespace Latte.Elements;


[Flags]
public enum WindowElementStyle
{
    Closeable = 1 << 0,
    Resizable = 1 << 1,
    Moveable = 1 << 2,
    
    Default = Closeable | Resizable | Moveable
}


public class WindowElement : RectangleElement, IDraggable
{
    public TextElement Title { get; protected set; }
    
    protected ButtonElement CloseButton { get; }
    
    public WindowElementStyle Style { get; set; }
    
    public bool IsClosed { get; protected set; }
    
    public event EventHandler? OpenEvent;
    public event EventHandler? ClosedEvent;
    
    public bool Dragging { get; protected set; }

    protected Vector2f DraggerPosition { get; set; }
    protected Vector2f LastDraggerPosition { get; set; }
    protected Vector2f DraggerPositionDelta => DraggerPosition - LastDraggerPosition;

    public MouseClickState MouseClickState { get; }
    public bool DisableTruePressOnlyWhenMouseIsUp { get; protected set; }

    public event EventHandler? MouseEnterEvent;
    public event EventHandler? MouseLeaveEvent;
    public event EventHandler? MouseDownEvent;
    public event EventHandler? MouseUpEvent;


    public WindowElement(string title, Vector2f position, Vector2f size, WindowElementStyle style = WindowElementStyle.Default)
        : base(null, position, size)
    {
        Title = new(this, new(), 20, title);
        Title.Alignment = AlignmentType.HorizontalCenter | AlignmentType.Top;
        Title.AlignmentMargin.Set(new(0, 10));
        
        Color.Set(new(50, 50, 50, 220));
        
        CloseButton = new(this, new(), new(15, 15), "")
        {
            Color = { Value = new(255, 100, 100) },
            
            Alignment = AlignmentType.TopRight,
            AlignmentMargin = { Value = new(-7, 8) },
            
            Radius = { Value = 3f }
        };
        CloseButton.MouseUpEvent += (_, _) => Close();

        Style = style;
        
        MouseClickState = new();
        
        DisableTruePressOnlyWhenMouseIsUp = true;
    }
    
    
    public override void Update()
    {
        (this as IDraggable).UpdateClickStateProperties();
        (this as IDraggable).ProcessMouseEvents();
        
        LastDraggerPosition = DraggerPosition;
        DraggerPosition = App.MainWindow.WorldMousePosition;
        
        if (Style.HasFlag(WindowElementStyle.Moveable) && Dragging)
            AbsolutePosition += DraggerPositionDelta;
        
        CloseButton.Visible = Style.HasFlag(WindowElementStyle.Closeable);
        
        base.Update();
    }
    
    
    public void Open() => OnOpen();
    public void Close() => OnClosed();


    protected virtual void OnOpen()
    {
        Show();
        OpenEvent?.Invoke(this, EventArgs.Empty);
    }
    
    protected virtual void OnClosed()
    {
        Hide();
        ClosedEvent?.Invoke(this, EventArgs.Empty);
    }
    
    
    public virtual void OnMouseEnter()
        => MouseEnterEvent?.Invoke(this, EventArgs.Empty);
    
    public virtual void OnMouseLeave()
        => MouseLeaveEvent?.Invoke(this, EventArgs.Empty);

    public virtual void OnMouseDown()
    {
        Dragging = true;
        MouseDownEvent?.Invoke(this, EventArgs.Empty);
    }
    
    public virtual void OnMouseUp()
    {
        Dragging = false;
        MouseUpEvent?.Invoke(this, EventArgs.Empty);
    }

    
    public bool IsPointOver(Vector2f point)
        => Math.IsPointOverRoundedRect(point, AbsolutePosition, Size, Radius.Value);
}