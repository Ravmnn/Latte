using System;
using System.Collections.Generic;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Core.Animation;
using Latte.Core.Application;


namespace Latte.Elements.Primitives;


public abstract class Element : IUpdateable, IDrawable, IAlignable
{
    public Element? Parent { get; set; }
    public List<Element> Children { get; }
    
    protected List<AnimationState> PropertyAnimations { get; }
    
    public abstract Transformable Transformable { get; }
    
    public bool Visible { get; set; }
    public bool ShouldDrawElementBoundaries { get; set; }
    public bool ShouldDrawClipArea { get; set; }
    
    public Property<Vec2f> Position { get; }
    public Vec2f AbsolutePosition
    {
        get => Parent is not null ? Position + Parent.AbsolutePosition : Position;
        set => Position.Set(Parent is not null ? value - Parent.AbsolutePosition : value);
    }
    
    public Property<Vec2f> Origin { get; }

    public Property<Float> Rotation { get; }
    
    public AlignmentType? Alignment { get; set; }
    public Property<Vec2f> AlignmentMargin { get; }

    public event EventHandler? UpdateEvent;
    public event EventHandler? DrawEvent;


    protected Element(Element? parent)
    {
        Parent = parent;
        Children = [];

        PropertyAnimations = [];
     
        Visible = true;
        
        Position = new(this, new());
        Origin = new(this, new());
        Rotation = new(this, 0f);
        AlignmentMargin = new(this, new());
        
        Parent?.Children.Add(this);
    }


    public virtual void Update()
    {
        if (!Visible)
            return;
        
        if (Alignment is not null)
            AbsolutePosition = GetAlignmentPosition(Alignment.Value) + AlignmentMargin;
     
        UpdateSfmlProperties();

        foreach (AnimationState animation in PropertyAnimations)
            animation.Update();
        
        UpdateEvent?.Invoke(this, EventArgs.Empty);
        
        foreach (Element child in Children)
            child.Update();
    }
    
    
    protected virtual void UpdateSfmlProperties()
    {
        Transformable.Position = AbsolutePosition;
        Transformable.Origin = Origin.Value;
        Transformable.Rotation = Rotation.Value;
    }

    
    public virtual void Draw(RenderTarget target)
    {
        if (ShouldDrawElementBoundaries)
            Debug.DrawLineRect(target, GetBounds(), Color.Red);
        
        if (ShouldDrawClipArea)
            Debug.DrawLineRect(target, (FloatRect)GetClipArea(), Color.Magenta);
        
        if (!Visible)
            return;
        
        DrawEvent?.Invoke(this, EventArgs.Empty);
        
        foreach (Element child in Children)
            child.Draw(target);
    }

    protected virtual void BeginDraw()
    {
        ClipArea.BeginClip(GetFinalClipArea());
    }

    protected virtual void EndDraw()
    {
        ClipArea.EndClip();
    }

    
    public IntRect GetFinalClipArea() => ClipArea.OverlapElementsClipArea(this);
    public IntRect GetClipArea() => Parent?.GetThisClipArea() ?? App.MainWindow.RectSize;
    public virtual IntRect GetThisClipArea() => ClipArea.WorldFloatRectToClipArea(GetBounds());


    public abstract FloatRect GetBounds();
    public FloatRect ParentBounds() => Parent?.GetBounds() ?? (FloatRect)App.MainWindow.RectSize;


    public virtual Vec2f GetAlignmentPosition(AlignmentType alignment)
        => AlignmentCalculator.GetAlignedPositionOfChild(GetBounds(), ParentBounds(), alignment);
    
    
    public void Show() => Visible = true;
    public void Hide() => Visible = false;


    public void AddPropertyAnimation(AnimationState animation)
    {
        if (!PropertyAnimations.Contains(animation))
            PropertyAnimations.Add(animation);
    }
}