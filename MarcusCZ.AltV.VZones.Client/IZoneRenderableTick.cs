using AltV.Net.Data;

namespace MarcusCZ.AltV.VZones.Client;

public interface IZoneRenderableTick
{
    void Render(Rgba color);
}