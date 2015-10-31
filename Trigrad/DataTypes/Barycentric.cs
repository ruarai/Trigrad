using System.Drawing;

namespace Trigrad.DataTypes
{
    public static class Barycentric
    {
        public static BarycentricCoordinates GetCoordinates(Point Pp, Point Pa, Point Pb, Point Pc)
        {
            double[] v0 = { Pb.X - Pa.X, Pb.Y - Pa.Y };
            double[] v1 = { Pc.X - Pa.X, Pc.Y - Pa.Y };
            double[] v2 = { Pp.X - Pa.X, Pp.Y - Pa.Y };
            double d00 = dotProduct(v0, v0);
            double d01 = dotProduct(v0, v1);
            double d11 = dotProduct(v1, v1);
            double d20 = dotProduct(v2, v0);
            double d21 = dotProduct(v2, v1);
            double denom = d00 * d11 - d01 * d01;
            double v = ((d11 * d20 - d01 * d21) / denom);
            double w = ((d00 * d21 - d01 * d20) / denom);
            double u = (1.0f - v - w);

            return new BarycentricCoordinates(u, v, w);
        }

        public static bool ValidCoords(BarycentricCoordinates coords)
        {
            return coords.U >= 0 && coords.V >= 0 && coords.W >= 0;
        }

        private static double dotProduct(double[] vec1, double[] vec2)
        {
            return vec1[0]*vec2[0] + vec1[1]*vec2[1];
        }
    }
    public struct BarycentricCoordinates
    {
        public BarycentricCoordinates(double u, double v, double w)
        {
            U = u;
            V = v;
            W = w;
        }
        public double U;
        public double V;
        public double W;
    }
}
