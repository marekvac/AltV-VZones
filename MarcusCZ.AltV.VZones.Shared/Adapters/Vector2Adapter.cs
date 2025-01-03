using System.Numerics;
using AltV.Net;

namespace MarcusCZ.AltV.VZones.Shared.Adapters;

public class Vector2Adapter : IMValueAdapter<Vector2>
{
    object IMValueBaseAdapter.FromMValue(IMValueReader reader)
    {
        return FromMValue(reader);
    }

    public void ToMValue(object obj, IMValueWriter writer)
    {
        if (!(obj is Vector2 vector2))
            return;
        ToMValue(vector2, writer);
    }

    public Vector2 FromMValue(IMValueReader reader)
    {
        Vector2 vector2 = default;
        reader.BeginObject();
        while (reader.HasNext())
        {
            switch (reader.NextName())
            {
                case "x":
                    vector2.X = (float) reader.NextDouble();
                    break;
                case "y":
                    vector2.Y = (float) reader.NextDouble();
                    break;
            }
        }
        reader.EndObject();
        return vector2;
    }

    public void ToMValue(Vector2 value, IMValueWriter writer)
    {
        writer.BeginObject();
        writer.Name("x");
        writer.Value((double) value.X);
        writer.Name("y");
        writer.Value((double) value.Y);
        writer.EndObject();
    }
}