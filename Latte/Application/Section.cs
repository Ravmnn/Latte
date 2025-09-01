using System;
using System.Linq;
using System.Collections.Generic;

using SFML.Graphics;

using Latte.Core;
using Latte.Application.Elements.Behavior;
using Latte.Application.Elements.Primitives;


namespace Latte.Application;


public class Section : IUpdateable, IDrawable
{
    // Elements are ordered based on their priority
    private List<Element> _elements;


    public IEnumerable<Element> Elements => _elements;

    public event EventHandler<ElementEventArgs>? ElementAddedEvent;
    public event EventHandler<ElementEventArgs>? ElementRemovedEvent;
    public event EventHandler<ElementEventArgs>? ElementListModifiedEvent;

    public event EventHandler? UpdateEvent;
    public event EventHandler? DrawEvent;


    public Section()
    {
        _elements = [];

        ElementAddedEvent += (_, args) => ElementListModifiedEvent?.Invoke(this, args);
        ElementRemovedEvent += (_, args) => ElementListModifiedEvent?.Invoke(this, args);
    }


    public virtual void Initialize() { }
    public virtual void Deinitialize() { }

    public virtual void Update()
    {
        SortElementListByPriority();

        UpdateEvent?.Invoke(this, EventArgs.Empty);
    }

    public virtual void Draw(RenderTarget target)
        => DrawEvent?.Invoke(this, EventArgs.Empty);


    private void SortElementListByPriority()
        => _elements = _elements.OrderBy(element => element.Priority).ToList();


    public void AddElement(Element element)
    {
        AddSingleElement(element);
        AddElementsHierarchy(element.Children);
    }

    private void AddElementsHierarchy(IEnumerable<Element> elements)
        => elements.ForeachElementRecursively(AddSingleElement);

    private void AddSingleElement(Element element)
    {
        if (HasElement(element))
            return;

        _elements.Add(element);

        AddEventListenersTo(element);

        ElementAddedEvent?.Invoke(null, new ElementEventArgs(element));
    }


    public bool RemoveElement(Element element)
    {
        if (!RemoveSingleElement(element))
            return false;

        RemoveElementsChildrenOf(element);
        return true;
    }

    private void RemoveElementsChildrenOf(Element parent)
    {
        foreach (var element in _elements.ToArray().Reverse())
            if (element.IsChildOf(parent))
                RemoveSingleElement(element);
    }

    private bool RemoveSingleElement(Element element)
    {
        var result = _elements.Remove(element);

        RemoveEventListenersOf(element);

        ElementRemovedEvent?.Invoke(null, new ElementEventArgs(element));

        return result;
    }


    public bool HasElement(Element element)
        => _elements.Contains(element);



    public void AddEventListenersTo(Element element)
    {
        AddChildAddedListenerTo(element);

        if (element is IFocusable focusable)
            FocusManager.AddFocusListenerTo(focusable);
    }

    public void RemoveEventListenersOf(Element element)
    {
        RemoveChildAddedListenerOf(element);

        if (element is IFocusable focusable)
            FocusManager.RemoveFocusListenerOf(focusable);
    }


    private void AddChildAddedListenerTo(Element element)
        => element.ChildAddedEvent += OnElementChildAdded;

    private void RemoveChildAddedListenerOf(Element element)
        => element.ChildAddedEvent -= OnElementChildAdded;



    private void OnElementChildAdded(object? _, ElementEventArgs e)
    {
        if (e.Element is not null)
            AddElement(e.Element);
    }
}
