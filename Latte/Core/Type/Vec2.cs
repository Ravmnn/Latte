using System;

using SFML.System;

using Latte.Tweening;


namespace Latte.Core.Type;




public struct Vec2f(float x = 0, float y = 0) : IFloatArrayModifiable
{
    public float X { get; set; } = x;
    public float Y { get; set; } = y;




    public static implicit operator Vector2f(Vec2f vec2) => new Vector2f(vec2.X, vec2.Y);
    public static implicit operator Vector2i(Vec2f vec2) => new Vector2i((int)vec2.X, (int)vec2.Y);
    public static implicit operator Vector2u(Vec2f vec2) => new Vector2u((uint)vec2.X, (uint)vec2.Y);

    public static implicit operator Vec2f(Vector2f vec2) => new Vec2f(vec2.X, vec2.Y);
    public static implicit operator Vec2f(Vector2i vec2) => new Vec2f(vec2.X, vec2.Y);
    public static implicit operator Vec2f(Vector2u vec2) => new Vec2f(vec2.X, vec2.Y);


    public static implicit operator Vec2i(Vec2f vec2) => new Vec2i((int)vec2.X, (int)vec2.Y);
    public static implicit operator Vec2u(Vec2f vec2) => new Vec2u((uint)vec2.X, (uint)vec2.Y);




    public static Vec2f operator+(Vec2f left, Vec2f right) => new Vec2f(left.X + right.X, left.Y + right.Y);
    public static Vec2f operator-(Vec2f left, Vec2f right) => new Vec2f(left.X - right.X, left.Y - right.Y);
    public static Vec2f operator-(Vec2f right) => new Vec2f(-right.X, -right.Y);
    public static Vec2f operator*(Vec2f left, Vec2f right) => new Vec2f(left.X * right.X, left.Y * right.Y);
    public static Vec2f operator/(Vec2f left, Vec2f right) => new Vec2f(left.X / right.X, left.Y / right.Y);


    public static Vec2f operator*(Vec2f left, float right) => new Vec2f(left.X * right, left.Y * right);
    public static Vec2f operator/(Vec2f left, float right) => new Vec2f(left.X / right, left.Y / right);


    public static bool operator==(Vec2f left, Vec2f right) => left.Equals(right);
    public static bool operator!=(Vec2f left, Vec2f right) => !left.Equals(right);




    public bool Equals(Vec2f other)
        => X == other.X && Y == other.Y;


    public override bool Equals(object? obj)
        => obj is not null && Equals((Vec2f)obj);




    public override int GetHashCode() => HashCode.Combine(X, Y);




    public override string ToString() => $"Vec2<{nameof(Single)}>({X}, {Y})";




    public void ModifyFrom(float[] values)
        => (X, Y) = (float.CreateChecked(values[0]), float.CreateChecked(values[1]));
}




