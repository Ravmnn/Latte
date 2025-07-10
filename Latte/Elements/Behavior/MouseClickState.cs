namespace Latte.Elements.Behavior;


public class MouseClickState
{
    public bool IsMouseOver { get; set; }
    public bool IsMouseHover { get; set; }
    public bool IsMouseDown { get; set; }
    public bool IsPressed { get; set; }
    public bool IsTruePressed { get; set; }

    // I couldn't find a better name for it.
    // "IsTruePressed" is true if "IsPressed" and the mouse wasn't down (mouse button pressed)
    // the last iteration.

    // Basically, it means that something is pressed and that the press state
    // started with the mouse inside of that thing. It's important to know that because, if not,
    // a button would be considered as pressed if the mouse just entered it with the left button down.
    // Complicated.

    public bool WasMouseOver { get; set; }
    public bool WasMouseHover { get; set; }
    public bool WasMouseDown { get; set; }
    public bool WasPressed { get; set; }
    public bool WasTruePressed { get; set; }
}
