using System.Numerics;

namespace MarcusCZ.AltV.VZones.Client.Structs;

public struct Line2
{
    public Vector2 P1, P2;

    public Line2(Vector2 p1, Vector2 p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public Vector2 GetCenter()
    {
        return (P1 + P2) / 2;
    }
    
    public bool Intersects(Line2 line2, out Vector2 intersection)
    {
        intersection = default;
        var denominator = ((line2.P2.Y - line2.P1.Y) * (P2.X - P1.X)) -
                          ((line2.P2.X - line2.P1.X) * (P2.Y - P1.Y));

        if (denominator == 0)
        {
            return false;
        }

        var a = P1.Y - line2.P1.Y;
        var b = P1.X - line2.P1.X;
        var numerator1 = ((line2.P2.X - line2.P1.X) * a) - ((line2.P2.Y - line2.P1.Y) * b);
        var numerator2 = ((P2.X - P1.X) * a) - ((P2.Y - P1.Y) * b);
        a = numerator1 / denominator;
        b = numerator2 / denominator;

        if (a >= 0 && a <= 1 && b >= 0 && b <= 1)
        {
            intersection =  new Vector2(
                P1.X + (a * (P2.X - P1.X)),
                P1.Y + (a * (P2.Y - P1.Y))
            );
            return true;
        }

        return false;
    }
}