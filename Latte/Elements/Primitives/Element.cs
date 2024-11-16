using System;
using System.Collections.Generic;

using SFML.System;
using SFML.Graphics;

using OpenTK.Graphics.OpenGL;

using Latte.Application;


namespace Latte.Elements.Primitives;


public abstract class Element : IUpdateable, IDrawable, IAlignable
{
    public Element? Parent { get; set; }
    public List<Element> Children { get; }
    
    public abstract Transformable Transformable { get; }
    
    public bool Visible { get; set; }
    public bool ShouldDrawElementBoundaries { get; set; }
    public bool ShouldDrawClipArea { get; set; }
    
    public Vector2f Position { get; set; }
    public Vector2f AbsolutePosition
    {
        get => Parent is not null ? Position + Parent.AbsolutePosition : Position;
        set
        {
            if (Parent is not null)
                Position = value - Parent.AbsolutePosition;
            else
                Position = value;
        }
    }
    
    public Vector2f Origin { get; set; }

    public float Rotation { get; set; }
    
    public AlignmentType? Alignment { get; set; }
    public Vector2f AlignmentMargin { get; set; }

    public event EventHandler? UpdateEvent;
    public event EventHandler? DrawEvent;


    protected Element(Element? parent)
    {
        Parent = parent;
        Children = [];
     
        Visible = true;
        
        Parent?.Children.Add(this);
    }


    public virtual void Update()
    {
        if (!Visible)
            return;
        
        if (Alignment is not null)
            AbsolutePosition = GetAlignmentPosition(Alignment.Value) + AlignmentMargin;
     
        UpdateSfmlProperties();
        
        UpdateEvent?.Invoke(this, EventArgs.Empty);
        
        foreach (Element child in Children)
            child.Update();
    }
    
    
    protected virtual void UpdateSfmlProperties()
    {
        Transformable.Position = AbsolutePosition;
        Transformable.Origin = Origin;
        Transformable.Rotation = Rotation;
    }

    
    public virtual void Draw(RenderTarget target)
    {
        if (ShouldDrawElementBoundaries)
            Debug.DrawLineRect(target, GetBounds(), Color.Red);
        
        if (ShouldDrawClipArea)
            Debug.DrawLineRect(target, (FloatRect)GetClipAreaOrWindow(), Color.Magenta);
        
        if (!Visible)
            return;
        
        DrawEvent?.Invoke(this, EventArgs.Empty);
        
        foreach (Element child in Children)
            child.Draw(target);
    }

    protected virtual void BeginDraw()
    {
        IntRect clipArea = GetFinalClipArea();
        Vector2u windowSize = App.MainWindow.Size;
        
        GL.Enable(EnableCap.ScissorTest);
        
        // the Y parameter needs to be converted to OpenGL coordinate system
        GL.Scissor(clipArea.Left, (int)windowSize.Y - clipArea.Height - clipArea.Top, clipArea.Width, clipArea.Height);
    }

    protected virtual void EndDraw()
    {
        GL.Disable(EnableCap.ScissorTest);
    }


    protected IntRect GetFinalClipArea()
    {
        Element? element = this;
        IntRect? area = null;
        
        do
        {
            IntRect newArea = element.GetClipAreaOrWindow();

            if (area is null)
                area = newArea;
            
            else if (area.Value.Intersects(newArea, out IntRect overlap))
                area = overlap;
                
            element = element.Parent;
        }
        while (element is not null);

        return area.Value;
    }

    
    protected IntRect GetClipAreaOrWindow() 
        => Parent?.GetThisClipArea() ?? App.MainWindow.RectSize;

    protected virtual IntRect GetThisClipArea() => WorldFloatRectToClipArea(GetBounds());


    protected static IntRect WorldFloatRectToClipArea(FloatRect rect)
    {
        Vector2i transformedPosition = App.MainWindow.MapCoordsToPixel(rect.Position);
        Vector2i transformedSize = App.MainWindow.MapCoordsToPixel(rect.Position + rect.Size) - transformedPosition;
        
        return new(transformedPosition, transformedSize);
    }


    public abstract FloatRect GetBounds();
    
    
    protected FloatRect ParentOrWindowBounds()
        => Parent?.GetBounds() ?? (FloatRect)App.MainWindow.RectSize;


    public virtual Vector2f GetAlignmentPosition(AlignmentType alignment)
        => AlignmentCalculator.GetAlignedPositionOfChild(GetBounds(), ParentOrWindowBounds(), alignment);
    
    
    public void Show()
        => Visible = true;
    
    public void Hide()
        => Visible = false;
}