using System;
using System.Drawing;
using Trigrad.DataTypes;

namespace Trigrad.ColorGraders
{
    public class DistanceGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Color Grade(Color cU, Color cV, Color cW, BarycentricCoordinates coords, Point p, Point pU, Point pV, Point pW)
        {
            double distU = dist(p, pU);
            double distV = dist(p, pV);
            double distW = dist(p, pW);


            if (distU < distV && distU < distW)
                return cU;
            if (distV < distW)
                return cV;
            return cW;
        }

        private double dist(Point a, Point b)
        {
            int dX = a.X - b.X;
            int dY = a.Y - b.Y;

            if (dX > dY)
                return dX;
            return dY;
        }
    }
}
