using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

using Latte.Core.Animation;
using Latte.Elements.Primitives;


namespace Latte.Elements;


public class Keyframe() : IEnumerable
{
    public Dictionary<string, IAnimatable> Properties { get; } = [];


    public Keyframe(Property[] properties) : this()
    {
        Properties = ElementPropertiesToKeyframeProperties(properties);
    }
        
    
    public void Add(string name, IAnimatable value)
        => Properties[name] = value;
    
    public bool Remove(string name)
        => Properties.Remove(name);
    
    public bool Exists(string name)
        => Properties.ContainsKey(name);
    
    
    public IEnumerator GetEnumerator()
        => Properties.GetEnumerator();


    public static Dictionary<string, IAnimatable> ElementPropertiesToKeyframeProperties(Property[] properties)
        => (from property in properties
            where property is AnimatableProperty
            let animatableProperty = property as AnimatableProperty
            select new KeyValuePair<string, IAnimatable>(animatableProperty.Name, animatableProperty.Value)).ToDictionary();
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
        foreach (var (key, property) in element.Properties)
        {
            if (!to.Properties.TryGetValue(key, out IAnimatable? targetValue))
                continue;

            if (property is not AnimatableProperty animatableProperty)
                throw new InvalidOperationException($"Property \"{property.Name}\" is not an AnimatableProperty.");
                
            animatableProperty.Animate(targetValue, time, easingType);
        }
    }
}