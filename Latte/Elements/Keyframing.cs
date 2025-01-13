using System;
using System.Linq;
using System.Collections.Generic;

using Latte.Core.Animation;
using Latte.Elements.Primitives;


namespace Latte.Elements;


public class Keyframe() : Dictionary<string, IAnimatable>
{
    public Keyframe(AnimatableProperty[] properties) : this()
    {
        foreach (var (name, value) in ElementPropertiesToKeyframeProperties(properties))
            this[name] = value;
    }


    public void SetIfNotDefined(string name, IAnimatable value)
    {
        if (!ContainsKey(name))
            this[name] = value;
    }

    public void SetIfDefined(string name, IAnimatable value)
    {
        if (ContainsKey(name))
            this[name] = value;
    }


    private static Dictionary<string, IAnimatable> ElementPropertiesToKeyframeProperties(AnimatableProperty[] properties)
        => (from property in properties select new KeyValuePair<string, IAnimatable>(property.Name, property.Value)).ToDictionary();
}


public class ElementKeyframeAnimator(Element element, double time, Easing easing = Easing.Linear)
{
    public Element Element { get; set; } = element;
    public Keyframe? DefaultProperties { get; set; } // TODO: improve this

    public double Time { get; set; } = time;
    public Easing Easing { get; set; } = easing;


    public void Animate(Keyframe to, double? time = null, Easing? easing = null)
    {
        if (DefaultProperties is not null)
            Animate(Element, DefaultProperties, time ?? Time, easing ?? Easing);

        Animate(Element, to, time ?? Time, easing ?? Easing);
    }


    public static void Animate(Element element, Keyframe to, double time, Easing easing = Easing.Linear)
    {
        foreach (Property elementProperty in element.Properties)
        {
            bool keyframePropertyExists = to.TryGetValue(elementProperty.Name, out IAnimatable? targetPropertyValue);

            if (!keyframePropertyExists)
                continue;

            if (elementProperty is not AnimatableProperty animatableElementProperty)
                throw new InvalidOperationException($"Property \"{elementProperty.Name}\" is not an AnimatableProperty.");

            animatableElementProperty.Animate(targetPropertyValue!, time, easing);
        }
    }
}
