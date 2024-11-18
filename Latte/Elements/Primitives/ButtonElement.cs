using System;

using SFML.System;
using SFML.Graphics;

using Latte.Core;
using Latte.Elements.Primitives.Shapes;


using Math = Latte.Core.Math;


namespace Latte.Elements.Primitives;


public class ButtonElement : RectangleElement, IDefaultClickable
{
    public TextElement Text { get; protected set; }

    public MouseClickState MouseClickState { get; }
    public bool Continuous { get; protected set; }
    
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
            Text = { FillColor = SFML.Graphics.Color.Black }
        };
        
        BorderColor.Set(new(100, 100, 100));
        BorderSize.Set(3f);

        MouseClickState = new();
        Continuous = false;
        
        SetColorVariants(Color.Value);
    }


    public override void Update()
    {
        (this as IDefaultClickable).UpdateClickStateProperties();
        (this as IDefaultClickable).ProcessMouseEvents();
        
        base.Update();
    }
    
    
    public virtual void OnMouseEnter()
    {
        Color.Set(HoverColor);
        MouseEnterEvent?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnMouseLeave()
    {
        Color.Set(NormalColor);
        MouseLeaveEvent?.Invoke(this, EventArgs.Empty);
    }
    
    public virtual void OnMouseDown()
    {
        Color.Set(DownColor);
        MouseDownEvent?.Invoke(this, EventArgs.Empty);
    }
    
    public virtual void OnMouseUp()
    {
        Color.Set(MouseClickState.IsMouseHover ? HoverColor : NormalColor);
        MouseUpEvent?.Invoke(this, EventArgs.Empty);
    }



    public virtual bool IsPointOver(Vector2f point)
        => Math.IsPointOverRoundedRect(point, AbsolutePosition, Size, Radius.Value);


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