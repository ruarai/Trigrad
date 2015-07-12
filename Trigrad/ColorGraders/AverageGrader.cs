using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trigrad.ColorGraders
{
    /// <summary> Color grader that fills the triangle with an average of the three vertice samples. </summary>
    public class AverageGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Color Grade(Color cU, Color cV, Color cW, double u, double v, double w, int x, int y, Point pU, Point pV, Point pW)
        {
            int R = cU.R + cV.R + cW.R;
            int G = cU.G + cV.G + cW.G;
            int B = cU.B + cV.B + cW.B;

            return Color.FromArgb((byte) (R/3), (byte) (G/3), (byte) (B/3));
        }
    }
}
