using AltV.Net;
using MarcusCZ.AltV.VZones.Shared;

namespace MarcusCZ.AltV.VZones.Client.Adapters;

[Obsolete]
public class ZoneAdapter : IMValueAdapter<IZone>
{
    object IMValueBaseAdapter.FromMValue(IMValueReader reader)
    {
        return FromMValue(reader);
    }

    public void ToMValue(object obj, IMValueWriter writer)
    {
        if (obj.GetType().IsAssignableFrom(typeof(IZone)))
        {
            ToMValue(obj, writer);
        }
    }

    public IZone FromMValue(IMValueReader reader)
    {
        reader.BeginObject();
        if (reader.NextName() != "type") return null;
        
        switch (reader.NextString())
        {
            case "zone2":
                return new Zone2Adapter().FromMValue(reader);
            case "zone3":
                return new Zone3Adapter().FromMValue(reader);
        }
        
        return null;
    }

    public void ToMValue(IZone value, IMValueWriter writer)
    {
        value.GetAdapter().ToMValue(value, writer);
    }
}