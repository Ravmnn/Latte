using System.Linq;
using System.Collections.Generic;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Application;
using Latte.Application.Elements.Primitives;
using Latte.Debugging.Inspection;


namespace Latte.Debugging.Elements;


[DebuggerIgnoreInspection]
public class InspectionWindow : DebugWindow
{
    private readonly List<InspectionFrameElement> _frames;

    private BaseObject? _lastInspectedObject;
    private BaseObject? _lockAtObject;


    public DebugScrollArea ScrollArea { get; }
    public GridLayoutElement DataGrid { get; }

    public BaseObject? ObjectToInspect { get; set; }

    public BaseObject? LockAtObject
    {
        get => _lockAtObject;
        set => _lockAtObject = value?.HasCachedObjectAttribute<DebuggerIgnoreInspection>() ?? false ? null : value;
    }


    public InspectionWindow() : base("Inspector", new Vec2f(10, 10), new Vec2f(400, 400))
    {
        _lastInspectedObject = null;
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
        if (LockAtObject is not null)
            ObjectToInspect = LockAtObject;

        else if (MouseInput.TrueObjectWhichCaughtMouseInput is { } element && App.Debugger is not null)
            ObjectToInspect = element;

        UpdateInspectionFrames();

        base.ConstantUpdate();
    }


    private void UpdateInspectionFrames()
    {
        if (App.Debugger is null || ObjectToInspect is null)
            return;

        if (ObjectToInspect == _lastInspectedObject )
            UpdateInspectionFramesData(Inspector.Inspect(ObjectToInspect));

        else if (!ObjectToInspect.HasCachedObjectAttribute<DebuggerIgnoreInspection>())
            CreateInspectionFrames(Inspector.Inspect(ObjectToInspect));
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

        _lastInspectedObject = MouseInput.TrueObjectWhichCaughtMouseInput;
    }


    public override void Draw(RenderTarget renderTarget)
    {
        base.Draw(renderTarget);

        if (LockAtObject is not null)
            Debugger.DrawObjectBounds(renderTarget, LockAtObject, SFML.Graphics.Color.Green);
    }
}
