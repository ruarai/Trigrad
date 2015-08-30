using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigrad.DataTypes;

namespace Trigrad.ColorGraders
{
    /// <summary> Color grader that fills a triangle with a triangle gradient. </summary>
    public class BarycentricGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Color Grade(Color cU, Color cV, Color cW, BarycentricCoordinates coords, Point p, Point pU, Point pV, Point pW)
        {
            byte R = (byte)(cU.R * coords.U + cV.R * coords.V + cW.R * coords.W);
            byte G = (byte)(cU.G * coords.U + cV.G * coords.V + cW.G * coords.W);
            byte B = (byte)(cU.B * coords.U + cV.B * coords.V + cW.B * coords.W);

            return Color.FromArgb(R, G, B);
        }
    }
}
