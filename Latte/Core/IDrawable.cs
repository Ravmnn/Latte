using System;


namespace Latte.Core;




public interface IDrawable
{
    event EventHandler? DrawEvent;




    void Draw(IRenderer target);
}
