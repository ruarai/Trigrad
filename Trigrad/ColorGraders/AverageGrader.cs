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
    /// <summary> Color grader that fills the triangle with an average of the three vertice samples. </summary>
    public class AverageGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Pixel Grade(Sample u, Sample v, Sample w, DrawPoint p)
        {
            int R = u.Color.R + v.Color.R + w.Color.R;
            int G = u.Color.G + v.Color.G + w.Color.G;
            int B = u.Color.B + v.Color.B + w.Color.B;

            return new Pixel((byte) (R/3), (byte) (G/3), (byte) (B/3));
        }
    }
}
