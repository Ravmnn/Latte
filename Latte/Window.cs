using System;

using SFML.System;
using SFML.Window;
using SFML.Graphics;


namespace Latte;


// TODO: add element clipping (OpenGL).
// TODO: add windows to UI
// TODO: add checkbox
// TODO: add text inputs
// TODO: add layouts (horizontal, vertical, grid...)

// TODO: work on the game only after finishing all these stuff... good luck :)


public class Window : RenderWindow
{
    public Vector2i MousePosition => Mouse.GetPosition(this);
    public Vector2f WorldMousePosition => MapPixelToCoords(MousePosition);
    
    public View View { get; set; }
    public IntRect RectSize => new(new(0, 0), (Vector2i)Size);

    public event EventHandler? ClosedEvent;
    
    
    public Window(VideoMode mode, string title, Styles style = Styles.Default, ContextSettings settings = new()) : base(mode, title, style, settings)
    {
        View = GetView();
     
        Closed += (_, _) => Close();
        Resized += (_, args) => ResizeViewToFitScreenSize(new(args.Width, args.Height));
        
        SetVerticalSyncEnabled(true);
    }


    public void ProcessEvents()
    {
        DispatchEvents();
        SetView(View);
    }


    public override void Close()
    {
        ClosedEvent?.Invoke(this, EventArgs.Empty);
        base.Close();
        
        Environment.Exit(0); // force exit
    }
    
    
    private void ResizeViewToFitScreenSize(Vector2u newSize)
    {
        View.Size = (Vector2f)newSize;
    }
}