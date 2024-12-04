using System;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using Latte.Core.Type;


namespace Latte.Core;


// TODO: add AreaElement (like tabs)
// TODO: add scrollable areas
// TODO: add checkbox
// TODO: add text inputs


public class Window : RenderWindow, IUpdateable, IDrawable
{
    public Vec2i MousePosition => Mouse.GetPosition(this);
    public Vec2f ViewMousePosition => MapPixelToCoords(MousePosition);
    
    public IntRect WindowRect => new(new(0, 0), (Vector2i)Size);

    public event EventHandler? CloseEvent;
    
    
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
    {
        
    }


    public override void Close()
    {
        CloseEvent?.Invoke(this, EventArgs.Empty);
        base.Close();
        
        Environment.Exit(0); // force exit
    }
    
    
    public static Cursor GetCursorTypeFromCorners(Corners corners)
    {
        if (corners.HasFlag(Corners.TopLeft))
            return new(Cursor.CursorType.SizeTopLeft);
        
        if (corners.HasFlag(Corners.TopRight))
            return new(Cursor.CursorType.SizeTopRight);
        
        if (corners.HasFlag(Corners.BottomLeft))
            return new(Cursor.CursorType.SizeBottomLeft);
        
        if (corners.HasFlag(Corners.BottomRight))
            return new(Cursor.CursorType.SizeBottomRight);
        
     
        if (corners.HasFlag(Corners.Left) || corners.HasFlag(Corners.Right))
            return new(Cursor.CursorType.SizeHorizontal);
        
        if (corners.HasFlag(Corners.Top) || corners.HasFlag(Corners.Bottom))
            return new(Cursor.CursorType.SizeVertical);

        return new(Cursor.CursorType.Arrow);
    }
}