using System.Numerics;
using MarcusCZ.AltV.VZones.Shared.Util;

namespace MarcusCZ.AltV.VZones.Shared.Structs;

public class Rectangle3
{
    public Vector3 P1, P2;

    public Rectangle3(Vector3 p1, Vector3 p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public Rectangle3(Vector2 p1, Vector2 p2, float floor, float height)
    {
        P1 = new Vector3(p1, floor);
        P2 = new Vector3(p2, floor + height);
    }

    public bool IntersectsLine3(Line3 line, out Vector3 intersection)
    {
        intersection = default;
        
        Line2 xyWall = new Line2(VectorUtils.AsVector2(P1), VectorUtils.AsVector2(P2));
        
        // primka raycastu v rovine XY
        Line2 xyRay = new Line2(VectorUtils.AsVector2(line.P1), VectorUtils.AsVector2(line.P2));
        
        // kontrola zda se primky protinaji v rovine XY
        // Pokud se neprotinaji v teto rovine, hrac nemiri na stenu
        if (!xyWall.Intersects(xyRay, out Vector2 xyIntersection)) return false;

        // primka steny v pruseciku v rovine XZ
        Line2 xzWall = new Line2(
            xyIntersection with {Y = P1.Z},
            xyIntersection with {Y = P2.Z}
        );
        
        // primka raycastu v rovine XZ
        Line2 xzRay = new Line2(
            new Vector2(line.P1.X, line.P1.Z),
            new Vector2(line.P2.X, line.P2.Z)
        );

        // kontrola zda se primky protinaji v rovine XZ
        if (xzWall.Intersects(xzRay, out Vector2 xzIntersection))
        {
            intersection = new Vector3(xyIntersection, xzIntersection.Y);
            return true;
        }
        // primky se mohou v rovine XZ prekryvat, proto kontrolujeme posledni rovinu
        
        // primka steny v pruseciku v rovine YZ
        Line2 yzWall = new Line2(
            new Vector2(xyIntersection.Y, P1.Z),
            new Vector2(xyIntersection.Y, P2.Z)
        );
        
        // primka raycastu v rovine YZ
        Line2 yzRay = new Line2(
            new Vector2(line.P1.Y, line.P1.Z),
            new Vector2(line.P2.Y, line.P2.Z)
        );
        
        if (yzWall.Intersects(yzRay, out Vector2 yzIntersection))
        {
            intersection = new Vector3(xyIntersection, yzIntersection.Y);
            return true;
        }
        
        return false;
    }
}