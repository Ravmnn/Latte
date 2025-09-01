using System;
using System.Linq;
using System.Collections.Generic;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Application.Elements.Attributes;
using Latte.Application.Elements.Behavior;


namespace Latte.Application.Elements.Primitives;


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


public abstract class Element : BaseObject, IAlignable, ISizePoliciable, IMouseInputTarget
{
    private Element? _parent;
    private bool _active;


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

    public ElementAttributeManager Attributes { get; private set; }

    public bool Active
    {
        get => _active && Visible;
        set => _active = value;
    }

    protected bool ParentVisible => Parent?.VisibleAndParentVisible ?? true;

    public bool VisibleAndParentVisible => Visible && ParentVisible;

    public override bool CanDraw => base.CanDraw && VisibleAndParentVisible;

    public bool Clip { get; set; }
    public int ClipLayerIndex { get; protected set; }
    public int ClipLayerIndexOffset { get; set; }

    public PrioritySnap PrioritySnap { get; set; }
    public int PrioritySnapOffset { get; set; }

    public bool IgnoreMouseInput { get; set; }
    public bool CaughtMouseInput { get; set; }

    // TODO: there may be a better way of animating these... try to remove the property system
    public Vec2f RelativePosition
    {
        get => Parent is not null ? Parent.MapToRelative(Position) : AbsolutePosition;
        set => Position = Parent is not null ? Parent.MapToAbsolute(value) : value;
    }

    // TODO: changing this should modify relative position and vice versa
    public Vec2f AbsolutePosition
    {
        get => Position;
        set => Position = value;
    }

    public Alignment Alignment { get; set; }
    public Vec2f AlignmentMargin { get; set; }

    public SizePolicy SizePolicy { get; set; }
    public Vec2f SizePolicyMargin { get; set; }

    public event EventHandler<ElementEventArgs>? ParentChangedEvent;
    public event EventHandler<ElementEventArgs>? ChildAddedEvent;


    protected Element(Element? parent)
    {
        _active = true;


        Children = [];
        Parent = parent;

        Attributes = new ElementAttributeManager(this);

        Clip = true;

        PrioritySnap = PrioritySnap.None;
        PrioritySnapOffset = 1;

        RelativePosition = new Vec2f();

        Alignment = Alignment.None;
        AlignmentMargin = new Vec2f();

        SizePolicy = SizePolicy.None;
        SizePolicyMargin = new Vec2f();

        if (Parent is null)
            return;

        Parent.PriorityChangedEvent += (_, _) => AddParentPriorityDeltaToThis();
    }



