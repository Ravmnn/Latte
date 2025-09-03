using System.Linq;

using Latte.Core;
using Latte.Core.Type;
using Latte.Application.Elements.Behavior;


namespace Latte.Application;


public static class MouseInput
{
    private static Vec2i s_lastMousePosition;
    private static Vec2f s_lastObjectViewMousePosition;
    private static Vec2f s_lastMainViewMousePosition;


    public static Vec2i Position { get; private set; }
    public static Vec2i PositionDelta => Position - s_lastMousePosition;
    public static Vec2f PositionInObjectView { get; private set; }
    public static Vec2f PositionInMainView { get; private set; }
    public static Vec2f PositionDeltaInObjectView => PositionInObjectView - s_lastObjectViewMousePosition;
    public static Vec2f PositionDeltaInMainView => PositionInMainView - s_lastMainViewMousePosition;

    public static float ScrollDelta { get; private set; }

    public static BaseObject? ObjectWhichCaughtMouseInput { get; private set; }
    public static BaseObject? TrueObjectWhichCaughtMouseInput { get; private set; }

    public static BaseObject? ObjectWhichIsHoldingMouseInput { get; private set; }


    static MouseInput()
    {
        s_lastMousePosition = new Vec2i();
        s_lastObjectViewMousePosition = new Vec2f();
        s_lastMainViewMousePosition = new Vec2f();

        Position = new Vec2i();
        PositionInObjectView = new Vec2f();
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
        s_lastObjectViewMousePosition = PositionInObjectView;
        s_lastMainViewMousePosition = PositionInMainView;

        Position = App.Window.MousePosition;
        PositionInObjectView = App.Window.MapPixelToCoords(Position, App.ObjectView);
        PositionInMainView = App.Window.MapPixelToCoords(Position, App.MainView);
    }

    private static void UpdateMouseInputState()
    {
        if (!CheckMouseInputHolding())
            return;

        var objects = App.Objects.ToArray();

        ObjectWhichCaughtMouseInput = TrueObjectWhichCaughtMouseInput = null;

        for (var i = objects.Length - 1; i >= 0; i--)
        {
            var @object = objects[i];
            var mouseInputWasCaught = ObjectWhichCaughtMouseInput is not null;
            var isMouseOver = IsMouseOverObject(@object);

            if (@object is IClickable clickable)
                clickable.CaughtMouseInput = !mouseInputWasCaught && isMouseOver;

            if (!mouseInputWasCaught && @object.CanDraw && isMouseOver)
                SetObjectWhichCaughtMouseInput(@object);
        }
    }

    private static bool CheckMouseInputHolding()
    {
        if (ObjectWhichIsHoldingMouseInput is not IClickable clickableWhichIsHoldingMouseInput)
            return true;

        if (!clickableWhichIsHoldingMouseInput.MouseState.IsMouseDown)
            ObjectWhichIsHoldingMouseInput = null;
        else
            return false;

        return true;
    }

    private static void SetObjectWhichCaughtMouseInput(BaseObject @object)
    {
        if (@object is not IClickable clickable)
            return;

        if (!clickable.IgnoreMouseInput)
        {
            ObjectWhichCaughtMouseInput = @object;

            if (clickable.MouseState.IsTruePressed)
                ObjectWhichIsHoldingMouseInput = @object;
        }

        TrueObjectWhichCaughtMouseInput = @object;
    }


    public static bool IsMouseOverObject(BaseObject @object)
        => PositionInObjectView.IsPointOverObject(@object);

    public static bool IsMouseOverAnyObject() => ObjectWhichCaughtMouseInput is not null;
}
