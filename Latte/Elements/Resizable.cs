using System;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Core.Application;


namespace Latte.Elements;


public interface IResizable
{
    FloatRect Rect { get; }
    float CornerResizeAreaSize { get; }
    
    Corner CornerToResize { get; set; }
    
    Vec2f? MinSize { get; }
    Vec2f? MaxSize { get; }
    
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
    void UpdateCornersToResize()
    {
        // the corners being resized should not change while resizing
        if (Resizing)
            return;
        
        Vec2f point = App.Window.ViewMousePosition;
        
        FloatRect left = Rect with { Width = CornerResizeAreaSize };
        FloatRect right = Rect with { Left = Rect.Left + Rect.Width - CornerResizeAreaSize, Width = CornerResizeAreaSize };
        FloatRect top = Rect with { Height = CornerResizeAreaSize };
        FloatRect bottom = Rect with { Top = Rect.Top + Rect.Height - CornerResizeAreaSize, Height = CornerResizeAreaSize };

        CornerToResize = Corner.None;
        
        CornerToResize |= point.IsPointOverRect(left) ? Corner.Left : Corner.None;
        CornerToResize |= point.IsPointOverRect(right) ? Corner.Right : Corner.None;
        CornerToResize |= point.IsPointOverRect(top) ? Corner.Top : Corner.None;
        CornerToResize |= point.IsPointOverRect(bottom) ? Corner.Bottom : Corner.None;
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