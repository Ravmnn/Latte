using System.Reflection;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Latte.UI;


namespace Latte.Debugging.Inspection.Formatting;




public abstract class InspectionObjectFormatter<T>
{
    public abstract string Format(T @object, int indent = 0);


    protected string FormatAllProperties(object @object, int indent = 0)
    {
        var builder = new StringBuilder();
        var type = @object.GetType();

        type.ForeachProperty(property =>
        {
            builder.AppendLine(InspectionObjectFormatter.PropertyToString(@object, property, indent));
        });

        builder.Remove(builder.Length - 1, 1); // remove new line character

        return builder.ToString();
    }
}




public static class InspectionObjectFormatter
{
    public static (string, bool) Format(object? @object, int indent = 0)
        => @object switch
        {
            MouseClickState state => (new MouseClickStateObjectFormatter().Format(state, indent + 1), true),

            null => ("null", false),

            _ => (@object.ToString()!, false)
        };




    public static string PropertiesToString(object @object, IEnumerable<PropertyInfo> properties, int indent = 0)
    {
        var builder = new StringBuilder();

        foreach (var property in properties)
            builder.AppendLine(PropertyToString(@object, property, indent));

        return builder.ToString();
    }


    public static string PropertyToString(object @object, PropertyInfo property, int indent = 0)
    {
        var (formatResult, isComplexType) = Format(property.GetValue(@object), indent);
        var indentString = string.Concat(Enumerable.Repeat("    ", indent));

        return $"{indentString}{FormatPropertyName(property)}: {(isComplexType ? "\n" : "")}{formatResult}";
    }




    public static string FormatPropertyName(PropertyInfo property)
    {
        var propertyName = property.Name;
        var builder = new StringBuilder();

        for (var i = 0; i < propertyName.Length; i++)
        {
            var ch = propertyName[i];

            if (i > 0 && char.IsUpper(ch))
                builder.Append(' ');

            builder.Append(ch);
        }

        if (!char.IsUpper(propertyName[0]))
            builder[0] = char.ToUpper(propertyName[0]);

        return builder.ToString();
    }
}
