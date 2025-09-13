using System;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;


using SfmlCursor = SFML.Window.Cursor;


namespace Latte.Application;


public class Window : RenderWindow, IUpdateable
{
    public DefaultRenderer Renderer { get; set; }

    public Cursor Cursor { get; set; }

    public Vec2i MousePosition => Mouse.GetPosition(this);
    public Vec2f ViewMousePosition => MapPixelToCoords(MousePosition);

    public IntRect WindowRect => new IntRect(new Vector2i(0, 0), (Vector2i)Size);

    public event EventHandler? UpdateEvent;


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
        Renderer = new DefaultRenderer(this);

        Cursor = new Cursor(this);

        SetFramerateLimit(60);
    }


    public virtual void Update()
    {
        DispatchEvents();
        Cursor.Update();

        UpdateEvent?.Invoke(this, EventArgs.Empty);
    }


    // TODO: move to Cursor class
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
