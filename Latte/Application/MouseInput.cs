using System.Linq;

using Latte.Core;
using Latte.Core.Type;
using Latte.UI;


namespace Latte.Application;


public static class MouseInput
{
    private static Vec2i s_lastMousePosition;
    private static Vec2f s_lastViewMousePosition;
    private static bool s_canResetScrollDelta;


    public static Vec2i Position { get; private set; }
    public static Vec2i PositionDelta => Position - s_lastMousePosition;
    public static Vec2f PositionInView { get; private set; }
    public static Vec2f PositionDeltaInView => PositionInView - s_lastViewMousePosition;

    public static float ScrollDelta { get; private set; }

    public static BaseObject? ObjectWhichCaughtMouseInput { get; private set; }
    public static BaseObject? TrueObjectWhichCaughtMouseInput { get; private set; }

    public static BaseObject? ObjectWhichIsHoldingMouseInput { get; private set; }


    static MouseInput()
    {
        s_lastMousePosition = new Vec2i();
        s_lastViewMousePosition = new Vec2f();
        s_canResetScrollDelta = true;


        Position = new Vec2i();
        PositionInView = new Vec2f();
    }


    public static void AddScrollListener(Window window)
        => window.MouseWheelScrolled += (_, args) =>
        {
            ScrollDelta = args.Delta;
            s_canResetScrollDelta = false;
        };


    public static void Update()
    {
        UpdateMouseProperties();
        UpdateMouseInputState();

        if (s_canResetScrollDelta)
            ScrollDelta = 0f;

        s_canResetScrollDelta = true;
    }

    private static void UpdateMouseProperties()
    {
        s_lastMousePosition = Position;
        s_lastViewMousePosition = PositionInView;

        Position = App.Window.MousePosition;
        PositionInView = App.Window.MapPixelToCoords(Position, App.Window.GetView());
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
        => PositionInView.IsPointOverObject(@object);

    public static bool IsMouseOverAnyObject() => ObjectWhichCaughtMouseInput is not null;
}
