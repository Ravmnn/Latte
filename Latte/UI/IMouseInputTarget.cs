namespace Latte.UI;




public interface IMouseInputTarget
{
    bool IgnoreMouseInput { get; set; }
    bool CaughtMouseInput { get; set; }
}