public class Vec2i(int x = 0, int y = 0) : IFloatArrayModifiable
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;




    public static implicit operator Vector2f(Vec2i vec2) => new Vector2f(vec2.X, vec2.Y);
    public static implicit operator Vector2i(Vec2i vec2) => new Vector2i(vec2.X, vec2.Y);
    public static implicit operator Vector2u(Vec2i vec2) => new Vector2u((uint)vec2.X, (uint)vec2.Y);

    public static implicit operator Vec2i(Vector2f vec2) => new Vec2i((int)vec2.X, (int)vec2.Y);
    public static implicit operator Vec2i(Vector2i vec2) => new Vec2i(vec2.X, vec2.Y);
    public static implicit operator Vec2i(Vector2u vec2) => new Vec2i((int)vec2.X, (int)vec2.Y);


    public static implicit operator Vec2f(Vec2i vec2) => new Vec2f(vec2.X, vec2.Y);
    public static implicit operator Vec2u(Vec2i vec2) => new Vec2u((uint)vec2.X, (uint)vec2.Y);




    public static Vec2i operator+(Vec2i left, Vec2i right) => new Vec2i(left.X + right.X, left.Y + right.Y);
    public static Vec2i operator-(Vec2i left, Vec2i right) => new Vec2i(left.X - right.X, left.Y - right.Y);
    public static Vec2i operator-(Vec2i right) => new Vec2i(-right.X, -right.Y);
    public static Vec2i operator*(Vec2i left, Vec2i right) => new Vec2i(left.X * right.X, left.Y * right.Y);
    public static Vec2i operator/(Vec2i left, Vec2i right) => new Vec2i(left.X / right.X, left.Y / right.Y);


    public static Vec2i operator*(Vec2i left, float right) => new Vec2i((int)(left.X * right), (int)(left.Y * right));
    public static Vec2i operator/(Vec2i left, float right) => new Vec2i((int)(left.X / right), (int)(left.Y / right));


    public static bool operator==(Vec2i left, Vec2i right) => left.Equals(right);
    public static bool operator!=(Vec2i left, Vec2i right) => !left.Equals(right);




    public bool Equals(Vec2i other)
        => X == other.X && Y == other.Y;


    public override bool Equals(object? obj)
        => obj is not null && Equals((Vec2i)obj);




    public override int GetHashCode() => HashCode.Combine(X, Y);




    public override string ToString() => $"Vec2<{nameof(Single)}>({X}, {Y})";




    public void ModifyFrom(float[] values)
        => (X, Y) = (int.CreateChecked(values[0]), int.CreateChecked(values[1]));
}



public class Vec2u(uint x = 0, uint y = 0) : IFloatArrayModifiable
{
    public uint X { get; set; } = x;
    public uint Y { get; set; } = y;




    public static implicit operator Vector2f(Vec2u vec2) => new Vector2f(vec2.X, vec2.Y);
    public static implicit operator Vector2i(Vec2u vec2) => new Vector2i((int)vec2.X, (int)vec2.Y);
    public static implicit operator Vector2u(Vec2u vec2) => new Vector2u(vec2.X, vec2.Y);

    public static implicit operator Vec2u(Vector2f vec2) => new Vec2u((uint)vec2.X, (uint)vec2.Y);
    public static implicit operator Vec2u(Vector2i vec2) => new Vec2u((uint)vec2.X, (uint)vec2.Y);
    public static implicit operator Vec2u(Vector2u vec2) => new Vec2u(vec2.X, vec2.Y);


    public static implicit operator Vec2i(Vec2u vec2) => new Vec2i((int)vec2.X, (int)vec2.Y);
    public static implicit operator Vec2f(Vec2u vec2) => new Vec2f(vec2.X, vec2.Y);




    public static Vec2u operator+(Vec2u left, Vec2u right) => new Vec2u(left.X + right.X, left.Y + right.Y);
    public static Vec2u operator-(Vec2u left, Vec2u right) => new Vec2u(left.X - right.X, left.Y - right.Y);
    public static Vec2u operator*(Vec2u left, Vec2u right) => new Vec2u(left.X * right.X, left.Y * right.Y);
    public static Vec2u operator/(Vec2u left, Vec2u right) => new Vec2u(left.X / right.X, left.Y / right.Y);


    public static Vec2u operator*(Vec2u left, float right) => new Vec2u((uint)(left.X * right), (uint)(left.Y * right));
    public static Vec2u operator/(Vec2u left, float right) => new Vec2u((uint)(left.X / right), (uint)(left.Y / right));


    public static bool operator==(Vec2u left, Vec2u right) => left.Equals(right);
    public static bool operator!=(Vec2u left, Vec2u right) => !left.Equals(right);




    public bool Equals(Vec2u other)
        => X == other.X && Y == other.Y;


    public override bool Equals(object? obj)
        => obj is not null && Equals((Vec2u)obj);




    public override int GetHashCode() => HashCode.Combine(X, Y);




    public override string ToString() => $"Vec2<{nameof(Single)}>({X}, {Y})";




    public void ModifyFrom(float[] values)
        => (X, Y) = (uint.CreateChecked(values[0]), uint.CreateChecked(values[1]));
}
