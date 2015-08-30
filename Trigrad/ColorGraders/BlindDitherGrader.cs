using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigrad.DataTypes;

namespace Trigrad.ColorGraders
{
    /// <summary> Color grader that fills a triangle with a dithering that ignores the barycentric coordinates. </summary>
    public class BlindDitherGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Color Grade(Sample u, Sample v, Sample w, DrawPoint p)
        {
            int val = (p.Point.X + p.Point.Y) % 3;
            if (val == 0)
                return u.Color;
            if (val == 1)
                return v.Color;
            return w.Color;

        }
    }
}
