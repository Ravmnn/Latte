using Latte.Core.Type;
using Latte.Application;


namespace Latte.Tweening;


public static class Tween
{
    public static FloatsTweenAnimation From(float from, float to, double time, Easing easing = Easing.Linear) =>
        new FloatsTweenAnimation([from], [to], time, easing);

    public static FloatsTweenAnimation From(Vec2f from, Vec2f to, double time, Easing easing = Easing.Linear) =>
        new FloatsTweenAnimation([from.X, from.Y], [to.X, to.Y], time, easing);

    public static FloatsTweenAnimation From(Vec2i from, Vec2i to, double time, Easing easing = Easing.Linear) =>
        new FloatsTweenAnimation([from.X, from.Y], [to.X, to.Y], time, easing);

    public static FloatsTweenAnimation From(Vec2u from, Vec2u to, double time, Easing easing = Easing.Linear) =>
        new FloatsTweenAnimation([from.X, from.Y], [to.X, to.Y], time, easing);

    public static FloatsTweenAnimation From(ColorRGBA from, ColorRGBA to, double time, Easing easing = Easing.Linear) =>
        new FloatsTweenAnimation([from.R, from.G, from.B, from.A], [to.R, to.G, to.B, to.A], time, easing);


    public static FloatsTweenAnimation Animate(float from, float to, double time, Easing easing = Easing.Linear)
        => (App.AddTweenAnimation(From(from, to, time, easing)) as FloatsTweenAnimation)!;

    public static FloatsTweenAnimation Animate(Vec2f from, Vec2f to, double time, Easing easing = Easing.Linear)
        => (App.AddTweenAnimation(From(from, to, time, easing)) as FloatsTweenAnimation)!;

    public static FloatsTweenAnimation Animate(Vec2i from, Vec2i to, double time, Easing easing = Easing.Linear)
        => (App.AddTweenAnimation(From(from, to, time, easing)) as FloatsTweenAnimation)!;

    public static FloatsTweenAnimation Animate(Vec2u from, Vec2u to, double time, Easing easing = Easing.Linear)
        => (App.AddTweenAnimation(From(from, to, time, easing)) as FloatsTweenAnimation)!;

    public static FloatsTweenAnimation Animate(ColorRGBA from, ColorRGBA to, double time, Easing easing = Easing.Linear)
        => (App.AddTweenAnimation(From(from, to, time, easing)) as FloatsTweenAnimation)!;
}
