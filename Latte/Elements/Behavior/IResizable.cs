using System;

using Latte.Core;
using Latte.Core.Type;

using SFML.Graphics;


namespace Latte.Elements.Behavior;


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
