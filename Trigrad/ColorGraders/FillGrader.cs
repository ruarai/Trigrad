using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigrad.DataTypes;

namespace Trigrad.ColorGraders
{
    /// <summary> Color grader that fills a triangle with each point's nearest color. </summary>
    public class FillGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Color Grade(Color cU, Color cV, Color cW, BarycentricCoordinates coords, Point p, Point pU, Point pV, Point pW)
        {
            if (coords.U >= coords.V && coords.U >= coords.W)
                return cU;
            if (coords.V >= coords.W)
                return cV;
            return cW;
        }
    }
}
