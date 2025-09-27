using System;

using Latte.Rendering;


namespace Latte.Core;




public interface IDrawable
{
    event EventHandler? DrawEvent;




    void Draw(IRenderer target);
}
