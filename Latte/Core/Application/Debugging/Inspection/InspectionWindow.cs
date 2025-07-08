using System.Linq;
using System.Collections.Generic;

using Latte.Core.Type;
using Latte.Elements;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;
using SFML.Graphics;


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
        : base(null, new Vec2f(), new Vec2f(350, 350), 5)
    {
        _data = data;

        NameText = new TextElement(this, new Vec2f(), 10, data.Name)
        {
            Alignment = { Value = Elements.Alignment.HorizontalCenter | Elements.Alignment.Top },
            AlignmentMargin = { Value = new Vec2f(0, 10) }
        };

        DataText = new TextElement(this, new Vec2f(5, 20), 13, data.Data);

        Alignment.Set(Elements.Alignment.Center);

        Color.Set(new ColorRGBA(100, 100, 100, 150));
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
    
    public Element? ElementToInspect { get; set; }
    public Element? LockAtElement { get; set; }


    public InspectionWindow() : base("Inspector", new Vec2f(10, 10), new Vec2f(400, 400), WindowElementStyles.Moveable)
    {
        _lastInspectedElement = null;
        _frames = [];

        ScrollArea = new ScrollAreaElement(this, new Vec2f(), new Vec2f(380, 340))
        {
            Alignment = { Value = Elements.Alignment.Center },
            AlignmentMargin = { Value = new Vec2f(0, 20) },

            Color = { Value = new ColorRGBA(150, 150, 150, 100) },
            Radius = { Value = 3f }
        };

        DataGrid = new GridLayoutElement(ScrollArea, new Vec2f(), 0, 0, 380, 380)
        {
            GrowDirection = GridLayoutGrowDirection.Vertical,
            MinColumns = 1
        };

        Radius.Set(5f);

        BorderSize.Set(1f);

        Color.Set(new ColorRGBA(100, 100, 100, 100));
        BorderColor.Set(new ColorRGBA(255, 255, 255, 200));

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
            UpdateInspectionFramesData(App.Debugger.Inspectors.InspectAll(ElementToInspect));

        else if (!ElementToInspect.HasCachedElementAttribute<DebuggerIgnoreInspection>())
            CreateInspectionFrames(App.Debugger.Inspectors.InspectAll(ElementToInspect));
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

            App.AddElement(frame);
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
