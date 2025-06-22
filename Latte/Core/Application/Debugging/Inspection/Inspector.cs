using System.Text;
using System.Collections.Generic;

using Latte.Elements;
using Latte.Elements.Primitives;


namespace Latte.Core.Application.Debugging.Inspection;


public record InspectionData(string Name, string Data);



public interface IInspector
{
    InspectionData Inspect(object value);

    bool CanInspect(object value);
}


public interface IInspector<T> : IInspector
{
    InspectionData Inspect(T value);


    InspectionData IInspector.Inspect(object value)
        => Inspect((T)value);

    bool IInspector.CanInspect(object value)
        => value is T;
}


public sealed class Inspectors : List<IInspector>
{
    public IEnumerable<InspectionData> InspectAll(object value)
    {
        List<InspectionData> inspectionDataList = [];

        foreach (var inspector in this)
        {
            if (!inspector.CanInspect(value))
                continue;

            inspectionDataList.Add(inspector.Inspect(value));
        }

        return inspectionDataList;
    }


    public InspectionData? InspectOnly<T>(object value)
    {
        foreach (var inspector in this)
            if (value is T && inspector.CanInspect(value))
                return inspector.Inspect(value);

        return null;
    }
}





public sealed class ElementInspector : IInspector<Element>
{
    public InspectionData Inspect(Element element)
    {
        var data = new StringBuilder();

        foreach (var property in element.Properties)
            data.AppendLine($"{property.Name}: {property.Value}");

        return new InspectionData("Element", data.ToString());
    }
}


public sealed class ClickableInspector : IInspector<IClickable>
{
    public InspectionData Inspect(IClickable clickable)
    {
        var ms = clickable.MouseState;
        var data = new StringBuilder();

        data.AppendLine($"Ignore mouse input: {clickable.IgnoreMouseInput}");
        data.AppendLine($"Caught mouse input: {clickable.CaughtMouseInput}");

        data.AppendLine($"Mouse over: {ms.IsMouseOver}");
        data.AppendLine($"Mouse hover: {ms.IsMouseHover}");
        data.AppendLine($"Mouse down: {ms.IsMouseDown}");
        data.AppendLine($"Pressed: {ms.IsPressed}");
        data.AppendLine($"True pressed: {ms.IsTruePressed}");

        data.AppendLine($"Was mouse over: {ms.WasMouseOver}");
        data.AppendLine($"Was mouse hover: {ms.WasMouseHover}");
        data.AppendLine($"Was mouse down: {ms.WasMouseDown}");
        data.AppendLine($"Was pressed: {ms.WasPressed}");
        data.AppendLine($"Was true pressed: {ms.WasTruePressed}");

        return new InspectionData("Clickable", data.ToString());
    }
}


public sealed class DraggableInspector : IInspector<IDraggable>
{
    public InspectionData Inspect(IDraggable draggable)
    {
        var data = new StringBuilder();

        data.AppendLine($"Dragging: {draggable.Dragging}");
        data.AppendLine($"Was dragging: {draggable.WasDragging}");

        return new InspectionData("Draggable", data.ToString());
    }
}


public sealed class ResizableInspector : IInspector<IResizable>
{
    public InspectionData Inspect(IResizable resizable)
    {
        var data = new StringBuilder();

        data.AppendLine($"Resizing: {resizable.Resizing}");
        data.AppendLine($"Was resizing: {resizable.WasResizing}");

        data.AppendLine($"Rect: {resizable.Rect}");

        data.AppendLine($"Minimum size: {resizable.MinSize}");
        data.AppendLine($"Maximum size: {resizable.MaxSize}");

        data.AppendLine($"Corners selected: {resizable.CornerToResize}");
        data.AppendLine($"Corner selection size: {resizable.CornerResizeAreaSize}");

        return new InspectionData("Resizable", data.ToString());
    }
}
