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
    /// <summary> Color grader that fills a triangle with a triangle gradient. </summary>
    public class BarycentricGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Pixel Grade(Sample u, Sample v, Sample w, DrawPoint p)
        {
            byte R = (byte)(u.Color.R * p.BarycentricCoordinates.U + v.Color.R * p.BarycentricCoordinates.V + w.Color.R * p.BarycentricCoordinates.W);
            byte G = (byte)(u.Color.G * p.BarycentricCoordinates.U + v.Color.G * p.BarycentricCoordinates.V + w.Color.G * p.BarycentricCoordinates.W);
            byte B = (byte)(u.Color.B * p.BarycentricCoordinates.U + v.Color.B * p.BarycentricCoordinates.V + w.Color.B * p.BarycentricCoordinates.W);

            return new Pixel(R, G, B);
        }
    }
}
