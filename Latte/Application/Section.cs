using System;
using System.Linq;
using System.Collections.Generic;

using Latte.Core;
using Latte.Core.Objects;
using Latte.UI;
using Latte.UI.Elements;


namespace Latte.Application;




// TODO: improve this... make this really useful
public class Section : IUpdateable, IDrawable
{
    // Elements are ordered based on their priority
    private List<BaseObject> _objects;




    public IEnumerable<BaseObject> Objects => _objects;




    public event EventHandler<BaseObjectEventArgs>? ObjectAddedEvent;
    public event EventHandler<BaseObjectEventArgs>? ObjectRemovedEvent;
    public event EventHandler<BaseObjectEventArgs>? ObjectListModifiedEvent;


    public event EventHandler? UpdateEvent;
    public event EventHandler? DrawEvent;




    public Section()
    {
        _objects = [];

        ObjectAddedEvent += (_, args) => ObjectListModifiedEvent?.Invoke(this, args);
        ObjectRemovedEvent += (_, args) => ObjectListModifiedEvent?.Invoke(this, args);
    }




    public virtual void Initialize() { }
    public virtual void Deinitialize() { }




    public virtual void Update()
    {
        SortObjectListByPriority();

        UpdateEvent?.Invoke(this, EventArgs.Empty);
    }


    private void SortObjectListByPriority()
        => _objects = _objects.OrderBy(@object => @object.Priority).ToList();




    public virtual void Draw(IRenderer renderer)
        => DrawEvent?.Invoke(this, EventArgs.Empty);




    public void AddObjects(params IEnumerable<BaseObject> objects)
    {
        foreach (var @object in objects)
            AddObject(@object);
    }


    public void AddObject(BaseObject @object)
    {
        if (HasObject(@object))
            return;

        _objects.Add(@object);

        AddEventListenersTo(@object);

        ObjectAddedEvent?.Invoke(null, new BaseObjectEventArgs(@object));
    }




    public void RemoveObjects(params IEnumerable<BaseObject> objects)
    {
        foreach (var @object in objects)
            RemoveObject(@object);
    }


    public bool RemoveObject(BaseObject @object)
    {
        var result = _objects.Remove(@object);

        RemoveEventListenersOf(@object);

        ObjectRemovedEvent?.Invoke(null, new BaseObjectEventArgs(@object));

        return result;
    }




    public void AddElements(params IEnumerable<Element> elements)
    {
        foreach (var element in elements)
            AddElement(element);
    }


    public void AddElement(Element element)
    {
        AddObject(element);
        AddElementsHierarchy(element.Children);
    }


    private void AddElementsHierarchy(IEnumerable<Element> elements)
        => elements.ForeachElementRecursively(AddObject);




    public void RemoveElements(params IEnumerable<Element> elements)
    {
        foreach (var element in elements)
            RemoveElement(element);
    }


    public bool RemoveElement(Element element)
    {
        if (!RemoveObject(element))
            return false;

        RemoveElementsChildrenOf(element);
        return true;
    }


    private void RemoveElementsChildrenOf(Element parent)
    {
        foreach (var @object in _objects.ToArray().Reverse())
            if (@object is Element child && child.IsChildOf(parent))
                RemoveObject(child);
    }




    public bool HasObject(BaseObject @object)
        => _objects.Contains(@object);




    public IEnumerable<T> GetObjectsOfType<T>() where T : class
        => from @object in Objects
            let objectOfType = @object as T
            where objectOfType is not null
            select  objectOfType;




    public void AddEventListenersTo(BaseObject @object)
    {
        if (@object is Element element)
            AddChildAddedListenerTo(element);

        if (@object is IFocusable focusable)
            FocusManager.AddFocusListenerTo(focusable);
    }


    public void RemoveEventListenersOf(BaseObject @object)
    {
        if (@object is Element element)
            RemoveChildAddedListenerOf(element);

        if (@object is IFocusable focusable)
            FocusManager.RemoveFocusListenerOf(focusable);
    }




    private void AddChildAddedListenerTo(Element element)
        => element.ChildAddedEvent += OnElementChildAdded;


    private void RemoveChildAddedListenerOf(Element element)
        => element.ChildAddedEvent -= OnElementChildAdded;




    private void OnElementChildAdded(object? _, ElementEventArgs e)
    {
        if (e.Element is not null)
            AddObject(e.Element);
    }
}
