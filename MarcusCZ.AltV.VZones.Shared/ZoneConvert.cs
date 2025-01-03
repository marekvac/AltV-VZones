using System.Text.Json;
using MarcusCZ.AltV.VZones.Shared.Adapters.Json;

namespace MarcusCZ.AltV.VZones.Shared;

public static class ZoneConvert
{
    public static readonly JsonSerializerOptions JsonOptions = new () {WriteIndented = true, Converters = {new Vector3Converter(), new Vector2Converter()}};

    public static T? FromJson<T>(string json) where T : class, IZone
    {
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    public static string ToJson<T>(this T zone) where T : IZone
    {
        return JsonSerializer.Serialize(zone, JsonOptions);
    }
}