using System.Linq;
using System.Collections.Generic;

using SFML.Graphics;

using Latte.Core.Type;
using Latte.Application;
using Latte.Application.Elements.Attributes;
using Latte.Application.Elements.Primitives;
using Latte.Debugging.Inspection;


namespace Latte.Debugging.Elements;


[DebuggerIgnoreShowBounds, DebuggerIgnoreShowBoundsDimensionsAndPosition, DebuggerIgnoreShowClipArea, DebuggerIgnoreShowPriority]
[DebuggerIgnoreInspection]
public class InspectionWindow : DebugWindow
{
    private readonly List<InspectionFrameElement> _frames;

    private Element? _lastInspectedElement;
    private Element? _lockAtElement;


    public DebugScrollArea ScrollArea { get; }
    public GridLayoutElement DataGrid { get; }

    public Element? ElementToInspect { get; set; }

    public Element? LockAtElement
    {
        get => _lockAtElement;
        set => _lockAtElement = value?.HasCachedElementAttribute<DebuggerIgnoreInspection>() ?? false ? null : value;
    }


    public InspectionWindow() : base("Inspector", new Vec2f(10, 10), new Vec2f(400, 400))
    {
        _lastInspectedElement = null;
        _frames = [];

        ScrollArea = new DebugScrollArea(this, null, new Vec2f(380, 340))
        {
            AlignmentMargin = new Vec2f(0, 20)
        };

        DataGrid = new GridLayoutElement(ScrollArea, new Vec2f(), 0, 0, 380, 380)
        {
            GrowDirection = GridLayoutGrowDirection.Vertical,
            MinColumns = 1
        };
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
