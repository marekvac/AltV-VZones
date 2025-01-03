using AltV.Net.Data;

namespace MarcusCZ.AltV.VZones.Client;

public interface IZoneRenderable
{
    void Render(Rgba color);
    void CancelRender();
    bool IsRendering();
}