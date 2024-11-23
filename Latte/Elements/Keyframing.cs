using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Latte.Core.Animation;
using Latte.Elements.Primitives;


namespace Latte.Elements;


public class Keyframe() : IEnumerable
{
    public Dictionary<string, IAnimatable> Properties { get; } = [];


    public Keyframe(AnimatableProperty[] properties) : this()
    {
        Properties = ElementPropertiesToKeyframeProperties(properties);
    }

    
    // must implement for using collection initialization list
    public void Add(string name, IAnimatable value) => Set(name, value);
    public bool Remove(string name) => Properties.Remove(name);
    
    
    public bool Exists(string name) => Properties.ContainsKey(name);

    
    public void Set(string name, IAnimatable value) => Properties[name] = value;
    
    public void SetIfNotDefined(string name, IAnimatable value)
    {
        if (!Exists(name))
            Set(name, value);
    }

    public void SetIfDefined(string name, IAnimatable value)
    {
        if (Exists(name))
            Set(name, value);
    }
    
    
    public IAnimatable Get(string name) => Properties[name];
    public bool TryGet(string name, out IAnimatable? value) => Properties.TryGetValue(name, out value);
    
    
    public IAnimatable this[string name]
    {
        get => Properties[name];
        set => Properties[name] = value;
    }
    
    
    public IEnumerator GetEnumerator()
        => Properties.GetEnumerator();


    public static Dictionary<string, IAnimatable> ElementPropertiesToKeyframeProperties(AnimatableProperty[] properties)
        => (from property in properties select new KeyValuePair<string, IAnimatable>(property.Name, property.Value)).ToDictionary();
}


public class ElementKeyframeAnimator(Element element, double time, Easing easing = Easing.Linear)
{
    public Element Element { get; set; } = element;
    public Keyframe? DefaultProperties { get; set; }

    public double Time { get; set; } = time;
    public Easing Easing { get; set; } = easing;


    public void Animate(Keyframe to)
    {
        if (DefaultProperties is not null)
            Animate(Element, DefaultProperties, Time, Easing);
        
        Animate(Element, to, Time, Easing);
    }
    
    
    public static void Animate(Element element, Keyframe to, double time, Easing easing = Easing.Linear)
    {
        foreach (Property elementProperty in element.Properties)
        {
            bool keyframePropertyExists = to.TryGet(elementProperty.Name, out IAnimatable? targetPropertyValue);
            
            if (!keyframePropertyExists)
                continue;
            
            if (elementProperty is not AnimatableProperty animatableElementProperty)
                throw new InvalidOperationException($"Property \"{elementProperty.Name}\" is not an AnimatableProperty.");
            
            animatableElementProperty.Animate(targetPropertyValue!, time, easing);
        }
    }
}