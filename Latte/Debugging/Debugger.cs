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
    OnlyHoveredElement = 1 << 1,
    OnlyTrueHoveredElement = 1 << 2,

    ShowBounds = 1 << 3,
    ShowBoundsDimensionsAndPosition = 1 << 4,
    ShowClipArea = 1 << 5,
    ShowPriority = 1 << 6,
    ShowFocus = 1 << 7
}


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowBoundsAttribute(bool inherit = true) : ElementAttribute(inherit);


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowBoundsDimensionsAndPositionAttribute(bool inherit = true) : ElementAttribute(inherit);


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowClipAreaAttribute(bool inherit = true) : ElementAttribute(inherit);


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowPriorityAttribute(bool inherit = true) : ElementAttribute(inherit);


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowFocusAttribute(bool inherit = true) : ElementAttribute(inherit);


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreInspection(bool inherit = true) : ElementAttribute(inherit);


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

        Options = DebugOption.None;

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
                ToggleDebugOption(DebugOption.OnlyHoveredElement);
                break;

            case Keyboard.Scancode.F3:
                ToggleDebugOption(DebugOption.OnlyTrueHoveredElement);
                break;


            case Keyboard.Scancode.F4:
                ToggleDebugOption(DebugOption.ShowBounds);
                break;

            case Keyboard.Scancode.F5:
                ToggleDebugOption(DebugOption.ShowBoundsDimensionsAndPosition);
                break;

            case Keyboard.Scancode.F6:
                ToggleDebugOption(DebugOption.ShowClipArea);
                break;

            case Keyboard.Scancode.F7:
                ToggleDebugOption(DebugOption.ShowPriority);
                break;

            case Keyboard.Scancode.F8:
                ToggleDebugOption(DebugOption.ShowFocus);
                break;

            case Keyboard.Scancode.F9:
                InspectionWindow.LockAtElement = InspectionWindow.LockAtElement is null
                    ? MouseInput.TrueElementWhichCaughtMouseInput : null;

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

        foreach (var element in App.Elements)
            DebugElement(target, element);

        DrawEvent?.Invoke(this, EventArgs.Empty);
    }

    public void DebugElement(RenderTarget target, Element element)
    {
        if (Options.HasFlag(DebugOption.OnlyHoveredElement) && element != MouseInput.ElementWhichCaughtMouseInput)
            return;

        if (Options.HasFlag(DebugOption.OnlyTrueHoveredElement) && element != MouseInput.TrueElementWhichCaughtMouseInput)
            return;

        var clip = Options.HasFlag(DebugOption.Clip);

        if (clip)
        {
            Clipping.ClipEnable();
            Clipping.SetClipToParents(target, element);
        }

        if (Options.HasFlag(DebugOption.ShowBounds) && !element.HasCachedElementAttribute<DebuggerIgnoreShowBoundsAttribute>())
            DrawElementBounds(target, element, Color.Red);

        if (Options.HasFlag(DebugOption.ShowBoundsDimensionsAndPosition) && !element.HasCachedElementAttribute<DebuggerIgnoreShowBoundsDimensionsAndPositionAttribute>())
            DrawElementBoundsDimensionsAndPosition(target, element);

        if (Options.HasFlag(DebugOption.ShowClipArea) && !element.HasCachedElementAttribute<DebuggerIgnoreShowClipAreaAttribute>())
            DrawElementClipArea(target, element);

        if (Options.HasFlag(DebugOption.ShowPriority) && !element.HasCachedElementAttribute<DebuggerIgnoreShowPriorityAttribute>())
            DrawElementPriority(target, element);

        if (Options.HasFlag(DebugOption.ShowFocus) && !element.HasCachedElementAttribute<DebuggerIgnoreShowFocusAttribute>())
            if (element is IFocusable { Focused: true })
                DrawFocusIndicator(target, element);

        if (clip)
            Clipping.ClipDisable();
    }


    public static void DrawElementBounds(RenderTarget target, Element element, ColorRGBA color)
        => Debugging.Draw.LineRect(target, element.GetBounds(), color);

    public static void DrawElementBoundsDimensionsAndPosition(RenderTarget target, Element element)
    {
        var bounds = element.GetBounds();
        var borderLessBounds = element.GetBorderLessBounds();

        var backgroundColor = new Color(255, 255, 255, 220);

        var x = $"{bounds.Left:F1}";
        var y = $"{bounds.Top:F1}";
        var width = $"{bounds.Width:F1}";
        var height = $"{bounds.Height:F1}";

        Debugging.Draw.Text(target, borderLessBounds with { Left = borderLessBounds.Left - 40, Width = 10 }, Alignment.TopLeft, x, backgroundColor: backgroundColor);
        Debugging.Draw.Text(target, borderLessBounds with { Top = borderLessBounds.Top - 20, Height = 10 }, Alignment.TopLeft, y, backgroundColor: backgroundColor);
        Debugging.Draw.Text(target, borderLessBounds with { Top = borderLessBounds.Top + borderLessBounds.Height + 10 }, Alignment.HorizontalCenter | Alignment.Top, width, backgroundColor: backgroundColor);
        Debugging.Draw.Text(target, borderLessBounds with { Left = borderLessBounds.Left + borderLessBounds.Width + 10 }, Alignment.VerticalCenter | Alignment.Left, height, backgroundColor: backgroundColor);
    }

    public static void DrawElementClipArea(RenderTarget target, Element element)
        => Debugging.Draw.LineRect(target, (FloatRect)element.GetClipArea(), Color.Blue);

    public static void DrawElementPriority(RenderTarget target, Element element)
    {
        var absolutePriority = (uint)System.Math.Abs(element.Priority);

        Debugging.Draw.Rect(target, element.GetBounds(), ColorGenerator.FromIndex(absolutePriority, 50));
        Debugging.Draw.Text(target, element.GetBorderLessBounds(), Alignment.Center, element.Priority.ToString(), backgroundColor: Color.White);
    }

    public static void DrawFocusIndicator(RenderTarget target, Element element)
        => Debugging.Draw.LineRect(target, element.GetBounds(), Color.Magenta, 3f);
}
