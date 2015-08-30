using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigrad.DataTypes;

namespace Trigrad.ColorGraders
{
    /// <summary> Color grader that fills a triangle with a dithered output. </summary>
    public class DitherGrader : IGrader
    {
        private static Random r = new Random();

        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Color Grade(Color cU, Color cV, Color cW, BarycentricCoordinates coords, Point p, Point pU, Point pV, Point pW)
        {
            if (coords.U >= 0.9)
                return cU;
            if (coords.V >= 0.9)
                return cV;
            if (coords.W >= 0.9)
                return cW;


            int valU = (int)(p.X + p.Y + coords.U + r.Next(0, 4)) % 4;
            int valV = (int)(p.X + p.Y + coords.V + r.Next(0, 4)) % 4;
            int valW = (int)(p.X + p.Y + coords.W + r.Next(0, 4)) % 4;

            if (coords.U >= coords.V && coords.U >= coords.W)
                return ditherFurther(cU, cV, cW, valU);
            if (coords.V >= coords.W)
                return ditherFurther(cV, cU, cW, valV);

            return ditherFurther(cW, cV, cU, valW);
        }
        private static Color ditherFurther(Color a, Color b, Color c, int val)
        {
            switch (val)
            {
                case 0:
                    return a;
                case 1:
                    return b;
                case 2:
                    return a;
                case 3:
                    return c;
                default:
                    return a;
            }
        }
    }
}
