using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trigrad.ColorGraders
{
    /// <summary> Color grader that fills a triangle with a dithering that ignores the barycentric coordinates. </summary>
    public class BlindDitherGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Color Grade(Color cU, Color cV, Color cW, double u, double v, double w, int x, int y)
        {
            int val = (x + y) % 3;
            if (val == 0)
                return cU;
            if (val == 1)
                return cV;
            return cW;

        }
    }
}
