using System;

using SFML.Graphics;


namespace Latte.Core;


public interface IDrawable
{
    event EventHandler? DrawEvent;

    void Draw(RenderTarget target);
}
