using System;
using System.Linq;
using System.Collections.Generic;

using SFML.Graphics;

using Latte.Sfml;
using Latte.Core;
using Latte.Core.Type;
using Latte.Core.Application;


namespace Latte.Elements.Primitives;


public abstract class Element : IUpdateable, IDrawable, IAlignable
{
    public Element? Parent { get; set; }
    public List<Element> Children { get; }
    
    public List<Property> Properties { get; }

    public ElementKeyframeAnimator Animator { get; set; }
    
    public abstract Transformable Transformable { get; }
    
    public bool Visible { get; set; }
    public bool ShouldDrawElementBoundaries { get; set; }
    public bool ShouldDrawClipArea { get; set; }
    
    private bool _initialized;
    
    public AnimatableProperty<Vec2f> Position { get; }
    public Vec2f AbsolutePosition
    {
        get => Parent is not null ? Position + Parent.AbsolutePosition : Position;
        set => Position.Set(Parent is not null ? value - Parent.AbsolutePosition : value);
    }
    
    public AnimatableProperty<Vec2f> Origin { get; }

    public AnimatableProperty<Float> Rotation { get; }
    
    public AnimatableProperty<Vec2f> Scale { get; }
    
    public Property<AlignmentType> Alignment { get; set; }
    public AnimatableProperty<Vec2f> AlignmentMargin { get; }

    public event EventHandler? SetupEvent; 
    public event EventHandler? UpdateEvent;
    public event EventHandler? DrawEvent;


    protected Element(Element? parent)
    {
        Parent = parent;
        Children = [];

        Properties = [];

        Animator = new(this, 0.1);
     
        Visible = true;

        _initialized = false;
        
        Position = new(this, nameof(Position), new()) { ShouldAnimatorIgnore = true };
        Origin = new(this, nameof(Origin), new()) { ShouldAnimatorIgnore = true }; 
        Rotation = new(this, nameof(Rotation), 0f);
        Scale = new(this, nameof(Scale), new(1f, 1f));
        
        Alignment = new(this, nameof(Alignment), AlignmentType.None);
        AlignmentMargin = new(this, nameof(AlignmentMargin), new()) { ShouldAnimatorIgnore = true };
        
        Parent?.Children.Add(this);
    }


    protected virtual void Setup()
    {
        Animator.DefaultProperties = ToKeyframe();

        _initialized = true;
        
        SetupEvent?.Invoke(this, EventArgs.Empty);
    }


    public virtual void Update()
    {
        if (!_initialized)
            Setup();
        
        if (!Visible)
            return;
        
        if (Alignment != AlignmentType.None)
            AbsolutePosition = GetAlignmentPosition(Alignment.Value) + AlignmentMargin;
     
        UpdatePropertyAnimations();
        UpdateSfmlProperties();
        
        UpdateEvent?.Invoke(this, EventArgs.Empty);
        
        foreach (Element child in Children)
            child.Update();
    }
    
    
    protected virtual void UpdateSfmlProperties()
    {
        Transformable.Position = AbsolutePosition;
        Transformable.Origin = Origin.Value;
        Transformable.Rotation = Rotation.Value;
        Transformable.Scale = Scale.Value;
    }


    protected void UpdatePropertyAnimations()
    {
        foreach (Property property in Properties)
            if (property is AnimatableProperty { AnimationState: not null } animatableProperty)
                animatableProperty.AnimationState.Update();
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
    public virtual IntRect GetThisClipArea() => GetBounds().ToWindowCoordinates();


    public abstract FloatRect GetBounds();
    public FloatRect ParentBounds() => Parent?.GetBounds() ?? (FloatRect)App.MainWindow.RectSize;


    public virtual Vec2f GetAlignmentPosition(AlignmentType alignment)
        => AlignmentCalculator.GetAlignedPositionOfChild(GetBounds(), ParentBounds(), alignment);
    
    
    public void Show() => Visible = true;
    public void Hide() => Visible = false;
    
    
    public AnimatableProperty[] GetAnimatableProperties()
        => (from property in Properties where property is AnimatableProperty select property as AnimatableProperty).ToArray();


    public Keyframe ToKeyframe()
        => new(GetAnimatableProperties());
}