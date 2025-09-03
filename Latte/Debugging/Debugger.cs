using System;

using SFML.Window;
using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Application;
using Latte.Application.Elements.Attributes;
using Latte.Application.Elements.Behavior;
using Latte.Application.Elements.Primitives;
using Latte.Debugging.Elements;


namespace Latte.Debugging;


[Flags]
public enum DebugOption
{
    None,

    Clip = 1 << 0,
    OnlyHoveredObject = 1 << 1,
    OnlyVisibleObjects = 1 << 2,

    ShowBounds = 1 << 3,
    ShowBoundsDimensionsAndPosition = 1 << 4,
    ShowPriority = 1 << 5,
    ShowFocus = 1 << 6
}


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowBoundsAttribute : ElementAttribute;


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowBoundsDimensionsAndPositionAttribute : ElementAttribute;


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowPriorityAttribute : ElementAttribute;


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowFocusAttribute : ElementAttribute;


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreInspection : ElementAttribute;


public sealed class Debugger : IUpdateable, IDrawable
{
    public InspectionWindow InspectionWindow { get; }
    public AppStateWindow AppStateWindow { get; }

    public DebugOption Options { get; set; }
    public bool EnableKeyShortcuts { get; set; }

    public event EventHandler? UpdateEvent;
    public event EventHandler? DrawEvent;


    public Debugger()
    {
        InspectionWindow = new InspectionWindow { Visible = false };
        AppStateWindow = new AppStateWindow { Visible = false };

        Options = DebugOption.OnlyVisibleObjects;

        App.AddElement(InspectionWindow);
        App.AddElement(AppStateWindow);
    }


    public void Update()
    {
        ProcessDebugShortcuts();

        UpdateEvent?.Invoke(this, EventArgs.Empty);
    }

    private void ProcessDebugShortcuts()
    {
        if (!EnableKeyShortcuts || KeyboardInput.PressedKeyCode is null)
            return;

        switch (KeyboardInput.PressedKeyCode)
        {
            case Keyboard.Scancode.Escape:
                if (KeyboardInput.PressedKey?.Shift ?? false)
                    Options = DebugOption.None;

                break;

            case Keyboard.Scancode.F1:
                ToggleDebugOption(DebugOption.Clip);
                break;

            case Keyboard.Scancode.F2:
                ToggleDebugOption(DebugOption.OnlyHoveredObject);
                break;

            case Keyboard.Scancode.F3:
                ToggleDebugOption(DebugOption.OnlyVisibleObjects);
                break;


            case Keyboard.Scancode.F4:
                ToggleDebugOption(DebugOption.ShowBounds);
                break;

            case Keyboard.Scancode.F5:
                ToggleDebugOption(DebugOption.ShowBoundsDimensionsAndPosition);
                break;

            case Keyboard.Scancode.F6:
                ToggleDebugOption(DebugOption.ShowPriority);
                break;

            case Keyboard.Scancode.F7:
                ToggleDebugOption(DebugOption.ShowFocus);
                break;

            case Keyboard.Scancode.F9:
                InspectionWindow.LockAtObject = InspectionWindow.LockAtObject is null
                    ? MouseInput.TrueObjectWhichCaughtMouseInput : null;

                break;

            case Keyboard.Scancode.F10:
                ToggleDebugWindowsVisibility();
                break;
        }
    }

    private void ToggleDebugOption(DebugOption option)
    {
        if (Options.HasFlag(option))
            Options &= ~option;
        else
            Options |= option;
    }


    private void ToggleDebugWindowsVisibility()
    {
        InspectionWindow.Visible = !InspectionWindow.Visible;
        AppStateWindow.Visible = !AppStateWindow.Visible;
    }


    public void Draw(RenderTarget target)
    {
        if (Options == DebugOption.None)
            return;

        foreach (var @object in App.Objects)
            DebugObject(target, @object);

        DrawEvent?.Invoke(this, EventArgs.Empty);
    }

    public void DebugObject(RenderTarget target, BaseObject @object)
    {
        var onlyHoveredObject = Options.HasFlag(DebugOption.OnlyHoveredObject);
        var onlyVisibleObjects = Options.HasFlag(DebugOption.OnlyVisibleObjects);

        if (onlyHoveredObject && @object != MouseInput.ObjectWhichCaughtMouseInput)
            return;

        if (onlyVisibleObjects && !@object.CanDraw)
            return;

        var clip = Options.HasFlag(DebugOption.Clip);

        if (clip && @object is Element element)
        {
            Clipping.ClipEnable();
            Clipping.SetClipToParents(target, element);
        }

        if (Options.HasFlag(DebugOption.ShowBounds) && !@object.HasCachedObjectAttribute<DebuggerIgnoreShowBoundsAttribute>())
            DrawObjectBounds(target, @object, Color.Red);

        if (Options.HasFlag(DebugOption.ShowBoundsDimensionsAndPosition) && !@object.HasCachedObjectAttribute<DebuggerIgnoreShowBoundsDimensionsAndPositionAttribute>())
            DrawObjectBoundsDimensionsAndPosition(target, @object);

        if (Options.HasFlag(DebugOption.ShowPriority) && !@object.HasCachedObjectAttribute<DebuggerIgnoreShowPriorityAttribute>())
            DrawObjectPriority(target, @object);

        if (Options.HasFlag(DebugOption.ShowFocus) && !@object.HasCachedObjectAttribute<DebuggerIgnoreShowFocusAttribute>())
            if (@object is IFocusable { Focused: true })
                DrawFocusIndicator(target, @object);

        if (clip)
            Clipping.ClipDisable();
    }


    public static void DrawObjectBounds(RenderTarget target, BaseObject @object, ColorRGBA color)
        => Debugging.Draw.LineRect(target, @object.GetBounds(), color);

    public static void DrawObjectBoundsDimensionsAndPosition(RenderTarget target, BaseObject @object)
    {
        var bounds = @object.GetBounds();

        var backgroundColor = new Color(255, 255, 255, 220);

        var x = $"{bounds.Left:F1}";
        var y = $"{bounds.Top:F1}";
        var width = $"{bounds.Width:F1}";
        var height = $"{bounds.Height:F1}";

        Debugging.Draw.Text(target, bounds with { Left = bounds.Left - 40, Width = 10 }, Alignment.TopLeft, x, backgroundColor: backgroundColor);
        Debugging.Draw.Text(target, bounds with { Top = bounds.Top - 20, Height = 10 }, Alignment.TopLeft, y, backgroundColor: backgroundColor);
        Debugging.Draw.Text(target, bounds with { Top = bounds.Top + bounds.Height + 10 }, Alignment.HorizontalCenter | Alignment.Top, width, backgroundColor: backgroundColor);
        Debugging.Draw.Text(target, bounds with { Left = bounds.Left + bounds.Width + 10 }, Alignment.VerticalCenter | Alignment.Left, height, backgroundColor: backgroundColor);
    }

    public static void DrawObjectPriority(RenderTarget target, BaseObject @object)
    {
        var absolutePriority = (uint)Math.Abs(@object.Priority);

        Debugging.Draw.Rect(target, @object.GetBounds(), ColorGenerator.FromIndex(absolutePriority, 50));
        Debugging.Draw.Text(target, @object.GetBounds(), Alignment.Center, @object.Priority.ToString(), backgroundColor: Color.White);
    }

    public static void DrawFocusIndicator(RenderTarget target, BaseObject @object)
        => Debugging.Draw.LineRect(target, @object.GetBounds(), Color.Magenta, 3f);
}
