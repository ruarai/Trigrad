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
    /// <summary> Color grader that fills a triangle with each point's nearest color. </summary>
    public class FillGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Pixel Grade(Sample u, Sample v, Sample w, DrawPoint p)
        {
            if (p.BarycentricCoordinates.U >= p.BarycentricCoordinates.V && p.BarycentricCoordinates.U >= p.BarycentricCoordinates.W)
                return u.Color;
            if (p.BarycentricCoordinates.V >= p.BarycentricCoordinates.W)
                return v.Color;
            return w.Color;
        }
    }
}
