using SFML.System;
using SFML.Window;
using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Core;


public class Window(VideoMode mode, string title, Styles style = Styles.Default, ContextSettings? settings = null)
    : RenderWindow(mode, title, style, settings ?? DefaultSettings), IUpdateable, IDrawable
{
    // Usually, an error message "X Error of failed request:  BadCursor (invalid Cursor parameter)"
    // from the X window compositor (Linux only) was being shown
    // whenever a program using Latte was closed. The error could randomly happen or not.
    // That error looks like to be the result of freeing the memory of a Cursor object after
    // setting it as the Window cursor.
    // Basically... to solve it, the below field needs to exist.

    // TODO: create a cursor class that manages the cursor type better

    private Cursor _cursor = new Cursor(Cursor.CursorType.Arrow);


    public Cursor Cursor
    {
        get => _cursor;
        set
        {
            _cursor = value;
            SetMouseCursor(_cursor);
        }
    }

    public Vec2i MousePosition => Mouse.GetPosition(this);
    public Vec2f ViewMousePosition => MapPixelToCoords(MousePosition);

    public IntRect WindowRect => new IntRect(new Vector2i(0, 0), (Vector2i)Size);


    public static ContextSettings DefaultSettings { get; }


    static Window()
    {
        DefaultSettings = new ContextSettings
        {
            AntialiasingLevel = 2,
            DepthBits = 24,
            StencilBits = 8,
            MinorVersion = 3
        };
    }


    public virtual void Update() => DispatchEvents();

    public void Draw() => Draw(this);


    public virtual void Draw(RenderTarget target)
    {}


    public static Cursor GetCursorTypeFromCorners(Corner corner)
    {
        if (corner.HasFlag(Corner.TopLeft))
            return new Cursor(Cursor.CursorType.SizeTopLeft);

        if (corner.HasFlag(Corner.TopRight))
            return new Cursor(Cursor.CursorType.SizeTopRight);

        if (corner.HasFlag(Corner.BottomLeft))
            return new Cursor(Cursor.CursorType.SizeBottomLeft);

        if (corner.HasFlag(Corner.BottomRight))
            return new Cursor(Cursor.CursorType.SizeBottomRight);


        if (corner.HasFlag(Corner.Left) || corner.HasFlag(Corner.Right))
            return new Cursor(Cursor.CursorType.SizeHorizontal);

        if (corner.HasFlag(Corner.Top) || corner.HasFlag(Corner.Bottom))
            return new Cursor(Cursor.CursorType.SizeVertical);

        return new Cursor(Cursor.CursorType.Arrow);
    }
}
