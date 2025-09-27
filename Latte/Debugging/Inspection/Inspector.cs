using System.Collections.Generic;

using Latte.Debugging.Inspection.Formatting;


namespace Latte.Debugging.Inspection;




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
