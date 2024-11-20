using System;

using SFML.Window;
using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Core.Application;


namespace Latte.Elements;


public class MouseCornerState
{
    public bool Top { get; set; }
    public bool Bottom { get; set; }
    public bool Left { get; set; }
    public bool Right { get; set; }
    
    public bool TopLeft => Top && Left;
    public bool TopRight => Top && Right;
    public bool BottomLeft => Bottom && Left;
    public bool BottomRight => Bottom && Right;
    
    public bool Any => Top || Bottom || Left || Right;
}


public interface IResizable
{
    FloatRect Rect { get; }
    float CornerSize { get; }
    
    Vec2f? MinSize { get; }
    Vec2f? MaxSize { get; }
    
    MouseCornerState ResizeCorner { get; }
    
    bool Resizing { get; }
    bool WasResizing { get; }

    event EventHandler? ResizeBeginEvent;
    event EventHandler? ResizeEndEvent;
    event EventHandler? ResizingEvent;


    void OnResizeBegin();
    void OnResizeEnd();
    void OnResizing();


    void ProcessResizing();
}


public interface IDefaultResizable : IResizable
{
    void UpdateCornerStateProperties()
    {
        // the corners being resized should not change while resizing
        if (Resizing)
            return;
        
        Vec2f point = App.MainWindow.WorldMousePosition;
        
        FloatRect left = Rect with { Width = CornerSize };
        FloatRect right = Rect with { Left = Rect.Left + Rect.Width - CornerSize, Width = CornerSize };
        FloatRect top = Rect with { Height = CornerSize };
        FloatRect bottom = Rect with { Top = Rect.Top + Rect.Height - CornerSize, Height = CornerSize };

        ResizeCorner.Left = point.IsPointOverRect(left);
        ResizeCorner.Right = point.IsPointOverRect(right);
        ResizeCorner.Top = point.IsPointOverRect(top);
        ResizeCorner.Bottom = point.IsPointOverRect(bottom);
    }

    Cursor GetCursorTypeFromResizeCorner()
    {
        if (ResizeCorner.TopLeft)
            return new(Cursor.CursorType.SizeTopLeft);
        
        if (ResizeCorner.TopRight)
            return new(Cursor.CursorType.SizeTopRight);
        
        if (ResizeCorner.BottomLeft)
            return new(Cursor.CursorType.SizeBottomLeft);
        
        if (ResizeCorner.BottomRight)
            return new(Cursor.CursorType.SizeBottomRight);
        
     
        if (ResizeCorner.Left || ResizeCorner.Right)
            return new(Cursor.CursorType.SizeHorizontal);
        
        if (ResizeCorner.Top || ResizeCorner.Bottom)
            return new(Cursor.CursorType.SizeVertical);

        return new(Cursor.CursorType.Arrow);
    }


    void ProcessResizingEvents()
    {
        if (Resizing && !WasResizing)
            OnResizeBegin();
        
        if (!Resizing && WasResizing)
            OnResizeEnd();
        
        if (Resizing)
            OnResizing();
    }
}