using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Graphics;

using Latte.Elements.Primitives;


namespace Latte.Core.Application;


// TODO: add a list of elements in which their new children are added automatically


public class Section : IUpdateable, IDrawable
{
    // Elements are ordered based on their priority
    private List<Element> _elements;


    public Element[] Elements => _elements.ToArray();

    public event EventHandler<ElementEventArgs>? ElementAddedEvent;
    public event EventHandler<ElementEventArgs>? ElementRemovedEvent;
    public event EventHandler<ElementEventArgs>? ElementListModifiedEvent;


    public Section()
    {
        _elements = [];

        ElementAddedEvent += (_, args) => ElementListModifiedEvent?.Invoke(this, args);
        ElementRemovedEvent += (_, args) => ElementListModifiedEvent?.Invoke(this, args);
    }


    public virtual void Initialize() { }
    public virtual void Deinitialize() { }

    public virtual void Update() => SortElementListByPriority();
    public virtual void Draw(RenderTarget target) { }


    private void SortElementListByPriority()
        => _elements = _elements.OrderBy(element => element.Priority).ToList();


    public void AddElement(Element element)
    {
        AddSingleElement(element);
        AddElementsHierarchy(element.Children);
    }

    private void AddElementsHierarchy(List<Element> elements)
        => elements.ForeachElement(AddSingleElement);

    private void AddSingleElement(Element element)
    {
        if (HasElement(element))
            return;

        // TODO: probably this is not wanted.

        // if (_elements.Count > 0)
        //     element.Priority = _elements.Last().Priority + 1;

        _elements.Add(element);

        ElementAddedEvent?.Invoke(null, new(element));
    }

    // private void AddSingleElements(IEnumerable<Element> elements)
    // {
    //     foreach (Element element in elements)
    //         AddSingleElement(element);
    // }


    public bool RemoveElement(Element element)
    {
        if (!RemoveSingleElement(element))
            return false;

        RemoveElementsChildrenOf(element);
        return true;
    }

    private void RemoveElementsChildrenOf(Element parent)
    {
        foreach (Element element in _elements.ToArray().Reverse())
            if (element.IsChildOf(parent))
                RemoveSingleElement(element);
    }

    private bool RemoveSingleElement(Element element)
    {
        bool result = _elements.Remove(element);

        ElementRemovedEvent?.Invoke(null, new(element));

        return result;
    }


    public bool HasElement(Element element)
        => _elements.Contains(element);
}
