using System;
using System.Drawing;

namespace Trigrad.ColorGraders
{
    public class DistanceGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Color Grade(Color cU, Color cV, Color cW, double u, double v, double w, int x, int y, Point pU, Point pV, Point pW)
        {
            double distU = dist(new Point(x, y), pU);
            double distV = dist(new Point(x, y), pV);
            double distW = dist(new Point(x, y), pW);


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
