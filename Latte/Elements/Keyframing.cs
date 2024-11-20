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
    
    public void Set(string name, IAnimatable value) => Properties[name] = value;
    public bool Remove(string name) => Properties.Remove(name);
    public bool Exists(string name) => Properties.ContainsKey(name);


    public void SetIfNotDefined(string name, IAnimatable value)
    {
        if (!Exists(name))
            Set(name, value);
    }
    
    
    public IEnumerator GetEnumerator()
        => Properties.GetEnumerator();


    public IAnimatable this[string name]
    {
        get => Properties[name];
        set => Properties[name] = value;
    }


    public static Dictionary<string, IAnimatable> ElementPropertiesToKeyframeProperties(AnimatableProperty[] properties)
        => (from property in properties select new KeyValuePair<string, IAnimatable>(property.Name, property.Value)).ToDictionary();
}


public class ElementKeyframeAnimator(Element element, double time, EasingType easingType = EasingType.Linear)
{
    public Element Element { get; set; } = element;
    public Keyframe? DefaultProperties { get; set; }

    public double Time { get; set; } = time;
    public EasingType EasingType { get; set; } = easingType;


    public void Animate(Keyframe to)
    {
        if (DefaultProperties is not null)
            Animate(Element, DefaultProperties, Time, EasingType);
        
        Animate(Element, to, Time, EasingType);
    }
    
    
    public static void Animate(Element element, Keyframe to, double time, EasingType easingType = EasingType.Linear)
    {
        foreach (Property property in element.Properties)
        {
            if (!to.Properties.TryGetValue(property.Name, out IAnimatable? targetValue))
                continue;
            
            if (property is not AnimatableProperty animatableProperty)
                throw new InvalidOperationException($"Property \"{property.Name}\" is not an AnimatableProperty.");
         
            if (!animatableProperty.ShouldAnimatorIgnore)
                animatableProperty.Animate(targetValue, time, easingType);
        }
    }
}