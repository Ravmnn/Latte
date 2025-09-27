using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Objects;
using Latte.Core.Type;
using Latte.Rendering;
using Latte.Application;


namespace Latte.UI.Elements;




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
    public bool IgnoreMouseInput { get; set; }
    public bool CaughtMouseInput { get; set; }




    public override bool CanUpdate => Active;
    public override bool CanDraw => base.CanDraw && Visible;




    private Element? _parent;
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




    public List<Element> Children { get; }

    public event EventHandler<ElementEventArgs>? ChildAddedEvent;




    private bool _active;
    public bool Active
    {
        get => _active && Visible;
        set => _active = value;
    }


    public new bool Visible
    {
        get => base.Visible && ParentVisible;
        set => base.Visible = value;
    }

    protected bool ParentVisible => Parent?.Visible ?? true;


    public bool Clip { get; set; }
    public int ClipLayerIndex { get; protected set; }
    public int ClipLayerIndexOffset { get; set; }


    public PrioritySnap PrioritySnap { get; set; }
    public int PrioritySnapOffset { get; set; }




    public Vec2f RelativePosition { get; set; }

    public Vec2f AbsolutePosition
    {
        get => Position;
        set
        {
            RelativePosition = MapToParentRelative(value);
            Position = value;
        }
    }

    public Alignment Alignment { get; set; }
    public Vec2f AlignmentMargin { get; set; }

    public SizePolicy SizePolicy { get; set; }
    public Vec2f SizePolicyMargin { get; set; }




    protected Element(Element? parent)
    {
        _active = true;


        Children = [];
        Parent = parent;

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




    // TODO: change name
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

        AbsolutePosition = MapToParentAbsolute(RelativePosition);
    }




    public override void Draw(IRenderer renderer)
    {
        BeginDraw(renderer);
        SimpleDraw(renderer);
        EndDraw();
    }


    public void SimpleDraw(IRenderer renderer)
        => base.Draw(renderer);


    public abstract void BorderLessSimpleDraw(IRenderer renderer);


    protected virtual void BeginDraw(IRenderer renderer)
    {
        if (!Clip)
            return;

        Clipping.ClipEnable();

        if (Parent is null)
            return;

        Clipping.SetClipToParents(renderer, this);
        Clipping.Clip(ClipLayerIndex + ClipLayerIndexOffset);
    }


    protected virtual void EndDraw()
    {
        if (!Clip)
            return;

        Clipping.ClipDisable();
    }




    public Vec2f MapToParentAbsolute(Vec2f point)
        => Parent is not null ? Parent.MapToAbsolute(point) : point;

    public Vec2f MapToParentRelative(Vec2f point)
        => Parent is not null ? Parent.MapToRelative(point) : point;


    // The clip area is a rectangle. It represents the borderless bounds of
    // the parent of an element. It is not used to directly clip the element,
    // stencil buffer is used instead.


    public IntRect GetIntersectedClipArea() => Clipping.OverlapElementClipAreaToParents(this) ?? new IntRect();
    public IntRect GetClipArea() => Parent?.GetThisClipArea() ?? App.Window.WindowRect;
    public virtual IntRect GetThisClipArea() => GetBorderLessBounds().ToWindowCoordinates();

    public bool IsInsideClipArea()
        => GetBounds().Intersects((FloatRect)GetIntersectedClipArea());




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




    public virtual void ApplyAlignment()
        => RelativePosition = GetAlignmentRelativePosition() + AlignmentMargin;


    public virtual Vec2f GetAlignmentPosition(Alignment alignment)
        => AlignmentCalculator.GetAlignedPositionOfChild(GetBounds(), GetParentBorderLessBounds(), alignment);

    public virtual Vec2f GetAlignmentRelativePosition(Alignment alignment)
        => AlignmentCalculator.GetAlignedRelativePositionOfChild(GetRelativeBounds(), GetParentBorderLessRelativeBounds(), alignment);


    public Vec2f GetAlignmentPosition() => GetAlignmentPosition(Alignment);
    public Vec2f GetAlignmentRelativePosition() => GetAlignmentRelativePosition(Alignment);





    public abstract void ApplySizePolicy();


    public virtual FloatRect GetSizePolicyRect(SizePolicy policyType)
        => SizePolicyCalculator.CalculateChildRect(GetBorderLessBounds(), GetParentBorderLessBounds(), policyType);


    public FloatRect GetSizePolicyRect()
        => GetSizePolicyRect(SizePolicy).ShrinkRect(SizePolicyMargin);




    protected void SetRelativePositionOrAlignment(Vec2f? position)
    {
        if (position is null)
            Alignment = Alignment.Center;
        else
            RelativePosition = position;
    }




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
        var elements = App.Section.GetObjectsOfType<Element>();
        var element = elements.LastOrDefault(element => element != this && !element.ParentHierarchyHasPrioritySnap());

        if (element is not null)
            Priority = element.Priority + PrioritySnapOffset;
    }


    public void LowerToBottom()
    {
        var elements = App.Section.GetObjectsOfType<Element>();
        var element = elements.FirstOrDefault(element => element != this && !element.ParentHierarchyHasPrioritySnap());

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
