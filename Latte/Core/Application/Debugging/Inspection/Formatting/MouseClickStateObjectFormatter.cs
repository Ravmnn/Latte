using Latte.Elements.Behavior;


namespace Latte.Core.Application.Debugging.Inspection.Formatting;


public class MouseClickStateObjectFormatter : InspectionObjectFormatter<MouseClickState>
{
    public override string Format(MouseClickState state, int indent = 0)
        => FormatAllProperties(state, indent);
}
