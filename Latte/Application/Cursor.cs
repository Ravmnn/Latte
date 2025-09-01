using System;

using Latte.Core;


using SfmlCursor = SFML.Window.Cursor;


namespace Latte.Application;


public class Cursor : IUpdateable
{
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
