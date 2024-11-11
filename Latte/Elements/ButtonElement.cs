using System;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using Latte.Elements.Shapes;


namespace Latte.Elements;


public class ButtonElement : RectangleElement, IClickable
{
    public TextElement Text { get; protected set; }

    protected bool IsMouseHover { get; set; }
    protected bool WasMouseHover { get; set; }
    protected bool IsMouseDown { get; set; }
    protected bool WasMouseDown { get; set; }

    public event EventHandler? MouseEnterEvent;
    public event EventHandler? MouseLeaveEvent;
    public event EventHandler? MouseDownEvent;
    public event EventHandler? MouseUpEvent;
    
    public Color NormalColor { get; set; }
    public Color HoverColor { get; set; }
    public Color DownColor { get; set; }
    
    
    public ButtonElement(Element? parent, Vector2f position, Vector2f size, string text) : base(parent, position, size)
    {
        Text = new(this, new(), 32, text)
        {
            Alignment = AlignmentType.Center,
            Text = { FillColor = Color.Black }
        };
        
        SetColorVariants(Color);
    }


    public override void Update()
    {
        IsMouseHover = IsPointOver(MainWindow.Current!.WorldMousePosition);
        IsMouseDown = IsMouseHover && Mouse.IsButtonPressed(Mouse.Button.Left);
    
        if (IsMouseDown)
            OnMouseDown();
        
        else if (WasMouseDown)
            OnMouseUp();
        
        if (IsMouseHover && !WasMouseHover)
            OnMouseEnter();
        
        else if (!IsMouseHover && WasMouseHover)
            OnMouseLeave();
            
        WasMouseHover = IsMouseHover;
        WasMouseDown = IsMouseDown;
        
        base.Update();
    }
    
    
    protected virtual void OnMouseEnter()
    {
        Color = HoverColor;
        MouseEnterEvent?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnMouseLeave()
    {
        Color = NormalColor;
        MouseLeaveEvent?.Invoke(this, EventArgs.Empty);
    }
    
    protected virtual void OnMouseDown()
    {
        Color = DownColor;
        MouseDownEvent?.Invoke(this, EventArgs.Empty);
    }
    
    protected virtual void OnMouseUp()
    {
        Color = IsMouseHover ? HoverColor : NormalColor;
        MouseUpEvent?.Invoke(this, EventArgs.Empty);
    }



    public virtual bool IsPointOver(Vector2f point)
        => GetBounds().Contains(point);


    public void SetColorVariants(Color color)
    {
        const int defaultColorDecrement = 50;
        
        NormalColor = color;
        
        color.R -= defaultColorDecrement;
        color.G -= defaultColorDecrement;
        color.B -= defaultColorDecrement;
        
        HoverColor = color;
        
        color.R -= defaultColorDecrement;
        color.G -= defaultColorDecrement;
        color.B -= defaultColorDecrement;
        
        DownColor = color;
    }
}