using System.Collections.Generic;

using Latte.Core.Application.Debugging.Inspection.Formatting;


namespace Latte.Core.Application.Debugging.Inspection;


public record InspectionData(string Name, string Data);


public static class Inspector
{
    public static IEnumerable<InspectionData> Inspect(object @object)
    {
        var inspectionDatas = new List<InspectionData>();
        var properties = new OrganizedPropertyContainer(@object).GetNonEmpty();

        foreach (var (type, typeProperties) in properties)
            inspectionDatas.Add(new InspectionData(type.Name, InspectionObjectFormatter.PropertiesToString(@object, typeProperties)));

        return inspectionDatas;
    }
}
