using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Core.Application;


namespace Latte.Elements.Primitives;


[AttributeUsage(AttributeTargets.Class)]
public class CanOnlyHaveChildOfTypeAttribute(Type type) : Attribute
{
    public Type Type { get; } = type;


    public static void Check(Element element)
    {
        if (element.GetAttribute<CanOnlyHaveChildOfTypeAttribute>() is not { } attribute)
            return;

        foreach (Element child in element.Children)
            if (child.GetType() != attribute.Type)
                throw new InvalidOperationException($"The element \"{element.GetType().Name}\" can only have children of type: \"{attribute.Type.Name}\"");
    }
}


[AttributeUsage(AttributeTargets.Class)]
public class ChildrenAmountLimitAttribute(uint amount) : Attribute
{
    public uint Amount { get; } = amount;


    public static void Check(Element element)
    {
        if (element.GetAttribute<ChildrenAmountLimitAttribute>() is not { } attribute)
            return;

        if (element.Children.Count > attribute.Amount)
            throw new InvalidOperationException($"The element \"{element.GetType().Name}\" can only have {attribute.Amount} children.");
    }
}


public class ElementEventArgs(Element? element) : EventArgs
{
    public Element? Element { get; } = element;
}


public enum PrioritySnap
{
    None,

    AlwaysOnTop,
    AlwaysOnBottom,
    AlwaysOnParentTop,
    AlwaysOnParentBottom
}


public abstract class Element : IUpdateable, IDrawable, IAlignable, ISizePoliciable, IMouseInputTarget
{
    private Element? _parent;
    private readonly List<Property> _properties;

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

    public PrioritySnap PrioritySnap { get; set; }
    public int PrioritySnapOffset { get; set; }

    public event EventHandler? PriorityChangedEvent;

    public bool IgnoreMouseInput { get; set; }
    public bool CaughtMouseInput { get; set; }

    public AnimatableProperty<Vec2f> RelativePosition { get; }
    public Vec2f AbsolutePosition
    {
        get => Parent is not null ? RelativePosition + Parent.AbsolutePosition : RelativePosition;
        set => RelativePosition.Set(Parent is not null ? value - Parent.AbsolutePosition : value);
    }

    public AnimatableProperty<Vec2f> Origin { get; }

    public AnimatableProperty<Float> Rotation { get; }

    public AnimatableProperty<Vec2f> Scale { get; }

    public Property<Alignment> Alignment { get; }
    public AnimatableProperty<Vec2f> AlignmentMargin { get; }

    public Property<SizePolicyType> SizePolicy { get; }
    public AnimatableProperty<Vec2f> SizePolicyMargin { get; }

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

        PrioritySnap = PrioritySnap.None;
        PrioritySnapOffset = 1;

        RelativePosition = new(this, nameof(RelativePosition), new()) { CanAnimate = false };
        Origin = new(this, nameof(Origin), new()) { CanAnimate = false };
        Rotation = new(this, nameof(Rotation), 0f);
        Scale = new(this, nameof(Scale), new(1f, 1f));

        Alignment = new(this, nameof(Alignment), Elements.Alignment.None);
        AlignmentMargin = new(this, nameof(AlignmentMargin), new()) { CanAnimate = false };

        SizePolicy = new(this, nameof(SizePolicy), SizePolicyType.None);
        SizePolicyMargin = new(this, nameof(SizePolicyMargin), new()) { CanAnimate = false };

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
        RemoveNonChildren();

        CanOnlyHaveChildOfTypeAttribute.Check(this);
        ChildrenAmountLimitAttribute.Check(this);

        if (!Initialized)
            Setup();

        UpdatePriority();
        UpdateGeometry();

        UpdatePropertyAnimations();
        UpdateSfmlProperties();

        LastPriority = Priority;

