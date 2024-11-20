using System;
using System.Linq;
using System.Collections.Generic;

using Latte.Core.Animation;
using Latte.Elements.Primitives;


namespace Latte.Elements;


public readonly struct Keyframe(Dictionary<string, IAnimatable> properties)
{
    public Dictionary<string, IAnimatable> Properties { get; } = properties;


    public static Dictionary<string, IAnimatable> PropertiesToKeyframeProperties(Property[] properties)
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
            Animate(Element, DefaultProperties.Value, Time, EasingType);
        
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