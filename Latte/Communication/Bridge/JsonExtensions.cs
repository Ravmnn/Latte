using System.Text.Json;
using System.Text.Json.Nodes;


namespace Latte.Communication.Bridge;




public static class JsonExtensions
{
    public static JsonObject? ToJsonObject(this object @object)
    {
        var jsonString = JsonSerializer.Serialize(@object);
        var jsonObject = JsonNode.Parse(jsonString)?.AsObject();

        return jsonObject;
    }


    public static string ToJsonString(this object @object)
    {
        var jsonObject = @object.ToJsonObject();
        return jsonObject?.ToJsonString() ?? string.Empty;
    }


    public static T? JsonStringAs<T>(this string jsonString)
        => JsonSerializer.Deserialize<T>(jsonString) ?? default;
}
