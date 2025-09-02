using System;
using System.Linq;
using System.Collections.Generic;

using SFML.Graphics;

using Latte.Core.Type;
using Latte.Application;
using Latte.Application.Elements;
using Latte.Application.Elements.Attributes;
using Latte.Application.Elements.Primitives;


namespace Latte.Debugging.Inspection;


[DebuggerIgnoreShowBounds, DebuggerIgnoreShowBoundsDimensionsAndPosition, DebuggerIgnoreShowClipArea, DebuggerIgnoreShowPriority]
[DebuggerIgnoreInspection]
public class InspectionWindow : WindowElement
{
    private readonly List<InspectionFrameElement> _frames;

    private Element? _lastInspectedElement;
    private Element? _lockAtElement;


    public ScrollAreaElement ScrollArea { get; }
    public GridLayoutElement DataGrid { get; }

    public Element? ElementToInspect { get; set; }

    public Element? LockAtElement
    {
        get => _lockAtElement;
        set => _lockAtElement = value?.HasCachedElementAttribute<DebuggerIgnoreInspection>() ?? false ? null : value;
    }


    public InspectionWindow() : base("Inspector", new Vec2f(10, 10), new Vec2f(400, 400), WindowElementStyles.Moveable)
    {
        _lastInspectedElement = null;
        _frames = [];

        Title.Color = SFML.Graphics.Color.White;

        ScrollArea = new ScrollAreaElement(this, new Vec2f(), new Vec2f(380, 340))
        {
            Alignment = Application.Elements.Behavior.Alignment.Center,
            AlignmentMargin = new Vec2f(0, 20),

            Color = new ColorRGBA(150, 150, 150, 100),
            Radius = 3f
        };

        DataGrid = new GridLayoutElement(ScrollArea, new Vec2f(), 0, 0, 380, 380)
        {
            GrowDirection = GridLayoutGrowDirection.Vertical,
            MinColumns = 1
        };

        Radius = 5f;

        BorderSize = 1f;

        Color = new ColorRGBA(100, 100, 100, 100);
        BorderColor = new ColorRGBA(255, 255, 255, 200);

        PrioritySnap = PrioritySnap.AlwaysOnTop;
        PrioritySnapOffset = 2;
    }


    public override void ConstantUpdate()
    {
        if (LockAtElement is not null)
            ElementToInspect = LockAtElement;

        else if (MouseInput.TrueElementWhichCaughtMouseInput is { } element && App.Debugger is not null)
            ElementToInspect = element;

        UpdateInspectionFrames();

        base.ConstantUpdate();
    }


    private void UpdateInspectionFrames()
    {
        if (App.Debugger is null || ElementToInspect is null)
            return;

        if (ElementToInspect == _lastInspectedElement )
            UpdateInspectionFramesData(Inspector.Inspect(ElementToInspect));

        else if (!ElementToInspect.HasCachedElementAttribute<DebuggerIgnoreInspection>())
            CreateInspectionFrames(Inspector.Inspect(ElementToInspect));
    }


    private void UpdateInspectionFramesData(IEnumerable<InspectionData> data)
    {
        foreach (var inspectionData in data)
            _frames.First(frame => frame.Data.Name == inspectionData.Name).Data = inspectionData;
    }


    private void CreateInspectionFrames(IEnumerable<InspectionData> data)
    {
        DataGrid.Clear();
        _frames.Clear();

        foreach (var inspectionData in data)
        {
            var frame = new InspectionFrameElement(inspectionData);
            DataGrid.AddElementAtEnd(frame);
            _frames.Add(frame);
        }

        _lastInspectedElement = MouseInput.TrueElementWhichCaughtMouseInput;
    }


    public override void Draw(RenderTarget renderTarget)
    {
        base.Draw(renderTarget);

        if (LockAtElement is not null)
            Debugger.DrawElementBounds(renderTarget, LockAtElement, SFML.Graphics.Color.Green);
    }
}
