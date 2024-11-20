using System;

using SFML.System;
using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;
using OpenTK.Graphics.OpenGL4;
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


public class WindowElement : RectangleElement, IDefaultDraggable, IDefaultResizable
{
    public TextElement Title { get; protected set; }
    
    protected ButtonElement CloseButton { get; }
    
    public WindowElementStyle Style { get; set; }
    
    public bool IsClosed { get; protected set; }
    
    public event EventHandler? OpenEvent;
    public event EventHandler? ClosedEvent;
    
    public bool Dragging { get; protected set; }
    public bool WasDragging { get; protected set; }
    
    public bool Resizing { get; protected set; }
    public bool WasResizing { get; protected set; }

    protected Vector2f DraggerPosition { get; set; }
    protected Vector2f LastDraggerPosition { get; set; }
    protected Vector2f DraggerPositionDelta => DraggerPosition - LastDraggerPosition;

    public MouseClickState ClickState { get; }
    public bool DisableTruePressOnlyWhenMouseIsUp { get; protected set; }
    
    public MouseCornerState ResizeCorner { get; }
    public FloatRect Rect => new(Position.Value, Size.Value);
    public float CornerSize { get; protected set; }
    
    public Vec2f MinSize { get; set; }
    public Vec2f MaxSize { get; set; }

    public event EventHandler? MouseEnterEvent;
    public event EventHandler? MouseLeaveEvent;
    public event EventHandler? MouseDownEvent;
    public event EventHandler? MouseUpEvent;

    public event EventHandler? DragBeginEvent;
    public event EventHandler? DragEndEvent;
    public event EventHandler? DraggingEvent;
    
    public event EventHandler? ResizeBeginEvent;
    public event EventHandler? ResizeEndEvent;
    public event EventHandler? ResizingEvent;


    public WindowElement(string title, Vec2f position, Vec2f size, WindowElementStyle style = WindowElementStyle.Default)
        : base(null, position, size)
    {
        Title = new(this, new(), 20, title);
        Title.Alignment.Set(AlignmentType.HorizontalCenter | AlignmentType.Top);
        Title.AlignmentMargin.Set(new(0, 10));
        
        Color.Set(new(50, 50, 50, 220));
        
        CloseButton = new(this, new(), new(15, 15), "")
        {
            Color = { Value = new(255, 100, 100) },
            
            Alignment = { Value = AlignmentType.TopRight },
            AlignmentMargin = { Value = new(-7, 8) },
            
            Down =
            {
                { "Scale", new Vec2f(0.95f, 0.95f) }
            }
        };
        CloseButton.MouseUpEvent += (_, _) => Close();

        Style = style;

        ClickState = new();
        DisableTruePressOnlyWhenMouseIsUp = true;
        
        ResizeCorner = new();
        CornerSize = 10f;

        MinSize = new(50, 50);
    }
    
    
    public override void Update()
    {
        if (!Visible)
            return;
        
        (this as IDefaultClickable).UpdateClickStateProperties();
        (this as IDefaultClickable).ProcessMouseEvents();
        
        (this as IDefaultResizable).UpdateCornerStateProperties();
        
        App.MainWindow.SetMouseCursor((this as IDefaultResizable).GetCursorTypeFromResizeCorner());
        
        LastDraggerPosition = DraggerPosition;
        DraggerPosition = App.MainWindow.WorldMousePosition;

        if (Style.HasFlag(WindowElementStyle.Resizable))
            (this as IDefaultResizable).ProcessResizingEvents();
            
        if (Style.HasFlag(WindowElementStyle.Moveable))
            (this as IDefaultDraggable).ProcessDraggingEvents();
        
        CloseButton.Visible = Style.HasFlag(WindowElementStyle.Closeable);

        WasDragging = Dragging;
        WasResizing = Resizing;
        
        base.Update();
    }


    public void ProcessDragging()
    {
        AbsolutePosition += DraggerPositionDelta;
    }


    public void ProcessResizing()
    {
        if (ResizeCorner.Top)
            ResizeCornerByDraggingDeltaFactor(topFactor: 1, bottomFactor: -1);
        
        else if (ResizeCorner.Bottom)
            ResizeCornerByDraggingDeltaFactor(bottomFactor: 1);

        if (ResizeCorner.Left)
            ResizeCornerByDraggingDeltaFactor(leftFactor: 1, rightFactor: -1);
        
        else if (ResizeCorner.Right)
            ResizeCornerByDraggingDeltaFactor(rightFactor: 1);
    }


    private void ResizeCornerByDraggingDeltaFactor(int leftFactor = 0, int topFactor = 0, int rightFactor = 0, int bottomFactor = 0)
    {
        Position.Value.X += DraggerPositionDelta.X * leftFactor;
        Position.Value.Y += DraggerPositionDelta.Y * topFactor;
        Size.Value.X += DraggerPositionDelta.X * rightFactor;
        Size.Value.Y += DraggerPositionDelta.Y * bottomFactor;
        
        CheckCornerMinSize(leftFactor != 0, topFactor != 0, rightFactor != 0, bottomFactor != 0);
    }
    
    private void CheckCornerMinSize(bool left = false, bool top = false, bool right = false, bool bottom = false)
    {
        if ((right && Size.Value.X >= MinSize.X) || (bottom && Size.Value.Y >= MinSize.Y))
            return;
        
        Vec2f difference = MinSize - Size.Value;
                
        if (left) Position.Value.X -= difference.X;
        if (top) Position.Value.Y -= difference.Y;
        if (right) Size.Value.X += difference.X;
        if (bottom) Size.Value.Y += difference.Y;
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
        Resizing = ResizeCorner.Any;
        Dragging = !Resizing; // don't drag while resizing
        
        MouseDownEvent?.Invoke(this, EventArgs.Empty);
    }
    
    public virtual void OnMouseUp()
    {
        Dragging = false;
        Resizing = false;
        
        MouseUpEvent?.Invoke(this, EventArgs.Empty);
    }
    
    
    public virtual void OnDragBegin() => DragBeginEvent?.Invoke(this, EventArgs.Empty);
    public virtual void OnDragEnd() => DragEndEvent?.Invoke(this, EventArgs.Empty);
    
    public virtual void OnDragging()
    {
        ProcessDragging();
        DraggingEvent?.Invoke(this, EventArgs.Empty);
    }


    public virtual void OnResizeBegin() => ResizeBeginEvent?.Invoke(this, EventArgs.Empty);
    public virtual void OnResizeEnd() => ResizeEndEvent?.Invoke(this, EventArgs.Empty);
    
    public virtual void OnResizing()
    {
        ProcessResizing();
        ResizingEvent?.Invoke(this, EventArgs.Empty);
    }

    
    public bool IsPointOver(Vec2f point)
        => Math.IsPointOverRoundedRect(point, AbsolutePosition, Size, Radius.Value);
}