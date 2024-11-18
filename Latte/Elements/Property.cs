using Latte.Core.Animation;
using Latte.Elements.Primitives;
using OpenTK.Graphics.GL;


namespace Latte.Elements;


public class Property<T> where T : IAnimateable<T>
{
    public Element Owner { get; }
    public T Value { get; set; }


    public Property(Element owner, T value)
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


    public static implicit operator T(Property<T> property) => property.Value;
}