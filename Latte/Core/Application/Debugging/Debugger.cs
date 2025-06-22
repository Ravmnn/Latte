using System;
using System.Globalization;

using SFML.Window;
using SFML.Graphics;

using Latte.Core.Type;
using Latte.Core.Application.Debugging.Inspection;
using Latte.Elements;
using Latte.Elements.Primitives;


namespace Latte.Core.Application.Debugging;


[Flags]
public enum DebugOption
{
    None,

    Clip = 1 << 0,
    OnlyHoveredElement = 1 << 1,
    OnlyTrueHoveredElement = 1 << 2,

    ShowBounds = 1 << 3,
    ShowBoundsDimensions = 1 << 4,
    ShowClipArea = 1 << 5,
    ShowPriority = 1 << 6
}


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowBoundsAttribute(bool inherit = true) : ElementAttribute(inherit);


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowBoundsDimensionsAttribute(bool inherit = true) : ElementAttribute(inherit);


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowClipAreaAttribute(bool inherit = true) : ElementAttribute(inherit);


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowPriorityAttribute(bool inherit = true) : ElementAttribute(inherit);


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreInspection(bool inherit = true) : ElementAttribute(inherit);


public sealed class Debugger : IUpdateable, IDrawable
{
    public InspectionWindow InspectionWindow { get; }
    public Inspectors Inspectors { get; }

    public DebugOption Options { get; set; }
    public bool EnableKeyShortcuts { get; set; }


    public Debugger()
    {
        InspectionWindow = new InspectionWindow
        {
            Visible = false
        };

        Inspectors = [
            new ElementInspector(), new ClickableInspector(), new DraggableInspector(),
            new ResizableInspector()
        ];

        Options = DebugOption.None;

        App.AddElement(InspectionWindow);
    }


    public void Update() => ProcessDebugShortcuts();

    private void ProcessDebugShortcuts()
    {
        if (!EnableKeyShortcuts || App.PressedKey is null)
            return;

        switch (App.PressedKey.Scancode)
        {
            case Keyboard.Scancode.Escape:
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
                ToggleDebugOption(DebugOption.ShowBoundsDimensions);
                break;

            case Keyboard.Scancode.F6:
                ToggleDebugOption(DebugOption.ShowClipArea);
                break;

            case Keyboard.Scancode.F7:
                ToggleDebugOption(DebugOption.ShowPriority);
                break;


            case Keyboard.Scancode.F12:
                InspectionWindow.Visible = !InspectionWindow.Visible;
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


    public void Draw(RenderTarget target)
    {
        if (Options == DebugOption.None)
            return;

        foreach (var element in App.Elements)
            DebugElement(target, element);
    }

    public void DebugElement(RenderTarget target, Element element)
    {
        if (Options.HasFlag(DebugOption.OnlyHoveredElement) && element != MouseInput.ElementWhichCaughtMouseInput)
            return;

        if (Options.HasFlag(DebugOption.OnlyTrueHoveredElement) && element != MouseInput.TrueElementWhichCaughtMouseInput)
            return;

        var clip = Options.HasFlag(DebugOption.Clip);

        if (clip)
            ClipArea.BeginClip(element.GetFinalClipArea());

        if (Options.HasFlag(DebugOption.ShowBounds) && !element.HasCachedElementAttribute<DebuggerIgnoreShowBoundsAttribute>())
            DrawElementBounds(target, element);

        if (Options.HasFlag(DebugOption.ShowBoundsDimensions) && !element.HasCachedElementAttribute<DebuggerIgnoreShowBoundsDimensionsAttribute>())
            DrawElementBoundsDimensions(target, element);

        if (Options.HasFlag(DebugOption.ShowClipArea) && !element.HasCachedElementAttribute<DebuggerIgnoreShowClipAreaAttribute>())
            DrawElementClipArea(target, element);

        if (Options.HasFlag(DebugOption.ShowPriority) && !element.HasCachedElementAttribute<DebuggerIgnoreShowPriorityAttribute>())
            DrawElementPriority(target, element);

        if (clip)
            ClipArea.EndClip();
    }


    public static void DrawElementBounds(RenderTarget target, Element element)
        => Debugging.Draw.LineRect(target, element.GetBounds(), Color.Red);

    public static void DrawElementBoundsDimensions(RenderTarget target, Element element)
    {
        var bounds = element.GetBounds();
        var borderLessBounds = element.GetBorderLessBounds();

        var backgroundColor = new Color(255, 255, 255, 220);

        var width = bounds.Width.ToString(CultureInfo.InvariantCulture);
        var height = bounds.Height.ToString(CultureInfo.InvariantCulture);

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
}
