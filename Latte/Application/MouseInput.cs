using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Window;

using Latte.Core;
using Latte.Core.Objects;
using Latte.Core.Type;
using Latte.UI;


namespace Latte.Application;


public class MouseButtonEventArgs(Mouse.Button button) : EventArgs
{
    public Mouse.Button Button { get; } = button;
}


public static class MouseInput
{
    private static bool s_canResetScrollDelta = true;

    private static readonly List<Mouse.Button> s_pressedButtons = [];


    public static Vec2i Position { get; private set; } = new Vec2i();
    public static Vec2i LastPosition { get; private set; } = new Vec2i();
    public static Vec2i PositionDelta => Position - LastPosition;
    public static Vec2f PositionInView { get; private set; } = new Vec2f();
    public static Vec2f LastPositionInView { get; private set; } = new Vec2f();
    public static Vec2f PositionDeltaInView => PositionInView - LastPositionInView;

    public static bool MouseMoved => Position != LastPosition;

    public static float ScrollDelta { get; private set; }

    public static BaseObject? ObjectWhichCaughtMouseInput { get; private set; }
    public static BaseObject? ClickableWhichCaughtMouseInput { get; private set; }
    public static BaseObject? TrueClickableWhichCaughtMouseInput { get; private set; }

    public static BaseObject? ObjectWhichIsHoldingMouseInput { get; private set; }

    public static event EventHandler<MouseButtonEventArgs>? ButtonDownEvent;
    public static event EventHandler<MouseButtonEventArgs>? ButtonUpEvent;
    public static event EventHandler<MouseButtonEventArgs>? DragStartEvent;
    public static event EventHandler<MouseButtonEventArgs>? DragEndEvent;
    public static event EventHandler<MouseButtonEventArgs>? DraggingEvent;


    public static void AddScrollListener(Window window)
        => window.MouseWheelScrolled += OnScroll;

    public static void RemoveScrollListener(Window window)
        => window.MouseWheelScrolled -= OnScroll;


    public static void Update()
    {
        UpdateMouseProperties();
        UpdatePressedButtons();
        UpdateDragging();
        UpdateMouseInputState();

        if (s_canResetScrollDelta)
            ScrollDelta = 0f;

        s_canResetScrollDelta = true;
    }

    private static void UpdateMouseProperties()
    {
        LastPosition = Position;
        LastPositionInView = PositionInView;

        Position = App.Window.MousePosition;
        PositionInView = App.Window.MapPixelToCoords(Position, App.Window.GetView());
    }

    private static void UpdatePressedButtons()
    {
        var currentlyPressedButtons = GetPressedButtons();

        foreach (var currentlyPressedButton in currentlyPressedButtons.ToArray())
            if (!s_pressedButtons.Contains(currentlyPressedButton))
                OnButtonDown(currentlyPressedButton);

        foreach (var pressedButton in s_pressedButtons.ToArray())
            if (!currentlyPressedButtons.Contains(pressedButton))
                OnButtonUp(pressedButton);
    }

    private static void UpdateDragging()
    {
        if (!MouseMoved)
            return;

        foreach (var pressedButton in s_pressedButtons)
            OnDragging(pressedButton);
    }


    private static List<Mouse.Button> GetPressedButtons()
    {
        var buttons = new List<Mouse.Button>();

        AddIfButtonPressed(ref buttons, Mouse.Button.Left);
        AddIfButtonPressed(ref buttons, Mouse.Button.Right);
        AddIfButtonPressed(ref buttons, Mouse.Button.Middle);
        AddIfButtonPressed(ref buttons, Mouse.Button.XButton1);
        AddIfButtonPressed(ref buttons, Mouse.Button.XButton2);

        return buttons;
    }

    private static void AddIfButtonPressed(ref List<Mouse.Button> buttons, Mouse.Button button)
    {
        if (Mouse.IsButtonPressed(button))
            buttons.Add(button);
    }

    private static void UpdateMouseInputState()
    {
        if (!CheckMouseInputHolding())
            return;

        var objects = App.Objects.ToArray();

        ClickableWhichCaughtMouseInput = TrueClickableWhichCaughtMouseInput = null;

        for (var i = objects.Length - 1; i >= 0; i--)
        {
            var @object = objects[i];
            var mouseInputWasCaught = ClickableWhichCaughtMouseInput is not null;
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
        ObjectWhichCaughtMouseInput = @object;

        if (@object is not IClickable clickable)
            return;

        if (!clickable.IgnoreMouseInput)
        {
            ClickableWhichCaughtMouseInput = @object;

            if (clickable.MouseState.IsTruePressed)
                ObjectWhichIsHoldingMouseInput = @object;
        }

        TrueClickableWhichCaughtMouseInput = @object;
    }


    private static bool IsMouseOverObject(BaseObject @object)
        => PositionInView.IsPointOverObject(@object);


    private static void OnScroll(object? _, MouseWheelScrollEventArgs args)
    {
        ScrollDelta = args.Delta;
        s_canResetScrollDelta = false;
    }


    private static void OnButtonDown(Mouse.Button button)
    {
        s_pressedButtons.Add(button);

        ButtonDownEvent?.Invoke(null, new MouseButtonEventArgs(button));

        OnDragStart(button);
    }

    private static void OnButtonUp(Mouse.Button button)
    {
        s_pressedButtons.Remove(button);

        ButtonUpEvent?.Invoke(null, new MouseButtonEventArgs(button));

        OnDragEnd(button);
    }


    private static void OnDragStart(Mouse.Button button)
        => DragStartEvent?.Invoke(null, new MouseButtonEventArgs(button));

    private static void OnDragEnd(Mouse.Button button)
        => DragEndEvent?.Invoke(null, new MouseButtonEventArgs(button));

    private static void OnDragging(Mouse.Button button)
        => DraggingEvent?.Invoke(null, new MouseButtonEventArgs(button));
}
