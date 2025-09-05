using System;

using Latte.Core;


using SfmlCursor = SFML.Window.Cursor;


namespace Latte.Application;


public class Cursor : IUpdateable
{
    // Usually, an error message "X Error of failed request: BadCursor (invalid Cursor parameter)"
    // from the X window compositor (Linux only) was being shown
    // whenever a program using Latte was closed. The error could randomly happen or not.
    // That error looks like to be the result of freeing the memory of a Cursor object after
    // setting it as the Window cursor.
    // Basically... to solve it, the below field needs to exist.

    private SfmlCursor _cursor;


    public Window Window { get; }

    public SfmlCursor SfmlCursor
    {
        get => _cursor;
        private set
        {
            _cursor = value;
            SetThisToWindow();
        }
    }

    public SfmlCursor.CursorType Type { get; set; }

    public event EventHandler? UpdateEvent;


    public Cursor(Window window)
    {
        _cursor = new SfmlCursor(SfmlCursor.CursorType.Arrow);


        Window = window;

        Type = SfmlCursor.CursorType.Arrow;
    }


    public void Update()
    {
        SfmlCursor = new SfmlCursor(Type);

        UpdateEvent?.Invoke(this, EventArgs.Empty);
    }


    private void SetThisToWindow()
        => Window.SetMouseCursor(SfmlCursor);
}
