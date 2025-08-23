using Latte.Application.Elements.Behavior;


namespace Latte.Debugging.Inspection.Formatting;


public class MouseClickStateObjectFormatter : InspectionObjectFormatter<MouseClickState>
{
    public override string Format(MouseClickState state, int indent = 0)
        => FormatAllProperties(state, indent);
}
