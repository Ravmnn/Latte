using System.Text;

using Latte.Elements.Behavior;


namespace Latte.Core.Application.Debugging.Inspection.Formatting;


public class MouseClickStateObjectFormatter : InspectionObjectFormatter<MouseClickState>
{
    // TODO: automatize:

    public override string Format(MouseClickState state, int indent = 0)
    {
        var builder = new StringBuilder();
        var stateType = state.GetType();

        builder.AppendLine(InspectionObjectFormatter.PropertyToString(state, stateType.GetProperty("IsMouseOver")!, indent));
        builder.AppendLine(InspectionObjectFormatter.PropertyToString(state, stateType.GetProperty("IsMouseHover")!, indent));
        builder.AppendLine(InspectionObjectFormatter.PropertyToString(state, stateType.GetProperty("IsMouseDown")!, indent));
        builder.AppendLine(InspectionObjectFormatter.PropertyToString(state, stateType.GetProperty("IsPressed")!, indent));
        builder.AppendLine(InspectionObjectFormatter.PropertyToString(state, stateType.GetProperty("IsTruePressed")!, indent));
        builder.AppendLine(InspectionObjectFormatter.PropertyToString(state, stateType.GetProperty("WasMouseOver")!, indent));
        builder.AppendLine(InspectionObjectFormatter.PropertyToString(state, stateType.GetProperty("WasMouseHover")!, indent));
        builder.AppendLine(InspectionObjectFormatter.PropertyToString(state, stateType.GetProperty("WasMouseDown")!, indent));
        builder.AppendLine(InspectionObjectFormatter.PropertyToString(state, stateType.GetProperty("WasPressed")!, indent));
        builder.Append(InspectionObjectFormatter.PropertyToString(state, stateType.GetProperty("WasTruePressed")!, indent));

        return builder.ToString();
    }
}
