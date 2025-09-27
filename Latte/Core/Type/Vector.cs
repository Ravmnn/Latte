using System;
using System.Numerics;

using SFML.System;

using Latte.Tweening;


namespace Latte.Core.Type;




public class Vec2<T>(T x, T y) : ICloneable, IFloatArrayModifiable where T :
    IAdditionOperators<T, T, T>,
    ISubtractionOperators<T, T, T>,
    IMultiplyOperators<T, T, T>,
    IDivisionOperators<T, T, T>,
    IUnaryNegationOperators<T, T>,
    IEqualityOperators<T, T, bool>,
    IComparisonOperators<T, T, bool>,
    IConvertible,
    INumber<T>
{
    public T X { get; set; } = x;
    public T Y { get; set; } = y;




    public static implicit operator Vector2f(Vec2<T> vec2) => new Vector2f(vec2.X.ToSingle(null), vec2.Y.ToSingle(null));
    public static implicit operator Vector2i(Vec2<T> vec2) => new Vector2i(vec2.X.ToInt32(null), vec2.Y.ToInt32(null));
    public static implicit operator Vector2u(Vec2<T> vec2) => new Vector2u(vec2.X.ToUInt32(null), vec2.Y.ToUInt32(null));


    public static implicit operator Vec2f(Vec2<T> vec2) => new Vec2f(vec2.X.ToSingle(null), vec2.Y.ToSingle(null));
    public static implicit operator Vec2i(Vec2<T> vec2) => new Vec2i(vec2.X.ToInt32(null), vec2.Y.ToInt32(null));
    public static implicit operator Vec2u(Vec2<T> vec2) => new Vec2u(vec2.X.ToUInt32(null), vec2.Y.ToUInt32(null));




    public static Vec2<T> operator+(Vec2<T> left, Vec2<T> right) => new Vec2<T>(left.X + right.X, left.Y + right.Y);
    public static Vec2<T> operator-(Vec2<T> left, Vec2<T> right) => new Vec2<T>(left.X - right.X, left.Y - right.Y);
    public static Vec2<T> operator-(Vec2<T> right) => new Vec2<T>(-right.X, -right.Y);
    public static Vec2<T> operator*(Vec2<T> left, Vec2<T> right) => new Vec2<T>(left.X * right.X, left.Y * right.Y);
    public static Vec2<T> operator/(Vec2<T> left, Vec2<T> right) => new Vec2<T>(left.X / right.X, left.Y / right.Y);


    public static Vec2<T> operator*(Vec2<T> left, T right) => new Vec2<T>(left.X * right, left.Y * right);
    public static Vec2<T> operator/(Vec2<T> left, T right) => new Vec2<T>(left.X / right, left.Y / right);


    public static bool operator==(Vec2<T> left, Vec2<T> right) => left.Equals(right);
    public static bool operator!=(Vec2<T> left, Vec2<T> right) => !left.Equals(right);




    public bool Equals(Vec2<T> other)
        => X == other.X && Y == other.Y;


    public override bool Equals(object? obj)
        => obj is not null && Equals((Vec2<T>)obj);




    public override int GetHashCode() => HashCode.Combine(X, Y);




    public override string ToString() => $"Vec2<{typeof(T).Name}>({X}, {Y})";




    public Vec2<T> Copy() => (Clone() as Vec2<T>)!;
    public object Clone() => new Vec2<T>(X, Y);




    public void ModifyFrom(float[] values)
        => (X, Y) = (T.CreateChecked(values[0]), T.CreateChecked(values[1]));
}




public class Vec2f(float x = 0, float y = 0) : Vec2<float>(x, y)
{
    public static implicit operator Vec2f(Vector2f vec2) => new Vec2f(vec2.X, vec2.Y);
}




public class Vec2i(int x = 0, int y = 0) : Vec2<int>(x, y)
{
    public static implicit operator Vec2i(Vector2i vec2) => new Vec2i(vec2.X, vec2.Y);
}




public class Vec2u(uint x = 0, uint y = 0) : Vec2<uint>(x, y)
{
    public static implicit operator Vec2u(Vector2u vec2) => new Vec2u(vec2.X, vec2.Y);
}
