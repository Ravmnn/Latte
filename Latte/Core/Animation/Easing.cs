using System;


namespace Latte.Core.Animation;


/// <summary>
/// Definition of easing functions. 
/// </summary>
public enum EasingType
{
    Linear,

    EaseInQuad, EaseInCubic, EaseInQuart, EaseInQuint,
    EaseOutQuad, EaseOutCubic, EaseOutQuart, EaseOutQuint,
    EaseInOutQuad, EaseInOutCubic, EaseInOutQuart, EaseInOutQuint,

    EaseInExpo, EaseOutExpo, EaseInOutExpo,
    EaseInCirc, EaseOutCirc, EaseInOutCirc,
    EaseInElast, EaseOutElast, EaseInOutElast,
    EaseInBack, EaseOutBack, EaseInOutBack,
    EaseInBounce, EaseOutBounce, EaseInOutBounce
}


/// <summary>
/// Stores a lot of functions for easing animations, where "t" is the animation progress
/// from 0 to 1.
/// </summary>
public static class EasingFunctions
{
    private static float In(float t, int pow) => MathF.Pow(t, pow);
    private static float Out(float t, int pow) => 1 - MathF.Pow(1 - t, pow);

    private static float InOut(float t, int pow)
    {
        int mult = (int)MathF.Pow(2, pow - 1);

        if (t < 0.5)
            return mult * In(t, pow);
        else
            return 1 - MathF.Pow(-2 * t + 2, pow) / 2;
    }


    public static float Ease(float t, EasingType type) => type switch
    {
        EasingType.Linear => Linear(t),

        EasingType.EaseInQuad => InQuad(t),
        EasingType.EaseInCubic => InCubic(t),
        EasingType.EaseInQuart => InQuart(t),
        EasingType.EaseInQuint => InQuint(t),

        EasingType.EaseOutQuad => OutQuad(t),
        EasingType.EaseOutCubic => OutCubic(t),
        EasingType.EaseOutQuart => OutQuart(t),
        EasingType.EaseOutQuint => OutQuint(t),

        EasingType.EaseInOutQuad => InOutQuad(t),
        EasingType.EaseInOutCubic => InOutCubic(t),
        EasingType.EaseInOutQuart => InOutQuart(t),
        EasingType.EaseInOutQuint => InOutQuint(t),

        EasingType.EaseInExpo => InExpo(t),
        EasingType.EaseOutExpo => OutExpo(t),
        EasingType.EaseInOutExpo => InOutExpo(t),

        EasingType.EaseInCirc => InCirc(t),
        EasingType.EaseOutCirc => OutCirc(t),
        EasingType.EaseInOutCirc => InOutCirc(t),

        EasingType.EaseInElast => InElast(t),
        EasingType.EaseOutElast => OutElast(t),
        EasingType.EaseInOutElast => InOutElast(t),

        EasingType.EaseInBack => InBack(t),
        EasingType.EaseOutBack => OutBack(t),
        EasingType.EaseInOutBack => InOutBack(t),

        EasingType.EaseInBounce => InBounce(t),
        EasingType.EaseOutBounce => OutBounce(t),
        EasingType.EaseInOutBounce => InOutBounce(t),

        _ => Linear(t)
    };


    public static float Linear(float t) => t;


    public static float InQuad(float t) => In(t, 2);
    public static float InCubic(float t) => In(t, 3);
    public static float InQuart(float t) => In(t, 4);
    public static float InQuint(float t) => In(t, 5);


    public static float OutQuad(float t) => Out(t, 2);
    public static float OutCubic(float t) => Out(t, 3);
    public static float OutQuart(float t) => Out(t, 4);
    public static float OutQuint(float t) => Out(t, 5);


    public static float InOutQuad(float t) => InOut(t, 2);
    public static float InOutCubic(float t) => InOut(t, 3);
    public static float InOutQuart(float t) => InOut(t, 4);
    public static float InOutQuint(float t) => InOut(t, 5);


    public static float InExpo(float t) => t == 0f ? 0f : MathF.Pow(2f, 10f * t - 10f);
    public static float OutExpo(float t) => t == 1f ? 1f : 1f - MathF.Pow(2f, -10f * t);
    public static float InOutExpo(float t)
    {
        if (t is 0f or 1f)
            return t;

        if (t < 0.5f)
            return MathF.Pow(2f, 20f * t - 10f) / 2f;
        else
            return (2f - MathF.Pow(2f, -20f * t + 10f)) / 2f;
    }


    public static float InCirc(float t) => 1f - MathF.Sqrt(1f - MathF.Pow(t, 2f));
    public static float OutCirc(float t) => MathF.Sqrt(1f - MathF.Pow(t - 1f, 2f));
    public static float InOutCirc(float t)
    {
        if (t < 0.5f)
            return (1f - MathF.Sqrt(1f - MathF.Pow(2f * t, 2f))) / 2f;
        else
            return (MathF.Sqrt(1f - MathF.Pow(-2f * t + 2f, 2f)) + 1f) / 2f;
    }


    public static float InElast(float t)
    {
        const float c4 = 2f * MathF.PI / 3f;

        if (t is 0f or 1f)
            return t;

        return -MathF.Pow(2f, 10f * t - 10f) * MathF.Sin((t * 10f - 10.75f) * c4);
    }

    public static float OutElast(float t)
    {
        const float c4 = 2f * MathF.PI / 3f;

        if (t is 0f or 1f)
            return t;

        return MathF.Pow(2f, -10f * t) * MathF.Sin((t * 10f - 0.75f) * c4) + 1f;
    }

    public static float InOutElast(float t)
    {
        const float c5 = 2f * MathF.PI / 4.5f;

        if (t is 0f or 1f)
            return t;

        if (t < 0.5f)
            return -(MathF.Pow(2f, 20f * t - 10f) * MathF.Sin((20f * t - 11.125f) * c5)) / 2f;
        else
            return MathF.Pow(2f, -20f * t + 10f) * MathF.Sin((20f * t - 11.125f) * c5) / 2f + 1f;
    }


    public static float InBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;

        return c3 * MathF.Pow(t, 3f) - c1 * MathF.Pow(t, 2f);
    }

    public static float OutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;

        return 1f + c3 * MathF.Pow(t - 1f, 3f) + c1 * MathF.Pow(t - 1f, 2f);
    }

    public static float InOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c2 = c1 * 1.525f;

        if (t < 0.5f)
            return MathF.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2) / 2f;
        else
            return (MathF.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) / 2f;
    }


    public static float InBounce(float t) => 1 - OutBounce(1f - t);

    public static float OutBounce(float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (t < 1f / d1)
            return n1 * t * t;
        
        if (t < 2 / d1) 
            return n1 * (t -= 1.5f / d1) * t + 0.75f;
        
        if (t < 2.5 / d1) 
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;
        
        return n1 * (t -= 2.625f / d1) * t + 0.984375f;
    }

    public static float InOutBounce(float t)
    {
        if (t < 0.5f)
            return (1f - OutBounce(1f - 2f * t)) / 2f;
        
        return (1f + OutBounce(2f * t - 1f)) / 2f;
    }
}