using System.Linq;
using System.Collections.Generic;

using Latte.Elements;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Core.Application.Debugging.Inspection;


public class InspectionFrameElement : RectangleElement
{
    private InspectionData _data;


    public TextElement NameText { get; }
    public TextElement DataText { get; }

    public InspectionData Data
    {
        get => _data;
        set
        {
            _data = value;

            NameText.Text.Set(_data.Name);
            DataText.Text.Set(_data.Data);
        }
    }


    public InspectionFrameElement(InspectionData data)
        : base(null, new(), new(350, 350), 5)
    {
        _data = data;

        NameText = new(this, new(), 10, data.Name)
        {
            Alignment = { Value = Elements.Alignment.HorizontalCenter | Elements.Alignment.Top },
            AlignmentMargin = { Value = new(0, 10) }
        };

        DataText = new(this, new(5, 20), 13, data.Data);

        Alignment.Set(Elements.Alignment.Center);

        Color.Set(new(100, 100, 100, 150));
    }
}


[DebuggerIgnoreShowBounds, DebuggerIgnoreShowBoundsDimensions, DebuggerIgnoreShowClipArea, DebuggerIgnoreShowPriority]
[DebuggerIgnoreInspection]
public class InspectionWindow : WindowElement
{
    private Element? _lastInspectedElement;
    private readonly List<InspectionFrameElement> _frames;


    public ScrollAreaElement ScrollArea { get; }
    public GridLayoutElement DataGrid { get; }


    public InspectionWindow() : base("Inspector", new(10, 10), new(400, 400), WindowElementStyles.Moveable)
    {
        _lastInspectedElement = null;
        _frames = [];

        ScrollArea = new(this, new(), new(380, 340))
        {
            Alignment = { Value = Elements.Alignment.Center },
            AlignmentMargin = { Value = new(0, 20) },

            Color = { Value = new(150, 150, 150, 100) },
            Radius = { Value = 3f }
        };

        DataGrid = new(ScrollArea, new(), 0, 0, 380, 380)
        {
            GrowDirection = GridLayoutGrowDirection.Vertical,
            MinColumns = 1
        };

        Radius.Set(5f);

        BorderSize.Set(1f);

        Color.Set(new(100, 100, 100, 100));
        BorderColor.Set(new(255, 255, 255, 200));

        PrioritySnap = PrioritySnap.AlwaysOnTop;
    }


    public override void ConstantUpdate()
    {
        UpdateInspectionFrames();

        base.ConstantUpdate();
    }


    private void UpdateInspectionFrames()
    {
        if (App.TrueElementWhichCaughtMouseInput is not {} element || App.Debugger is not {} debugger)
            return;

        if (element == _lastInspectedElement)
            UpdateInspectionFramesData(debugger.Inspectors.InspectAll(_lastInspectedElement));

        else if (!element.HasCachedElementAttribute<DebuggerIgnoreInspection>())
            CreateInspectionFrames(debugger.Inspectors.InspectAll(element));
    }


    private void UpdateInspectionFramesData(IEnumerable<InspectionData> data)
    {
        foreach (InspectionData inspectionData in data)
            _frames.First(frame => frame.Data.Name == inspectionData.Name).Data = inspectionData;
    }


    private void CreateInspectionFrames(IEnumerable<InspectionData> data)
    {
        DataGrid.Clear();
        _frames.Clear();

        foreach (InspectionData inspectionData in data)
        {
            InspectionFrameElement frame = new(inspectionData);
            DataGrid.AddElementAtEnd(frame);
            _frames.Add(frame);

            App.AddElement(frame);
        }

        _lastInspectedElement = App.TrueElementWhichCaughtMouseInput;
    }
}
