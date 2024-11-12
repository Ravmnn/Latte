using System;
using System.Collections.Generic;

using SFML.System;
using SFML.Graphics;


namespace Latte.Elements;


public abstract class Element : IUpdateable, IDrawable, IAlignable
{
    public Element? Parent { get; set; }
    public List<Element> Children { get; }
    
    public abstract Transformable Transformable { get; }
    
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

    public bool Visible { get; set; }
    public bool DrawElementBoundaries { get; set; }

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

    
    public virtual void Draw(RenderTarget target)
    {
        if (!Visible)
            return;
        
        if (DrawElementBoundaries)
        {
            FloatRect bounds = GetBounds();
            target.Draw(new RectangleShape(bounds.Size)
            {
                Position = bounds.Position,
                FillColor = Color.Transparent,
                OutlineColor = Color.Red,
                OutlineThickness = 1f
            });
        }
        
        DrawEvent?.Invoke(this, EventArgs.Empty);
        
        foreach (Element child in Children)
            child.Draw(target);
    }


    public abstract FloatRect GetBounds();


    public virtual Vector2f GetAlignmentPosition(AlignmentType alignment)
    {
        FloatRect defaultBounds = App.MainWindow!.View.ViewToRect();
        return AlignmentCalculator.GetAlignedPositionOfChild(GetBounds(), Parent?.GetBounds() ?? defaultBounds, alignment);
    }


    protected virtual void UpdateSfmlProperties()
    {
        Transformable.Position = AbsolutePosition;
        Transformable.Origin = Origin;
        Transformable.Rotation = Rotation;
    }
    
    
    public void Show()
        => Visible = true;
    
    public void Hide()
        => Visible = false;
}