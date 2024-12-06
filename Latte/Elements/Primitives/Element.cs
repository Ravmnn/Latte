using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Core.Application;


using Color = SFML.Graphics.Color;
using Debug = Latte.Core.Debug;


namespace Latte.Elements.Primitives;


public class ElementEventArgs(Element? element) : EventArgs
{
    public Element? Element { get; } = element;
}


public abstract class Element : IUpdateable, IDrawable, IAlignable, ISizePoliciable
{
    private Element? _parent;
    private List<Property> _properties;
    
    private bool _visible;

    private int _priority;


    public Element? Parent
    {
        get => _parent;
        set
        {
            if (_parent == value)
                return;
            
            _parent = value;
            OnParentChange();
        }
    }

    public event EventHandler<ElementEventArgs>? ParentChangedEvent;
    
    public Property[] Properties => _properties.ToArray();
    
    public List<Element> Children { get; }
    public event EventHandler<ElementEventArgs>? ChildAddedEvent;

    public ElementKeyframeAnimator Animator { get; set; }
    
    public abstract Transformable Transformable { get; }

    protected bool ParentVisible => Parent?.Visible ?? true;
    
    public bool Visible
    {
        get => _visible && ParentVisible;
        set
        {
            if (_visible == value)
                return;
            
            _visible = value;
            OnVisibilityChange();
        }
    }

    public event EventHandler? VisibilityChangedEvent;
    
    public bool Initialized { get; private set; }

    public bool ShouldDrawElementBoundaries { get; set; }
    public bool ShouldDrawClipArea { get; set; }

    public bool CanDraw => Initialized && Visible && IsInsideClipArea();

    public int Priority
    {
        get => _priority;
        set
        {
            if (_priority == value)
                return;
            
            _priority = value;
            OnPriorityChange();
        }
    }
    
    protected int LastPriority { get; private set; }
    
    public event EventHandler? PriorityChangedEvent;

    public bool BlocksMouseInput { get; set; }
    
    public AnimatableProperty<Vec2f> RelativePosition { get; }
    public Vec2f AbsolutePosition
    {
        get => Parent is not null ? RelativePosition + Parent.AbsolutePosition : RelativePosition;
        set => RelativePosition.Set(Parent is not null ? value - Parent.AbsolutePosition : value);
    }
    
    public AnimatableProperty<Vec2f> Origin { get; }

    public AnimatableProperty<Float> Rotation { get; }
    
    public AnimatableProperty<Vec2f> Scale { get; }
    
    public Property<Alignments> Alignment { get; }
    public AnimatableProperty<Vec2f> AlignmentMargin { get; }
    
    public Property<SizePolicyType> SizePolicy { get; }

    public event EventHandler? SetupEvent; 
    public event EventHandler? UpdateEvent;
    public event EventHandler? DrawEvent;


    protected Element(Element? parent)
    {
        _properties = [];
        
        Parent = parent;
        Children = [];

        Animator = new(this, 0.07);

        Visible = true;

        BlocksMouseInput = true;
        
        RelativePosition = new(this, nameof(RelativePosition), new()) { CanAnimate = false };
        Origin = new(this, nameof(Origin), new()) { CanAnimate = false }; 
        Rotation = new(this, nameof(Rotation), 0f);
        Scale = new(this, nameof(Scale), new(1f, 1f));
        
        Alignment = new(this, nameof(Alignment), Alignments.None);
        AlignmentMargin = new(this, nameof(AlignmentMargin), new()) { CanAnimate = false };

        SizePolicy = new(this, nameof(SizePolicy), SizePolicyType.None);
        
        if (Parent is null)
            return;

        Parent.PriorityChangedEvent += (_, _) => AddParentPriorityDeltaToThis();
    }


    protected virtual void Setup()
    {
        Animator.DefaultProperties = ToKeyframe();

        UpdateSfmlProperties();
        
        Initialized = true;
        
        SetupEvent?.Invoke(this, EventArgs.Empty);
    }


    public virtual void Update()
    {
        CanOnlyHaveChildOfTypeAttribute.Check(this);
        
        RemoveNonChildren();
        
        if (!Initialized)
            Setup();
        
        if (SizePolicy.Value != SizePolicyType.None)
            ApplySizePolicy();
        
        if (Alignment.Value != Alignments.None)
            ApplyAlignment();
        
        UpdatePropertyAnimations();
        UpdateSfmlProperties();

        LastPriority = Priority;
        
        UpdateEvent?.Invoke(this, EventArgs.Empty);
    }
    
    private void UpdatePropertyAnimations()
    {
        foreach (Property property in Properties)
            if (property is AnimatableProperty { Animation: not null } animatableProperty)
                animatableProperty.Animation.Update();
    }
    
