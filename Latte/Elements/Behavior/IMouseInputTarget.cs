namespace Latte.Elements.Behavior;


public interface IMouseInputTarget
{
    bool IgnoreMouseInput { get; set; }
    bool CaughtMouseInput { get; set; }
}
