using System;
using System.Collections.Generic;

using SFML.System;
using SFML.Graphics;
using SFML.Graphics.Glsl;

using Latte.Application;


namespace Latte.Elements;


public abstract class Element : IUpdateable, IDrawable, IAlignable
{
    public Element? Parent { get; set; }
    public List<Element> Children { get; }
    
    public abstract Transformable Transformable { get; }
    
    protected RenderTexture BufferTexture { get; set; }
    
    public bool Visible { get; set; }
    public bool ShouldDrawElementBoundaries { get; set; }
    
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
        Vector2f size = ParentOrWindowBounds().Size;
        BufferTexture = new((uint)size.X, (uint)size.Y);
        
        if (!Visible)
            return;
        
        if (Alignment is not null)
            AbsolutePosition = GetAlignmentPosition(Alignment.Value) + AlignmentMargin;
     
        UpdateSfmlProperties();
        
        UpdateEvent?.Invoke(this, EventArgs.Empty);
        
        foreach (Element child in Children)
            child.Update();
    }
    
    
    protected void UpdateClipShaderParameters()
    {
        FloatRect bounds = ParentOrWindowBounds();
        
        Loaded.ClipShader.SetUniform("texture", BufferTexture.Texture);
        Loaded.ClipShader.SetUniform("clipArea", new Vec4(bounds.Left, bounds.Top, bounds.Width, bounds.Height));
    }
    
    
    protected virtual void UpdateSfmlProperties()
    {
        Transformable.Position = AbsolutePosition;
        Transformable.Origin = Origin;
        Transformable.Rotation = Rotation;
    }

    
    public virtual void Draw(RenderTarget target)
    {
        if (!Visible)
            return;
        
        if (ShouldDrawElementBoundaries)
            DrawElementBoundaries(target);
        
        DrawEvent?.Invoke(this, EventArgs.Empty);
        
        foreach (Element child in Children)
            child.Draw(target);
    }


    protected void DrawElementBoundaries(RenderTarget target)
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


    public abstract FloatRect GetBounds();
    
    public FloatRect ParentOrWindowBounds()
        => Parent?.GetBounds() ?? new(new(0, 0), (Vector2f)App.MainWindow!.Size);


    public virtual Vector2f GetAlignmentPosition(AlignmentType alignment)
        => AlignmentCalculator.GetAlignedPositionOfChild(GetBounds(), ParentOrWindowBounds(), alignment);
    
    
    public void Show()
        => Visible = true;
    
    public void Hide()
        => Visible = false;
}