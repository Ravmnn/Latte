using SFML.System;
using SFML.Window;
using SFML.Graphics;

using Latte.Core.Type;


using SfmlCursor = SFML.Window.Cursor;


namespace Latte.Core;


public class Window : RenderWindow, IUpdateable, IDrawable
{
    // Usually, an error message "X Error of failed request:  BadCursor (invalid Cursor parameter)"
    // from the X window compositor (Linux only) was being shown
    // whenever a program using Latte was closed. The error could randomly happen or not.
    // That error looks like to be the result of freeing the memory of a Cursor object after
    // setting it as the Window cursor.
    // Basically... to solve it, the below field needs to exist.

    public Cursor Cursor { get; set; }

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


    public Window(VideoMode mode, string title, Styles style = Styles.Default, ContextSettings? settings = null)
        : base(mode, title, style, settings ?? DefaultSettings)
    {
        Cursor = new Cursor(this);
    }


    public virtual void Update()
    {
        DispatchEvents();
        Cursor.Update();
    }

    public void Draw() => Draw(this);


    public virtual void Draw(RenderTarget target)
    {}


    public static SfmlCursor.CursorType? GetCursorTypeFromCorners(Corner corner)
    {
        if (corner.HasFlag(Corner.TopLeft))
            return SfmlCursor.CursorType.SizeTopLeft;

        if (corner.HasFlag(Corner.TopRight))
            return SfmlCursor.CursorType.SizeTopRight;

        if (corner.HasFlag(Corner.BottomLeft))
            return SfmlCursor.CursorType.SizeBottomLeft;

        if (corner.HasFlag(Corner.BottomRight))
            return SfmlCursor.CursorType.SizeBottomRight;


        if (corner.HasFlag(Corner.Left) || corner.HasFlag(Corner.Right))
            return SfmlCursor.CursorType.SizeHorizontal;

        if (corner.HasFlag(Corner.Top) || corner.HasFlag(Corner.Bottom))
            return SfmlCursor.CursorType.SizeVertical;

        return null;
    }
}
