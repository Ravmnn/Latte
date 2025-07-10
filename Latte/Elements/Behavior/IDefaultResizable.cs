using Latte.Core;
using Latte.Core.Application;


namespace Latte.Elements.Behavior;


public interface IDefaultResizable : IResizable
{
    void UpdateCornersToResize()
    {
        // the corners being resized should not change while resizing
        if (Resizing)
            return;

        var point = MouseInput.PositionInElementView;

        var left = Rect with { Width = CornerResizeAreaSize };
        var right = Rect with { Left = Rect.Left + Rect.Width - CornerResizeAreaSize, Width = CornerResizeAreaSize };
        var top = Rect with { Height = CornerResizeAreaSize };
        var bottom = Rect with { Top = Rect.Top + Rect.Height - CornerResizeAreaSize, Height = CornerResizeAreaSize };

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
