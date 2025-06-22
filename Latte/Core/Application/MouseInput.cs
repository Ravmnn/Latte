using System.Linq;

using Latte.Core.Type;
using Latte.Elements;
using Latte.Elements.Primitives;


namespace Latte.Core.Application;


public static class MouseInput
{
    private static Vec2i s_lastMousePosition;
    private static Vec2f s_lastElementViewMousePosition;
    private static Vec2f s_lastMainViewMousePosition;


    public static Vec2i Position { get; private set; }
    public static Vec2i PositionDelta => Position - s_lastMousePosition;
    public static Vec2f PositionInElementView { get; private set; }
    public static Vec2f PositionInMainView { get; private set; }
    public static Vec2f PositionDeltaInElementView => PositionInElementView - s_lastElementViewMousePosition;
    public static Vec2f PositionDeltaInMainView => PositionInMainView - s_lastMainViewMousePosition;

    public static float ScrollDelta { get; private set; }

    public static Element? ElementWhichCaughtMouseInput { get; private set; }
    public static Element? TrueElementWhichCaughtMouseInput { get; private set; }

    public static IClickable? ElementWhichIsHoldingMouseInput { get; private set; }


    static MouseInput()
    {
        s_lastMousePosition = new Vec2i();
        s_lastElementViewMousePosition = new Vec2f();
        s_lastMainViewMousePosition = new Vec2f();

        Position = new Vec2i();
        PositionInElementView = new Vec2f();
        PositionInMainView = new Vec2f();
    }


    public static void AddScrollListener(Window window)
        => window.MouseWheelScrolled += (_, args) => ScrollDelta = args.Delta;


    public static void Update()
    {
        UpdateMouseProperties();
        UpdateMouseInputState();

        ScrollDelta = 0f;
    }

    private static void UpdateMouseProperties()
    {
        s_lastMousePosition = Position;
        s_lastElementViewMousePosition = PositionInElementView;
        s_lastMainViewMousePosition = PositionInMainView;

        Position = App.Window.MousePosition;
        PositionInElementView = App.Window.MapPixelToCoords(Position, App.ElementView);
        PositionInMainView = App.Window.MapPixelToCoords(Position, App.MainView);
    }

    private static void UpdateMouseInputState()
    {
        if (!CheckMouseInputHolding())
            return;

        Element[] elements = App.Elements.ToArray();

        ElementWhichCaughtMouseInput = TrueElementWhichCaughtMouseInput = null;

        for (var i = elements.Length - 1; i >= 0; i--)
        {
            var element = elements[i];
            var mouseInputWasCaught = ElementWhichCaughtMouseInput is not null;
            var isMouseOver = IsMouseOverElement(element);

            if (element is IClickable clickable)
                clickable.CaughtMouseInput = !mouseInputWasCaught && isMouseOver;

            if (!mouseInputWasCaught && element.Visible && isMouseOver)
                SetElementWhichCaughtMouseInput(element);
        }
    }

    private static bool CheckMouseInputHolding()
    {
        if (ElementWhichIsHoldingMouseInput is null)
            return true;

        if (!ElementWhichIsHoldingMouseInput.MouseState.IsMouseDown)
            ElementWhichIsHoldingMouseInput = null;
        else
            return false;

        return true;
    }

    private static void SetElementWhichCaughtMouseInput(Element element)
    {
        if (!element.IgnoreMouseInput)
        {
            ElementWhichCaughtMouseInput = element;

            if (element is IClickable { MouseState.IsTruePressed: true } clickable)
                ElementWhichIsHoldingMouseInput = clickable;
        }

        TrueElementWhichCaughtMouseInput = element;
    }


    public static bool IsMouseOverElement(Element element)
        => (element as IClickable)?.IsPointOver(PositionInElementView) ?? element.IsPointOverBounds(PositionInElementView);

    public static bool IsMouseOverAnyElement() => ElementWhichCaughtMouseInput is not null;
}
