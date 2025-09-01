using System;


namespace Latte.Tweening;


/// <summary>
/// Definition of easing functions.
/// </summary>
public enum Easing
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
        var mult = (int)MathF.Pow(2, pow - 1);

        if (t < 0.5)
            return mult * In(t, pow);

        return 1 - MathF.Pow(-2 * t + 2, pow) / 2;
    }


    public static float Ease(float t, Easing type) => type switch
    {
        Easing.Linear => Linear(t),

        Easing.EaseInQuad => InQuad(t),
        Easing.EaseInCubic => InCubic(t),
        Easing.EaseInQuart => InQuart(t),
        Easing.EaseInQuint => InQuint(t),

        Easing.EaseOutQuad => OutQuad(t),
        Easing.EaseOutCubic => OutCubic(t),
        Easing.EaseOutQuart => OutQuart(t),
        Easing.EaseOutQuint => OutQuint(t),

        Easing.EaseInOutQuad => InOutQuad(t),
        Easing.EaseInOutCubic => InOutCubic(t),
        Easing.EaseInOutQuart => InOutQuart(t),
        Easing.EaseInOutQuint => InOutQuint(t),

        Easing.EaseInExpo => InExpo(t),
        Easing.EaseOutExpo => OutExpo(t),
        Easing.EaseInOutExpo => InOutExpo(t),

        Easing.EaseInCirc => InCirc(t),
        Easing.EaseOutCirc => OutCirc(t),
        Easing.EaseInOutCirc => InOutCirc(t),

        Easing.EaseInElast => InElast(t),
        Easing.EaseOutElast => OutElast(t),
        Easing.EaseInOutElast => InOutElast(t),

        Easing.EaseInBack => InBack(t),
        Easing.EaseOutBack => OutBack(t),
        Easing.EaseInOutBack => InOutBack(t),

        Easing.EaseInBounce => InBounce(t),
        Easing.EaseOutBounce => OutBounce(t),
        Easing.EaseInOutBounce => InOutBounce(t),

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

        return (2f - MathF.Pow(2f, -20f * t + 10f)) / 2f;
    }


    public static float InCirc(float t) => 1f - MathF.Sqrt(1f - MathF.Pow(t, 2f));
    public static float OutCirc(float t) => MathF.Sqrt(1f - MathF.Pow(t - 1f, 2f));
    public static float InOutCirc(float t)
    {
        if (t < 0.5f)
            return (1f - MathF.Sqrt(1f - MathF.Pow(2f * t, 2f))) / 2f;

        return (MathF.Sqrt(1f - MathF.Pow(-2f * t + 2f, 2f)) + 1f) / 2f;
    }


    public static float InElast(float t)
    {
        const float C4 = 2f * MathF.PI / 3f;

        if (t is 0f or 1f)
            return t;

        return -MathF.Pow(2f, 10f * t - 10f) * MathF.Sin((t * 10f - 10.75f) * C4);
    }

    public static float OutElast(float t)
    {
        const float C4 = 2f * MathF.PI / 3f;

        if (t is 0f or 1f)
            return t;

        return MathF.Pow(2f, -10f * t) * MathF.Sin((t * 10f - 0.75f) * C4) + 1f;
    }

    public static float InOutElast(float t)
    {
        const float C5 = 2f * MathF.PI / 4.5f;

        if (t is 0f or 1f)
            return t;

        if (t < 0.5f)
            return -(MathF.Pow(2f, 20f * t - 10f) * MathF.Sin((20f * t - 11.125f) * C5)) / 2f;

        return MathF.Pow(2f, -20f * t + 10f) * MathF.Sin((20f * t - 11.125f) * C5) / 2f + 1f;
    }


    public static float InBack(float t)
    {
        const float C1 = 1.70158f;
        const float C3 = C1 + 1f;

        return C3 * MathF.Pow(t, 3f) - C1 * MathF.Pow(t, 2f);
    }

    public static float OutBack(float t)
    {
        const float C1 = 1.70158f;
        const float C3 = C1 + 1f;

        return 1f + C3 * MathF.Pow(t - 1f, 3f) + C1 * MathF.Pow(t - 1f, 2f);
    }

    public static float InOutBack(float t)
    {
        const float C1 = 1.70158f;
        const float C2 = C1 * 1.525f;

        if (t < 0.5f)
            return MathF.Pow(2f * t, 2f) * ((C2 + 1f) * 2f * t - C2) / 2f;
        return (MathF.Pow(2f * t - 2f, 2f) * ((C2 + 1f) * (t * 2f - 2f) + C2) + 2f) / 2f;
    }


    public static float InBounce(float t) => 1 - OutBounce(1f - t);

    public static float OutBounce(float t)
    {
        const float N1 = 7.5625f;
        const float D1 = 2.75f;

        if (t < 1f / D1)
            return N1 * t * t;

        if (t < 2 / D1)
            return N1 * (t -= 1.5f / D1) * t + 0.75f;

        if (t < 2.5 / D1)
            return N1 * (t -= 2.25f / D1) * t + 0.9375f;

        return N1 * (t -= 2.625f / D1) * t + 0.984375f;
    }

    public static float InOutBounce(float t)
    {
        if (t < 0.5f)
            return (1f - OutBounce(1f - 2f * t)) / 2f;

        return (1f + OutBounce(2f * t - 1f)) / 2f;
    }
}
