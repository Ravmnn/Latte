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


    public void From(Keyframe keyframe)
    {
        Clear();

        foreach (var (key, value) in keyframe)
            this[key] = value;
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
    : AnimationData(time, easing, false)
{
    public Element Element { get; set; } = element;

    public Keyframe? CurrentKeyframe { get; private set; }
    public Keyframe? BaseKeyframe { get; set; }
    protected IEnumerable<AnimationData> Animations { get; private set; } = [];


    public new void Abort()
    {
        base.Abort();

        foreach (AnimationData animation in Animations)
            animation.Abort();
    }

    public new void Start()
    {
        base.Start();

        foreach (AnimationData animation in Animations)
            animation.Start();
    }

    public new void Stop()
    {
        base.Stop();

        foreach (AnimationData animation in Animations)
            animation.Stop();
    }


    public void Animate(Keyframe to, double? time = null, Easing? easing = null)
    {
        Animations = [];

        Reset();
        Start();

        CurrentKeyframe = to;

        if (BaseKeyframe is not null)
            Animate(Element, BaseKeyframe, time ?? Time, easing ?? Easing);

        if (BaseKeyframe != to)
            Animations = Animate(Element, to, time ?? Time, easing ?? Easing);
    }


    // TODO: return IEnumerable or IList

    public static IEnumerable<AnimationData> Animate(Element element, Keyframe to, double time, Easing easing = Easing.Linear)
    {
        List<AnimationData> animations = [];

        foreach (Property elementProperty in element.Properties)
        {
            bool keyframePropertyExists = to.TryGetValue(elementProperty.Name, out IAnimatable? targetPropertyValue);

            if (!keyframePropertyExists)
                continue;

            if (elementProperty is not AnimatableProperty animatableElementProperty)
                throw new InvalidOperationException($"Property \"{elementProperty.Name}\" is not an AnimatableProperty.");

            if (animatableElementProperty.Animate(targetPropertyValue!, time, easing) is AnimationData animation)
                animations.Add(animation);
        }

        return animations;
    }
}
