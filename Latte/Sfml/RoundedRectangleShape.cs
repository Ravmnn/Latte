using System;

using SFML.System;
using SFML.Graphics;


namespace Latte.Sfml;


// this class original implementation was taken from
// https://github.com/SFML/SFML/wiki/Source%3A-Draw-Rounded-Rectangle

// it was modified to solve some problems with the rounded corner
// rendering. If Radius == 0, this class literally behaves like a RectangleShape

public class RoundedRectangleShape : Shape
{
    private Vector2f _size;
    private float _radius;
    private uint _cornerPointCount;
    
    
    public Vector2f Size
    {
        get => _size;
        set
        {
            _size = value;
            Update();
        }
    }

    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            Update();
        }
    }

    public uint CornerPointCount
    {
        get => _cornerPointCount;
        set
        {
            _cornerPointCount = value;
            Update();
        }
    }
    
    
    public RoundedRectangleShape(Vector2f size, float radius, uint cornerPointCount)
    {
        Size = size;
        Radius = radius;
        CornerPointCount = cornerPointCount;
    }


    public override uint GetPointCount()
        => Radius != 0 ? CornerPointCount * 4 : 4;
    

    private Vector2f RectangleGetPoint(uint index) => index switch
    {
        1 => new Vector2f(Size.X, 0),
        2 => new Vector2f(Size.X, Size.Y),
        3 => new Vector2f(0, Size.Y),
        _ => new Vector2f(0, 0)
    };

    public override Vector2f GetPoint(uint index)
    {
        const float Pi = 3.141592654f;

        // this GetPoint will not work properly if Radius is 0
        if (Radius == 0f)
            return RectangleGetPoint(index);
        
        if (index >= CornerPointCount * 4)
            return new Vector2f();

        var deltaAngle = 90.0f / (CornerPointCount - 1);
        
        var center = new Vector2f();
        var centerIndex = index / CornerPointCount;

        switch (centerIndex)
        {
            case 0:
                center.X = Size.X - Radius;
                center.Y = Radius;
                break;
            
            case 1:
                center.X = Radius;
                center.Y = Radius;
                break;
            
            case 2:
                center.X = Radius;
                center.Y = Size.Y - Radius;
                break;
            
            case 3:
                center.X = Size.X - Radius;
                center.Y = Size.Y - Radius;
                break;
        }

        return new Vector2f(Radius * MathF.Cos(deltaAngle * (index - centerIndex) * Pi / 180f) + center.X,
            -Radius * MathF.Sin(deltaAngle * (index - centerIndex) * Pi / 180f) + center.Y);
    }
}