using Latte.Core.Animation;
using Latte.Elements.Primitives;


namespace Latte.Elements;


public class AnimatableProperty<T> where T : IAnimatable<T>
{
    public Element Owner { get; }
    public T Value { get; set; }


    public AnimatableProperty(Element owner, T value)
    {
        Owner = owner;
        Value = value;
    }
    
    
    public void Set(T value) => Value = value;


    public void Animate(T to, float time, EasingType easingType = EasingType.Linear)
    {
        AnimationState animation = Value.AnimateThis(to, time, easingType);
        animation.Updated += (_, args) => Value = Value.AnimationValuesToThis(args.CurrentValues);
        
        Owner.AddPropertyAnimation(animation);
    }


    public static implicit operator T(AnimatableProperty<T> animatableProperty) => animatableProperty.Value;
}