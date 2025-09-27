using System;

using Latte.Core;


using SfmlCursor = SFML.Window.Cursor;


namespace Latte.Application;




public class Cursor(Window window) : IUpdateable
{
    // Usually, an error message "X Error of failed request: BadCursor (invalid Cursor parameter)"
    // from the X window compositor (Linux only) was being shown
    // whenever a program using Latte was closed. The error could randomly happen or not.
    // That error looks like to be the result of freeing the memory of a Cursor object after
    // setting it as the Window cursor.
    // Basically... to solve it, the below field needs to exist.

    private SfmlCursor _cursor = new SfmlCursor(SfmlCursor.CursorType.Arrow);




    public Window Window { get; } = window;

    public SfmlCursor SfmlCursor
    {
        get => _cursor;
        private set
        {
            _cursor = value;
            SetThisToWindow();
        }
    }

    public SfmlCursor.CursorType Type { get; set; } = SfmlCursor.CursorType.Arrow;




    public event EventHandler? UpdateEvent;




    public void Update()
    {
        SfmlCursor = new SfmlCursor(Type);

        UpdateEvent?.Invoke(this, EventArgs.Empty);
    }




    private void SetThisToWindow()
        => Window.SetMouseCursor(SfmlCursor);




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
