using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Core.Application;


namespace Latte.Elements.Primitives;


public abstract class ElementAttribute(bool inherit = false) : Attribute
{
    public bool Inherit { get; } = inherit;


    public virtual void Process(Element element) {}
}


[AttributeUsage(AttributeTargets.Class)]
public class ChildrenTypeAttribute(Type type) : ElementAttribute
{
    public Type Type { get; } = type;


    public override void Process(Element element)
    {
        foreach (Element child in element.Children)
            if (child.GetType() != Type)
                throw new InvalidOperationException($"The element \"{element.GetType().Name}\" can only have children of type: \"{Type.Name}\"");
    }
}


[AttributeUsage(AttributeTargets.Class)]
public class ChildrenAmountAttribute(uint amount) : ElementAttribute
{
    public uint Amount { get; } = amount;


    public override void Process(Element element)
    {
        if (element.Children.Count > Amount)
            throw new InvalidOperationException($"The element \"{element.GetType().Name}\" can only have {Amount} children.");
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


    public abstract Transformable Transformable { get; }

    public bool Initialized { get; private set; }

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

    public List<Element> Children { get; }

    public IEnumerable<Property> Properties => _properties;

    public ElementAttributeManager Attributes { get; private set; }

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

    protected bool ParentVisible => Parent?.Visible ?? true;

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

    public event EventHandler<ElementEventArgs>? ParentChangedEvent;
    public event EventHandler<ElementEventArgs>? ChildAddedEvent;
    public event EventHandler? VisibilityChangedEvent;
    public event EventHandler? PriorityChangedEvent;

    public event EventHandler? SetupEvent;
    public event EventHandler? ConstantUpdateEvent;
    public event EventHandler? UpdateEvent;
    public event EventHandler? DrawEvent;


    protected Element(Element? parent)
    {
        _properties = [];

        Children = [];
        Parent = parent;

        Attributes = new(this);

        Visible = true;

        PrioritySnap = PrioritySnap.None;
        PrioritySnapOffset = 1;

        RelativePosition = new(this, nameof(RelativePosition), new());
        Origin = new(this, nameof(Origin), new());
        Rotation = new(this, nameof(Rotation), 0f);
        Scale = new(this, nameof(Scale), new(1f, 1f));

        Alignment = new(this, nameof(Alignment), Elements.Alignment.None);
        AlignmentMargin = new(this, nameof(AlignmentMargin), new());

        SizePolicy = new(this, nameof(SizePolicy), SizePolicyType.None);
        SizePolicyMargin = new(this, nameof(SizePolicyMargin), new());

        if (Parent is null)
            return;

        Parent.PriorityChangedEvent += (_, _) => AddParentPriorityDeltaToThis();
    }



    // called once after object construction
    public virtual void Setup()
    {
        Initialized = true;

        SetupEvent?.Invoke(this, EventArgs.Empty);
    }



    // called once each frame, only if Visible is true
    public virtual void Update()
    {
        UpdateEvent?.Invoke(this, EventArgs.Empty);
    }


    // called at least one time each frame, independently of visibility. May be called
    // more than one time a frame
    public virtual void ConstantUpdate()
    {
        if (!Initialized)
            Setup();

        RemoveNonChildren();

        Attributes.ProcessAttributes();

        UpdatePriority();
        UpdateGeometry();
        UpdateSfmlProperties();

        LastPriority = Priority;

        ConstantUpdateEvent?.Invoke(this, EventArgs.Empty);
    }

    protected void RemoveNonChildren()
        => Children.RemoveAll(element => element.Parent != this);

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

    protected virtual void UpdateSfmlProperties()
    {
        Transformable.Position = AbsolutePosition;
        Transformable.Origin = Origin.Value;
        Transformable.Rotation = Rotation.Value;
        Transformable.Scale = Scale.Value;
    }



    // same as Update, but should be used to drawings
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
        => SizePolicyCalculator.CalculateChildRect(GetBorderLessBounds(), GetParentBorderLessBounds(), policyType);

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

        return Parent is not null && Parent.IsChildOf(parent);
    }



    public void Raise(uint amount = 1) => Priority += (int)amount;
    public void Lower(uint amount = 1) => Priority -= (int)amount;


    private bool ParentHierarchyHasPrioritySnap()
    {
        bool result = PrioritySnap != PrioritySnap.None;

        this.ForeachParent(element =>
        {
            if (element.PrioritySnap != PrioritySnap.None)
                result = true;
        });

        return result;
    }


    public void RaiseToTop()
    {
        Element? element = App.Elements.LastOrDefault(element => element != this && !element.ParentHierarchyHasPrioritySnap());

        if (element is not null)
            Priority = element.Priority + PrioritySnapOffset;
    }

    public void LowerToBottom()
    {
        Element? element = App.Elements.FirstOrDefault(element => element != this && !element.ParentHierarchyHasPrioritySnap());

        if (element is not null)
            Priority = element.Priority - PrioritySnapOffset;
    }

    public void RaiseToParentTop()
    {
        int higherPriority = int.MinValue;

        Parent?.Children.ForeachElement(element =>
        {
            if (element != this && !element.ParentHierarchyHasPrioritySnap() && element.Priority > higherPriority)
                higherPriority = element.Priority;
        });

        Priority = higherPriority == int.MinValue ? Priority : higherPriority + PrioritySnapOffset;
    }

    public void LowerToParentBottom()
    {
        int lowerPriority = int.MaxValue;

        Parent?.Children.ForeachElement(element =>
        {
            if (element != this && !element.ParentHierarchyHasPrioritySnap() && element.Priority < lowerPriority)
                lowerPriority = element.Priority;
        });

        Priority = lowerPriority == int.MaxValue ? Priority : lowerPriority - PrioritySnapOffset;
    }


    private void AddParentPriorityDeltaToThis()
    {
        if (Parent is not null)
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


    public IEnumerable<AnimatableProperty> GetAnimatableProperties()
        => from property in Properties
            where property is AnimatableProperty
            select property as AnimatableProperty;


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


    public static void ForeachParent(this Element element, Action<Element> action)
    {
        if (element.Parent is null)
            return;

        action(element.Parent);
        ForeachParent(element.Parent, action);
    }
}