    protected virtual void UpdateSfmlProperties()
    {
        Transformable.Position = AbsolutePosition;
        Transformable.Origin = Origin.Value;
        Transformable.Rotation = Rotation.Value;
        Transformable.Scale = Scale.Value;
    }

    protected void RemoveNonChildren()
        => Children.RemoveAll(element => element.Parent != this);
    

    
    public virtual void Draw(RenderTarget target)
    {
        DrawEvent?.Invoke(this, EventArgs.Empty);
    }

    public virtual void DrawDebug(RenderTarget target)
    {
        if (ShouldDrawElementBoundaries)
            Debug.DrawLineRect(target, GetBounds(), Color.Red);
        
        if (ShouldDrawClipArea)
            Debug.DrawLineRect(target, (FloatRect)GetClipArea(), Color.Magenta);
    }

    protected virtual void BeginDraw() => ClipArea.BeginClip(GetFinalClipArea());
    protected virtual void EndDraw() => ClipArea.EndClip();


    public IntRect GetFinalClipArea() => ClipArea.OverlapElementClipAreaToParents(this) ?? new();
    public IntRect GetClipArea() => Parent?.GetThisClipArea() ?? App.Window.WindowRect;
    public virtual IntRect GetThisClipArea() => GetBounds().ToWindowCoordinates();

    public bool IsInsideClipArea()
        => GetBounds().Intersects((FloatRect)GetFinalClipArea());


    public bool IsPointOverBounds(Vec2f point)
        => IsPointOverClipArea(point) && point.IsPointOverRect(GetBounds());
    
    public bool IsPointOverClipArea(Vec2f point)
        => point.IsPointOverRect(GetFinalClipArea().ToWorldCoordinates());
    
    
    public abstract FloatRect GetBounds();
    public FloatRect GetParentBounds() => Parent?.GetBounds() ?? (FloatRect)App.Window.WindowRect;


    public virtual Vec2f GetAlignmentPosition(Alignments alignment)
        => AlignmentCalculator.GetAlignedPositionOfChild(GetBounds(), GetParentBounds(), alignment);

    public Vec2f GetAlignmentPosition() => GetAlignmentPosition(Alignment);

    public virtual FloatRect GetSizePolicyRect(SizePolicyType policyType)
        => SizePolicyCalculator.CalculateChildRect(GetBounds(), GetParentBounds(), policyType);
    
    public FloatRect GetSizePolicyRect() => GetSizePolicyRect(SizePolicy);
    
    
    public virtual void ApplyAlignment()
        => AbsolutePosition = GetAlignmentPosition(Alignment) + AlignmentMargin;

    public abstract void ApplySizePolicy();
    
    
    public virtual void Show() => Visible = true;
    public virtual void Hide() => Visible = false;

    protected virtual void OnVisibilityChange()
        => VisibilityChangedEvent?.Invoke(this, EventArgs.Empty);


    public bool IsChildOf(Element parent)
    {
        Element? currentParent = Parent;

        while (currentParent is not null)
        {
            if (currentParent == parent)
                return true;
             
            currentParent = currentParent.Parent;
        }

        return false;
    }


    public void Raise() => Priority++;
    public void Lower() => Priority--;

    public void FullRaise() => Priority = App.Elements.Last().Priority + 1;
    public void FullLower() => Priority = App.Elements.First().Priority - 1;


    private void AddParentPriorityDeltaToThis()
    {
        if (Parent is null)
            return;

        Priority += Parent.Priority - Parent.LastPriority;
    }
    
    
    public void AddProperty(Property property) => _properties.Add(property);
    public bool RemoveProperty(Property property) => _properties.Remove(property);
    public bool HasProperty(Property property) => _properties.Contains(property);
    public Property GetProperty(string name)
        => _properties.Find(property => property.Name == name)
            ?? throw new ArgumentException($"Could not find property with name \"{name}\".");
    
    public bool TryGetProperty(string name, [MaybeNullWhen(false)] out Property property)
    {
        try
        {
            property = GetProperty(name);
            return true;
        }
        catch
        {
            property = null;
            return false;
        }
    }
    

    public Property this[string name] => GetProperty(name);
    
    
    public AnimatableProperty[] GetAnimatableProperties()
        => (from property in Properties where property is AnimatableProperty select property as AnimatableProperty).ToArray();


    public Keyframe ToKeyframe() => new(GetAnimatableProperties());


    protected virtual void OnParentChange()
    {
        Priority = Parent?.Priority + 1 ?? Priority;
        
        Parent?.OnChildAdded(this);
        ParentChangedEvent?.Invoke(this, new(Parent));
    }
    
    
    protected virtual void OnChildAdded(Element child)
    {
        Children.Add(child);
        ChildAddedEvent?.Invoke(this, new(child));
    }


    protected virtual void OnPriorityChange()
        => PriorityChangedEvent?.Invoke(this, EventArgs.Empty);
}