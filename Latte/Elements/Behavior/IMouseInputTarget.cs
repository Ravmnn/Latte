namespace Latte.Elements;


public interface IMouseInputTarget
{
    bool IgnoreMouseInput { get; set; }
    bool CaughtMouseInput { get; set; }
}
