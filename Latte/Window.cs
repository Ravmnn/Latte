using System;

using SFML.System;
using SFML.Window;
using SFML.Graphics;


namespace Latte;


// TODO: add windows to UI
// TODO: add checkbox
// TODO: add text inputs
// TODO: add layouts (horizontal, vertical, grid...)
// TODO: add animations


public class Window : RenderWindow
{
    public Vector2i MousePosition => Mouse.GetPosition(this);
    public Vector2f WorldMousePosition => MapPixelToCoords(MousePosition);
    
    public IntRect RectSize => new(new(0, 0), (Vector2i)Size);

    public event EventHandler? ClosedEvent;
    
    
    public Window(VideoMode mode, string title, Styles style = Styles.Default, ContextSettings settings = new()) : base(mode, title, style, settings)
    {
        Closed += (_, _) => Close();
        
        SetVerticalSyncEnabled(true);
    }


    public void ProcessEvents()
    {
        DispatchEvents();
    }


    public override void Close()
    {
        ClosedEvent?.Invoke(this, EventArgs.Empty);
        base.Close();
        
        Environment.Exit(0); // force exit
    }
}