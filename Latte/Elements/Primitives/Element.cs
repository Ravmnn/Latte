using System;
using System.Linq;
using System.Collections.Generic;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Core.Application;


namespace Latte.Elements.Primitives;


public abstract class Element : IUpdateable, IDrawable, IAlignable
{
    private bool _initialized;

    private bool _visible;
    
    
    public Element? Parent { get; set; }
    public List<Element> Children { get; }
    
    public List<Property> Properties { get; }

    public ElementKeyframeAnimator Animator { get; set; }
    
    public abstract Transformable Transformable { get; }

    public bool Visible
    {
        get => _visible;
        set
        {
            _visible = value;
            VisibilityChangeEvent?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public event EventHandler? VisibilityChangeEvent;
    
    public bool ShouldDrawElementBoundaries { get; set; }
    public bool ShouldDrawClipArea { get; set; }
    
    public AnimatableProperty<Vec2f> RelativePosition { get; }
    public Vec2f AbsolutePosition
    {
        get => Parent is not null ? RelativePosition + Parent.AbsolutePosition : RelativePosition;
        set => RelativePosition.Set(Parent is not null ? value - Parent.AbsolutePosition : value);
    }
    
    public AnimatableProperty<Vec2f> Origin { get; }

    public AnimatableProperty<Float> Rotation { get; }
    
    public AnimatableProperty<Vec2f> Scale { get; }
    
    public Property<Alignments> Alignment { get; set; }
    public AnimatableProperty<Vec2f> AlignmentMargin { get; }

    public event EventHandler? SetupEvent; 
    public event EventHandler? UpdateEvent;
    public event EventHandler? DrawEvent;


    protected Element(Element? parent)
    {
        _initialized = false;

        Parent = parent;
        Children = [];

        Properties = [];

        Animator = new(this, 0.1);
     
        _visible = true;

        VisibilityChangeEvent += (_, _) => OnVisibilityChange();
        
        RelativePosition = new(this, nameof(RelativePosition), new()) { CanAnimate = false };
        Origin = new(this, nameof(Origin), new()) { CanAnimate = false }; 
        Rotation = new(this, nameof(Rotation), 0f);
        Scale = new(this, nameof(Scale), new(1f, 1f));
        
        Alignment = new(this, nameof(Alignment), Alignments.None);
        AlignmentMargin = new(this, nameof(AlignmentMargin), new()) { CanAnimate = false };
        
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
        
        if (Alignment != Alignments.None)
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


    private void UpdatePropertyAnimations()
    {
        foreach (Property property in Properties)
            if (property is AnimatableProperty { Animation: not null } animatableProperty)
                animatableProperty.Animation.Update();
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

    protected virtual void BeginDraw() => ClipArea.BeginClip(GetFinalClipArea());
    protected virtual void EndDraw() => ClipArea.EndClip();


    public IntRect GetFinalClipArea() => ClipArea.OverlapElementClipAreaToParents(this);
    public IntRect GetClipArea() => Parent?.GetThisClipArea() ?? App.Window.WindowRect;
    public virtual IntRect GetThisClipArea() => GetBounds().ToWindowCoordinates();


    public abstract FloatRect GetBounds();
    public FloatRect GetParentBounds() => Parent?.GetBounds() ?? (FloatRect)App.Window.WindowRect;


    public virtual Vec2f GetAlignmentPosition(Alignments alignment)
        => AlignmentCalculator.GetAlignedPositionOfChild(GetBounds(), GetParentBounds(), alignment);
    
    
    public virtual void Show() => Visible = true;
    public virtual void Hide() => Visible = false;
    
    protected virtual void OnVisibilityChange() {}
    
    
    public AnimatableProperty[] GetAnimatableProperties()
        => (from property in Properties where property is AnimatableProperty select property as AnimatableProperty).ToArray();


    public Keyframe ToKeyframe() => new(GetAnimatableProperties());
}