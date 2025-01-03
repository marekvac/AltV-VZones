using System.Numerics;
using AltV.Net;

namespace MarcusCZ.AltV.VZones.Shared.Adapters;

public abstract class SharedCylZoneAdapter<T> : IMValueAdapter<T> where T : SharedCylZone
{
    private readonly Vector3Adapter _vector3Adapter = new ();
    
    object IMValueBaseAdapter.FromMValue(IMValueReader reader)
    {
        return FromMValue(reader);
    }

    public void ToMValue(object obj, IMValueWriter writer)
    {
        if (obj is T cylZone) ToMValue(cylZone, writer);
    }

    public T FromMValue(IMValueReader reader)
    {
        reader.BeginObject();
        string name = "";
        Vector3 center = default;
        float radius = default;
        float height = default;
        bool active = default;
        while (reader.HasNext())
        {
            switch (reader.NextName())
            {
                case "name":
                    name = reader.NextString();
                    break;
                case "radius":
                    radius = (float) reader.NextDouble();
                    break;
                case "h":
                    height = (float) reader.NextDouble();
                    break;
                case "center":
                    center = _vector3Adapter.FromMValue(reader);
                    break;
                case "active":
                    active = reader.NextBool();
                    break;
            }
        }
        reader.EndObject();
        
        T cylZone = (T)Activator.CreateInstance(typeof(T))!;
        cylZone.Name = name;
        cylZone.Radius = radius;
        cylZone.Height = height;
        cylZone.Center = center;
        cylZone.Active = active;
        return cylZone;
    }

    public void ToMValue(T value, IMValueWriter writer)
    {
        writer.BeginObject();
        writer.Name("name");
        writer.Value(value.Name);
        writer.Name("center");
        _vector3Adapter.ToMValue(value.Center, writer);
        writer.Name("radius");
        writer.Value(value.Radius);
        writer.Name("h");
        writer.Value(value.Height);
        writer.Name("active");
        writer.Value(value.Active);
        writer.EndObject();
    }
}