using System.Numerics;
using AltV.Net;
using AltV.Net.Elements.Args;

namespace MarcusCZ.AltV.VZones.Shared.Adapters;

public abstract class SharedZone3Adapter<T> : IMValueAdapter<T> where T : SharedZone3
{
    private readonly DefaultMValueAdapters.DefaultArrayAdapter<Vector2> _vector2ArrayAdapter = new(new Vector2Adapter());
    
    object IMValueBaseAdapter.FromMValue(IMValueReader reader)
    {
        return FromMValue(reader);
    }

    public void ToMValue(object obj, IMValueWriter writer)
    {
        if (obj is T zone3) ToMValue(zone3, writer);
    }

    public T FromMValue(IMValueReader reader)
    {
        reader.BeginObject();
        string name = "";
        float z = default;
        float height = default;
        List<Vector2> points = new();
        bool active = false;
        while (reader.HasNext())
        {
            switch (reader.NextName())
            {
                case "name":
                    name = reader.NextString();
                    break;
                case "z":
                    z = (float) reader.NextDouble();
                    break;
                case "h":
                    height = (float) reader.NextDouble();
                    break;
                case "points":
                    points = _vector2ArrayAdapter.FromMValue(reader);
                    break;
                case "active":
                    active = reader.NextBool();
                    break;
            }
        }

        reader.EndObject();
        
        T zone3 = (T)Activator.CreateInstance(typeof(T))!;
        zone3.Points = points;
        zone3.Height = height;
        zone3.Name = name;
        zone3.Z = z;
        zone3.Active = active;
        return zone3;
    }

    public void ToMValue(T value, IMValueWriter writer)
    {
        writer.BeginObject();
        writer.Name("name");
        writer.Value(value.Name);
        writer.Name("z");
        writer.Value(value.Z);
        writer.Name("h");
        writer.Value(value.Height);
        writer.Name("points");
        _vector2ArrayAdapter.ToMValue(value.Points, writer);
        writer.Name("active");
        writer.Value(value.Active);
        writer.EndObject();
    }
}