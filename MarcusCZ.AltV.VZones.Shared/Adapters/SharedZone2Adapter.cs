using System.Numerics;
using AltV.Net;

namespace MarcusCZ.AltV.VZones.Shared.Adapters;

public class SharedZone2Adapter<T> : IMValueAdapter<T> where T : SharedZone2
{
    private readonly Vector2Adapter _vector2Adapter = new();
    
    object IMValueBaseAdapter.FromMValue(IMValueReader reader)
    {
        return FromMValue(reader);
    }

    public void ToMValue(object obj, IMValueWriter writer)
    {
        if (obj is T value)
        {
            ToMValue(value, writer);
        }
    }

    public T FromMValue(IMValueReader reader)
    {
        reader.BeginObject();
        string name = "";
        Vector2 p1 = Vector2.Zero;
        Vector2 p2 = Vector2.Zero;
        float z = default;
        float height = default;
        bool active = default;
        while (reader.HasNext())
        {
            switch (reader.NextName())
            {
                case "name":
                    name = reader.NextString();
                    break;
                case "p1":
                    p1 = _vector2Adapter.FromMValue(reader);
                    break;
                case "p2":
                    p2 = _vector2Adapter.FromMValue(reader);
                    break;
                case "z":
                    z = (float) reader.NextDouble();
                    break;
                case "h":
                    height = (float) reader.NextDouble();
                    break;
                case "active":
                    active = reader.NextBool();
                    break;
            }
        }
        reader.EndObject();
        reader.EndObject();
        
        T zone = (T) Activator.CreateInstance(typeof(T), name, p1, p2, z, height)!;
        zone.Active = active;
        return zone;
    }

    public void ToMValue(T value, IMValueWriter writer)
    {
        writer.BeginObject();
        writer.Name("type");
        writer.Value("zone2");
        writer.Name("object");
        writer.BeginObject();
        writer.Name("name");
        writer.Value(value.Name);
        writer.Name("p1");
        _vector2Adapter.ToMValue(value.P1, writer);
        writer.Name("p2");
        _vector2Adapter.ToMValue(value.P2, writer);
        writer.Name("z");
        writer.Value((double)value.Z);
        writer.Name("h");
        writer.Value((double)value.Height);
        writer.Name("active");
        writer.Value(value.Active);
        writer.EndObject();
        writer.EndObject();
    }
}