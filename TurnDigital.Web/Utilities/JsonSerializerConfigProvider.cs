using System.Text.Json;
using System.Text.Json.Serialization;

namespace TurnDigital.Web.Utilities;

public class JsonSerializerConfigProvider
{
    private readonly JsonSerializerOptions _serializerOptions;

    public JsonSerializerConfigProvider()
    {
        _serializerOptions = new JsonSerializerOptions();
        
        _serializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public JsonSerializerOptions SerializerOptions => _serializerOptions;
}