using System;
using Latte.Core.Application;
using SFML.System;
using SFML.Window;
using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Core;


// TODO: add scrollable areas
// TODO: add text inputs


public class Window : RenderWindow, IUpdateable, IDrawable
{
    // Usually, an error message "X Error of failed request:  BadCursor (invalid Cursor parameter)"
    // from the X window compositor (Linux only) was being shown
    // whenever a program using Latte was closed. The error could randomly happen or not.
    // That error looks like to be the result of freeing the memory of a Cursor object after
    // setting it as the Window cursor.
    // Basically... to solve it, the below field needs to exist.

    private Cursor _cursor;


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

    public IntRect WindowRect => new(new(0, 0), (Vector2i)Size);

    public event EventHandler? ClosedEvent;


    public Window(VideoMode mode, string title, Styles style = Styles.Default, ContextSettings settings = new()) : base(mode, title, style, settings)
    {
        Closed += (_, _) => Close();
    }


    public virtual void Update()
    {
        DispatchEvents();
    }


    public void Draw() => Draw(this);

    public virtual void Draw(RenderTarget target)
    {}


    public override void Close()
    {
        ClosedEvent?.Invoke(this, EventArgs.Empty);
        base.Close();
    }


    public static Cursor GetCursorTypeFromCorners(Corner corner)
    {
        if (corner.HasFlag(Corner.TopLeft))
            return new(Cursor.CursorType.SizeTopLeft);

        if (corner.HasFlag(Corner.TopRight))
            return new(Cursor.CursorType.SizeTopRight);

        if (corner.HasFlag(Corner.BottomLeft))
            return new(Cursor.CursorType.SizeBottomLeft);

        if (corner.HasFlag(Corner.BottomRight))
            return new(Cursor.CursorType.SizeBottomRight);


        if (corner.HasFlag(Corner.Left) || corner.HasFlag(Corner.Right))
            return new(Cursor.CursorType.SizeHorizontal);

        if (corner.HasFlag(Corner.Top) || corner.HasFlag(Corner.Bottom))
            return new(Cursor.CursorType.SizeVertical);

        return new(Cursor.CursorType.Arrow);
    }
}
