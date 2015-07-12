using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trigrad.ColorGraders
{
    /// <summary> Color grader that fills the triangle with the highest valued vertice sample. </summary>
    public class TopGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Color Grade(Color cU, Color cV, Color cW, double u, double v, double w, int x, int y, Point pU, Point pV, Point pW)
        {
            int uSum = cU.R + cU.G + cU.B;
            int vSum = cV.R + cV.G + cV.B;
            int wSum = cW.R + cW.G + cW.B;

            if (uSum > vSum && uSum > wSum)
            {
                return cU;
            }
            else if (vSum > wSum)
                return cV;
            else
                return cW;

        }
    }
}
