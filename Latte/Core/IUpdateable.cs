using System;


namespace Latte.Core;




public interface IUpdateable
{
    event EventHandler? UpdateEvent;




    void Update();
}
