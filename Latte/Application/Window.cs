using System;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Rendering;


namespace Latte.Application;




public class Window : RenderWindow, IUpdateable
{
    public DefaultRenderer Renderer { get; set; }

    public Cursor Cursor { get; set; }


    public Vec2i MousePosition => Mouse.GetPosition(this);
    public Vec2f ViewMousePosition => MapPixelToCoords(MousePosition);

    public IntRect WindowRect => new IntRect(new Vector2i(0, 0), (Vector2i)Size);




    public event EventHandler? UpdateEvent;




    public Window(VideoMode mode, string title, Styles style, ContextSettings settings)
        : base(mode, title, style, settings)
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
}
