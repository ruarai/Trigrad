using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trigrad.ColorGraders
{
    /// <summary> Color grader that fills a triangle with a triangle gradient. </summary>
    public class BarycentricGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Color Grade(Color cU, Color cV, Color cW, double u, double v, double w,int x,int y)
        {
            byte R = (byte)(cU.R * u + cV.R * v + cW.R * w);
            byte G = (byte)(cU.G * u + cV.G * v + cW.G * w);
            byte B = (byte)(cU.B * u + cV.B * v + cW.B * w);

            return Color.FromArgb(R, G, B);
        }
    }
}
