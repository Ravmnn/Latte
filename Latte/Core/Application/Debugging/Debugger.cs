using System;
using System.Globalization;

using SFML.Window;
using SFML.Graphics;

using Latte.Core.Type;
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

    RenderBounds = 1 << 3,
    RenderBoundsDimensions = 1 << 4,
    RenderClipArea = 1 << 5,
    RenderPriority = 1 << 6
}


public class Debugger : IUpdateable, IDrawable
{
    public DebugOption Options { get; set; } = DebugOption.None;
    public bool EnableKeyShortcuts { get; set; }


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
                ToggleDebugOption(DebugOption.RenderBounds);
                break;

            case Keyboard.Scancode.F5:
                ToggleDebugOption(DebugOption.RenderBoundsDimensions);
                break;

            case Keyboard.Scancode.F6:
                ToggleDebugOption(DebugOption.RenderClipArea);
                break;

            case Keyboard.Scancode.F7:
                ToggleDebugOption(DebugOption.RenderPriority);
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

        foreach (Element element in App.Elements)
            DebugElement(target, element);
    }

    public void DebugElement(RenderTarget target, Element element)
    {
        if (Options.HasFlag(DebugOption.OnlyHoveredElement) && element != App.ElementWhichCaughtMouseInput)
            return;

        if (Options.HasFlag(DebugOption.OnlyTrueHoveredElement) && element != App.TrueElementWhichCaughtMouseInput)
            return;

        bool clip = Options.HasFlag(DebugOption.Clip);

        if (clip)
            ClipArea.BeginClip(element.GetFinalClipArea());

        if (Options.HasFlag(DebugOption.RenderBounds))
            DrawElementBounds(target, element);

        if (Options.HasFlag(DebugOption.RenderBoundsDimensions))
            DrawElementBoundsDimensions(target, element);

        if (Options.HasFlag(DebugOption.RenderClipArea))
            DrawElementClipArea(target, element);

        if (Options.HasFlag(DebugOption.RenderPriority))
            DrawElementPriority(target, element);

        if (clip)
            ClipArea.EndClip();
    }


    public static void DrawElementBounds(RenderTarget target, Element element)
        => Debugging.Draw.LineRect(target, element.GetBounds(), Color.Red);

    public static void DrawElementBoundsDimensions(RenderTarget target, Element element)
    {
        FloatRect bounds = element.GetBounds();
        FloatRect borderLessBounds = element.GetBorderLessBounds();

        Color backgroundColor = new(255, 255, 255, 220);

        string width = bounds.Width.ToString(CultureInfo.InvariantCulture);
        string height = bounds.Height.ToString(CultureInfo.InvariantCulture);

        Debugging.Draw.Text(target, borderLessBounds with { Top = borderLessBounds.Top + borderLessBounds.Height + 10 }, Alignment.HorizontalCenter | Alignment.Top, width, backgroundColor: backgroundColor);
        Debugging.Draw.Text(target, borderLessBounds with { Left = borderLessBounds.Left + borderLessBounds.Width + 10 }, Alignment.VerticalCenter | Alignment.Left, height, backgroundColor: backgroundColor);
    }

    public static void DrawElementClipArea(RenderTarget target, Element element)
        => Debugging.Draw.LineRect(target, (FloatRect)element.GetClipArea(), Color.Blue);

    public static void DrawElementPriority(RenderTarget target, Element element)
    {
        uint absolutePriority = (uint)System.Math.Abs(element.Priority);

        Debugging.Draw.Rect(target, element.GetBounds(), ColorGenerator.FromIndex(absolutePriority, 50));
        Debugging.Draw.Text(target, element.GetBorderLessBounds(), Alignment.Center, element.Priority.ToString(), backgroundColor: Color.White);
    }
}