        UpdateEvent?.Invoke(this, EventArgs.Empty);
    }

    private void UpdatePriority()
    {
        switch (PrioritySnap)
        {
            case PrioritySnap.AlwaysOnTop:
                RaiseToTop();
                break;

            case PrioritySnap.AlwaysOnBottom:
                LowerToBottom();
                break;

            case PrioritySnap.AlwaysOnParentTop:
                RaiseToParentTop();
                break;

            case PrioritySnap.AlwaysOnParentBottom:
                LowerToParentBottom();
                break;
        }
    }

    private void UpdateGeometry()
    {
        if (SizePolicy.Value != SizePolicyType.None)
            ApplySizePolicy();

        if (Alignment.Value != Elements.Alignment.None)
            ApplyAlignment();
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

    protected virtual void BeginDraw() => ClipArea.BeginClip(GetFinalClipArea());
    protected virtual void EndDraw() => ClipArea.EndClip();


    public IntRect GetFinalClipArea() => ClipArea.OverlapElementClipAreaToParents(this) ?? new();
    public IntRect GetClipArea() => Parent?.GetThisClipArea() ?? App.Window.WindowRect;
    public virtual IntRect GetThisClipArea() => GetBorderLessBounds().ToWindowCoordinates();

    public bool IsInsideClipArea()
        => GetBounds().Intersects((FloatRect)GetFinalClipArea());


    public bool IsPointOverBounds(Vec2f point)
        => IsPointOverClipArea(point) && point.IsPointOverRect(GetBounds());

    public bool IsPointOverClipArea(Vec2f point)
        => point.IsPointOverRect(GetFinalClipArea().ToWorldCoordinates());


    public abstract FloatRect GetBounds();
    public abstract FloatRect GetRelativeBounds();
    public virtual FloatRect GetBorderLessBounds() => GetBounds();
    public virtual FloatRect GetBorderLessRelativeBounds() => GetRelativeBounds();

    public FloatRect GetParentBounds() => Parent?.GetBounds() ?? (FloatRect)App.Window.WindowRect;
    public FloatRect GetParentRelativeBounds() => Parent?.GetRelativeBounds() ?? (FloatRect)App.Window.WindowRect;
    public FloatRect GetParentBorderLessBounds() => Parent?.GetBorderLessBounds() ?? (FloatRect)App.Window.WindowRect;
    public FloatRect GetParentBorderLessRelativeBounds() => Parent?.GetBorderLessRelativeBounds() ?? (FloatRect)App.Window.WindowRect;


    public virtual Vec2f GetAlignmentPosition(Alignment alignment)
        => AlignmentCalculator.GetAlignedPositionOfChild(GetBorderLessBounds(), GetParentBorderLessBounds(), alignment);

    public virtual Vec2f GetAlignmentRelativePosition(Alignment alignment)
        => AlignmentCalculator.GetAlignedRelativePositionOfChild(GetBorderLessRelativeBounds(), GetParentBorderLessRelativeBounds(), alignment);

    public Vec2f GetAlignmentPosition() => GetAlignmentPosition(Alignment);
    public Vec2f GetAlignmentRelativePosition() => GetAlignmentRelativePosition(Alignment);

    public virtual FloatRect GetSizePolicyRect(SizePolicyType policyType)
        => SizePolicyCalculator.CalculateChildRect(GetBounds(), GetParentBounds(), policyType);

    public FloatRect GetSizePolicyRect()
        => GetSizePolicyRect(SizePolicy).ShrinkRect(SizePolicyMargin);


    public virtual void ApplyAlignment()
        => RelativePosition.Set(GetAlignmentRelativePosition() + AlignmentMargin);

    public abstract void ApplySizePolicy();


    public virtual void Show() => Visible = true;
    public virtual void Hide() => Visible = false;

    protected virtual void OnVisibilityChange()
        => VisibilityChangedEvent?.Invoke(this, EventArgs.Empty);


    public bool IsChildOf(Element parent)
    {
        if (parent == Parent)
            return true;

        return parent.Parent is not null && IsChildOf(parent.Parent);
    }



    public void Raise(uint amount = 1) => Priority += (int)amount;
    public void Lower(uint amount = 1) => Priority -= (int)amount;

    public void RaiseToTop()
    {
        Element element = App.Elements.Last(element => element != this && element.PrioritySnap == PrioritySnap.None);
        Priority = element.Priority + PrioritySnapOffset;
    }

    public void LowerToBottom()
    {
        Element element = App.Elements.First(element => element != this && element.PrioritySnap == PrioritySnap.None);
        Priority = element.Priority - PrioritySnapOffset;
    }

    public void RaiseToParentTop()
    {
        int higherPriority = int.MinValue;

        Parent?.Children.ForeachElement(element =>
        {
            if (element != this && element.PrioritySnap == PrioritySnap.None && element.Priority > higherPriority)
                higherPriority = element.Priority;
        });

        Priority = higherPriority == int.MinValue ? Priority : higherPriority + PrioritySnapOffset;
    }

    public void LowerToParentBottom()
    {
        int lowerPriority = int.MaxValue;

        Parent?.Children.ForeachElement(element =>
        {
            if (element != this && element.PrioritySnap == PrioritySnap.None && element.Priority < lowerPriority)
                lowerPriority = element.Priority;
        });

        Priority = lowerPriority == int.MaxValue ? Priority : lowerPriority - PrioritySnapOffset;
    }


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


public static class ElementExtensions
{
    public static void ForeachElement(this IEnumerable<Element> elements, Action<Element> action)
    {
        foreach (Element element in elements)
        {
            action(element);
            ForeachElement(element.Children, action);
        }
    }
}
