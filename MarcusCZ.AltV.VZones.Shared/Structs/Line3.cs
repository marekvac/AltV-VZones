using System.Numerics;

namespace MarcusCZ.AltV.VZones.Shared.Structs;

public class Line3
{
    public Vector3 P1, P2;

    public Line3(Vector3 p1, Vector3 p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public Vector3 GetCenter()
    {
        return (P1 + P2) / 2;
    }
}