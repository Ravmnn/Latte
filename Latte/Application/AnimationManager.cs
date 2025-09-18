using System;
using System.Collections.Generic;

using Latte.Tweening;


namespace Latte.Application;


public static class AnimationManager
{
    public static List<TweenAnimation> TweenAnimations { get; } = [];


    public static void Update()
    {
        foreach (var animation in TweenAnimations.ToArray())
            animation.Update();
    }


    public static TweenAnimation AddTweenAnimation(TweenAnimation animation)
    {
        if (TweenAnimations.Contains(animation))
            return animation;

        animation.AbortEvent += OnAnimationEnd;
        animation.FinishEvent += OnAnimationEnd;

        TweenAnimations.Add(animation);

        return animation;
    }

    public static TweenAnimation RemoveTweenAnimation(TweenAnimation animation)
    {
        TweenAnimations.Remove(animation);

        animation.AbortEvent -= OnAnimationEnd;
        animation.FinishEvent -= OnAnimationEnd;

        return animation;
    }


    private static void OnAnimationEnd(object? sender, EventArgs _)
    {
        if (sender is not TweenAnimation animation)
            return;

        RemoveTweenAnimation(animation);
    }
}