    public override void ConstantUpdate()
    {
        RemoveNonChildren();

        Attributes.ProcessAttributes();

        UpdatePriority();
        UpdateClipLayerIndex();
        UpdateGeometry();

        base.ConstantUpdate();
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

    private void UpdateClipLayerIndex()
    {
        ClipLayerIndex = Clipping.GetClipLayerIndexOf(this);
    }

    private void UpdateGeometry()
    {
        if (SizePolicy != SizePolicy.None)
            ApplySizePolicy();

        if (Alignment != Alignment.None)
            ApplyAlignment();
    }


    public override void Draw(RenderTarget target)
    {
        BeginDraw(target);
        SimpleDraw(target);
        EndDraw();
    }


    public void SimpleDraw(RenderTarget target)
        => base.Draw(target);

    public abstract void BorderLessSimpleDraw(RenderTarget target);


    protected virtual void BeginDraw(RenderTarget target)
    {
        if (!Clip)
            return;

        Clipping.ClipEnable();

        if (Parent is null)
            return;

        Clipping.SetClipToParents(target, this);
        Clipping.Clip(ClipLayerIndex + ClipLayerIndexOffset);
    }

    protected virtual void EndDraw()
    {
        if (!Clip)
            return;

        Clipping.ClipDisable();
    }


    // The clip area is a rectangle. It represents the borderless bounds of
    // the parent of an element. It is not used to directly clip the element,
    // stencil buffer is used instead.

    public IntRect GetIntersectedClipArea() => Clipping.OverlapElementClipAreaToParents(this) ?? new IntRect();
    public IntRect GetClipArea() => Parent?.GetThisClipArea() ?? App.Window.WindowRect;
    public virtual IntRect GetThisClipArea() => GetBorderLessBounds().ToWindowCoordinates();

    public bool IsInsideClipArea()
        => GetBounds().Intersects((FloatRect)GetIntersectedClipArea());


    public bool IsPointOverBounds(Vec2f point)
        => IsPointOverClipArea(point) && point.IsPointOverRect(GetBounds());

    public bool IsPointOverClipArea(Vec2f point)
        => point.IsPointOverRect(GetIntersectedClipArea().ToWorldCoordinates());


    public abstract FloatRect GetBounds(); // TODO: move to BaseObject as IBounds
    public abstract FloatRect GetRelativeBounds();
    public virtual FloatRect GetBorderLessBounds() => GetBounds();
    public virtual FloatRect GetBorderLessRelativeBounds() => GetRelativeBounds();

    public FloatRect GetParentBounds() => Parent?.GetBounds() ?? (FloatRect)App.Window.WindowRect;
    public FloatRect GetParentRelativeBounds() => Parent?.GetRelativeBounds() ?? (FloatRect)App.Window.WindowRect;
    public FloatRect GetParentBorderLessBounds() => Parent?.GetBorderLessBounds() ?? (FloatRect)App.Window.WindowRect;
    public FloatRect GetParentBorderLessRelativeBounds() => Parent?.GetBorderLessRelativeBounds() ?? (FloatRect)App.Window.WindowRect;

    public FloatRect GetChildrenBounds()
        => (from child in Children select child.GetBounds()).GetBoundsOfRects();

    public FloatRect GetChildrenRelativeBounds()
        => (from child in Children select child.GetRelativeBounds()).GetBoundsOfRects();

    public FloatRect GetChildrenBorderLessBounds()
        => (from child in Children select child.GetBorderLessBounds()).GetBoundsOfRects();

    public FloatRect GetChildrenBorderLessRelativeBounds()
        => (from child in Children select child.GetBorderLessRelativeBounds()).GetBoundsOfRects();

    public virtual Vec2f GetAlignmentPosition(Alignment alignment)
        => AlignmentCalculator.GetAlignedPositionOfChild(GetBounds(), GetParentBorderLessBounds(), alignment);

    public virtual Vec2f GetAlignmentRelativePosition(Alignment alignment)
        => AlignmentCalculator.GetAlignedRelativePositionOfChild(GetRelativeBounds(), GetParentBorderLessRelativeBounds(), alignment);


    public Vec2f GetAlignmentPosition() => GetAlignmentPosition(Alignment);
    public Vec2f GetAlignmentRelativePosition() => GetAlignmentRelativePosition(Alignment);


    public virtual FloatRect GetSizePolicyRect(SizePolicy policyType)
        => SizePolicyCalculator.CalculateChildRect(GetBorderLessBounds(), GetParentBorderLessBounds(), policyType);

    public FloatRect GetSizePolicyRect()
        => GetSizePolicyRect(SizePolicy).ShrinkRect(SizePolicyMargin);


    public IEnumerable<Element> GetParents()
    {
        var parents = new List<Element>();
        var parent = Parent;

        if (parent is null)
            return parents;

        do
        {
            parents.Add(parent);
            parent = parent.Parent;
        }
        while (parent is not null);

        return parents;
    }


    protected void SetRelativePositionOrAlignment(Vec2f? position)
    {
        if (position is null)
            Alignment = Alignment.Center;
        else
            RelativePosition = position;
    }


    public virtual void ApplyAlignment()
        => RelativePosition = GetAlignmentRelativePosition() + AlignmentMargin;

    public abstract void ApplySizePolicy();


    public bool IsChildOf(Element parent)
    {
        if (parent == Parent)
            return true;

        return Parent is not null && Parent.IsChildOf(parent);
    }


    private bool ParentHierarchyHasPrioritySnap()
    {
        var result = PrioritySnap != PrioritySnap.None;

        foreach (var parent in GetParents())
            if (parent.PrioritySnap != PrioritySnap.None)
                result = true;

        return result;
    }


    public void RaiseToTop()
    {
        var element = App.Elements.LastOrDefault(element => element != this && !element.ParentHierarchyHasPrioritySnap());

        if (element is not null)
            Priority = element.Priority + PrioritySnapOffset;
    }

    public void LowerToBottom()
    {
        var element = App.Elements.FirstOrDefault(element => element != this && !element.ParentHierarchyHasPrioritySnap());

        if (element is not null)
            Priority = element.Priority - PrioritySnapOffset;
    }

    public void RaiseToParentTop()
    {
        var higherPriority = int.MinValue;

        Parent?.Children.ForeachElementRecursively(element =>
        {
            if (element != this && !element.ParentHierarchyHasPrioritySnap() && element.Priority > higherPriority)
                higherPriority = element.Priority;
        });

        Priority = higherPriority == int.MinValue ? Priority : higherPriority + PrioritySnapOffset;
    }

    public void LowerToParentBottom()
    {
        var lowerPriority = int.MaxValue;

        Parent?.Children.ForeachElementRecursively(element =>
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


    // TODO: all event callbacks should be public, since there are callbacks in interfaces and them can't be protected or private. Be consistent.

    protected virtual void OnParentChange()
    {
        Priority = Parent?.Priority + 1 ?? Priority;

        Parent?.OnChildAdded(this);
        ParentChangedEvent?.Invoke(this, new ElementEventArgs(Parent));
    }


    protected virtual void OnChildAdded(Element child)
    {
        Children.Add(child);
        ChildAddedEvent?.Invoke(this, new ElementEventArgs(child));
    }
}
