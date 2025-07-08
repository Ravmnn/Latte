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
