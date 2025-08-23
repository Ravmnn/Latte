using SFML.Graphics;
using SFML.System;

using Latte.Core.Type;


namespace Latte.Animation;


public static class Animate
{
    public static FloatAnimation Value(float from, float to, double time, Easing easing = Easing.Linear) =>
        new FloatAnimation([from], [to], time, easing);


    public static FloatAnimation Vec2f(Vec2f from, Vec2f to, double time, Easing easing = Easing.Linear) =>
        new FloatAnimation([from.X, from.Y], [to.X, to.Y], time, easing);

    public static FloatAnimation Vec2i(Vec2i from, Vec2i to, double time, Easing easing = Easing.Linear) =>
        new FloatAnimation([from.X, from.Y], [to.X, to.Y], time, easing);

    public static FloatAnimation Vec2u(Vec2u from, Vec2u to, double time, Easing easing = Easing.Linear) =>
        new FloatAnimation([from.X, from.Y], [to.X, to.Y], time, easing);


    public static FloatAnimation Color(ColorRGBA from, ColorRGBA to, double time, Easing easing = Easing.Linear) =>
        new FloatAnimation([from.R, from.G, from.B, from.A], [to.R, to.G, to.B, to.A], time, easing);



    public static FloatAnimation Vector2f(Vector2f from, Vector2f to, double time, Easing easing = Easing.Linear)
        => Vec2f(from, to, time, easing);

    public static FloatAnimation Vector2i(Vector2i from, Vector2i to, double time, Easing easing = Easing.Linear)
        => Vec2i(from, to, time, easing);

    public static FloatAnimation Vector2u(Vector2u from, Vector2u to, double time, Easing easing = Easing.Linear)
        => Vec2u(from, to, time, easing);


    public static FloatAnimation SFColor(Color from, Color to, double time, Easing easing = Easing.Linear)
        => Color(from, to, time, easing);
}
