using System;

using SFML.Graphics;


namespace Latte.Core;


public abstract class Object : IUpdateable, IDrawable
{
    public event EventHandler? UpdateEvent;
    public event EventHandler? DrawEvent;


    public void Update()
        => UpdateEvent?.Invoke(this, EventArgs.Empty);


    public void Draw(RenderTarget target)
        => DrawEvent?.Invoke(this, EventArgs.Empty);
}
