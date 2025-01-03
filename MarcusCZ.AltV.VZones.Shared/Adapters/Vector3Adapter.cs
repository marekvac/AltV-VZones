using System.Numerics;
using AltV.Net;

namespace MarcusCZ.AltV.VZones.Shared.Adapters;

public class Vector3Adapter : IMValueAdapter<Vector3>
{
    object IMValueBaseAdapter.FromMValue(IMValueReader reader)
    {
        return FromMValue(reader);
    }

    public void ToMValue(object obj, IMValueWriter writer)
    {
        if (!(obj is Vector3 vector2))
            return;
        ToMValue(vector2, writer);
    }

    public Vector3 FromMValue(IMValueReader reader)
    {
        Vector3 vector3 = new Vector3();
        reader.BeginObject();
        while (reader.HasNext())
        {
            switch (reader.NextName())
            {
                case "x":
                    vector3.X = (float) reader.NextDouble();
                    break;
                case "y":
                    vector3.Y = (float) reader.NextDouble();
                    break;
                case "z":
                    vector3.Z = (float) reader.NextDouble();
                    break;
            }
        }
        reader.EndObject();
        return vector3;
    }

    public void ToMValue(Vector3 value, IMValueWriter writer)
    {
        writer.BeginObject();
        writer.Name("x");
        writer.Value((double) value.X);
        writer.Name("y");
        writer.Value((double) value.Y);
        writer.Name("z");
        writer.Value((double) value.Z);
        writer.EndObject();
    }
}