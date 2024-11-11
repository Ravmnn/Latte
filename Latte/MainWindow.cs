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


public class MainWindow : RenderWindow
{
    public static MainWindow? Current { get; private set; }
    
    public Vector2i MousePosition => Mouse.GetPosition(this);
    public Vector2f WorldMousePosition => MapPixelToCoords(MousePosition);
    
    public View View { get; set; }

    public event EventHandler? ClosedEvent;
    
    
    public MainWindow(VideoMode mode, string title, Styles style = Styles.Default, ContextSettings settings = new()) : base(mode, title, style, settings)
    {
        // TODO: maybe add support to multiple windows?
        Current = this;
        
        View = GetView();
     
        Closed += (_, _) => Close();
        Resized += (_, args) => ResizeViewToFitScreenSize(new(args.Width, args.Height));
        
        SetVerticalSyncEnabled(true);
    }


    public void Update()
    {
        DispatchEvents();

        SetView(View); // update view after area
    }
    
    
    public void Draw()
    {
        Clear();
        
        Display();
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