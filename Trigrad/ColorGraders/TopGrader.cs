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
    /// <summary> Color grader that fills the triangle with the highest valued vertice sample. </summary>
    public class TopGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Pixel Grade(Sample u, Sample v, Sample w, DrawPoint p)
        {
            int uSum = u.Color.R + u.Color.G + u.Color.B;
            int vSum = v.Color.R + v.Color.G + v.Color.B;
            int wSum = w.Color.R + w.Color.G + w.Color.B;

            if (uSum > vSum && uSum > wSum)
            {
                return u.Color;
            }
            else if (vSum > wSum)
                return v.Color;
            else
                return w.Color;

        }
    }
}
