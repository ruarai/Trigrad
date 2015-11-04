using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelMapSharp;
using Trigrad.DataTypes;

namespace Trigrad.ColorGraders
{
    /// <summary> Color grader that fills a triangle with a dithered output. </summary>
    public class DitherGrader : IGrader
    {
        private static Random r = new Random();

        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Pixel Grade(Sample u, Sample v, Sample w, DrawPoint p)
        {
            if (p.BarycentricCoordinates.U >= 0.9)
                return u.Color;
            if (p.BarycentricCoordinates.V >= 0.9)
                return v.Color;
            if (p.BarycentricCoordinates.W >= 0.9)
                return w.Color;


            int valU = (int)(p.Point.X + p.Point.Y + p.BarycentricCoordinates.U + r.Next(0, 4)) % 4;
            int valV = (int)(p.Point.X + p.Point.Y + p.BarycentricCoordinates.V + r.Next(0, 4)) % 4;
            int valW = (int)(p.Point.X + p.Point.Y + p.BarycentricCoordinates.W + r.Next(0, 4)) % 4;

            if (p.BarycentricCoordinates.U >= p.BarycentricCoordinates.V && p.BarycentricCoordinates.U >= p.BarycentricCoordinates.W)
                return ditherFurther(u.Color, v.Color, w.Color, valU);
            if (p.BarycentricCoordinates.V >= p.BarycentricCoordinates.W)
                return ditherFurther(v.Color, w.Color, w.Color, valV);

            return ditherFurther(w.Color, v.Color, u.Color, valW);
        }
        private static Pixel ditherFurther(Pixel a, Pixel b, Pixel c, int val)
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
