using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trigrad.ColorGraders
{
    /// <summary> Color grader that fills a triangle with each point's nearest color. </summary>
    public class FillGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Color Grade(Color cU, Color cV, Color cW, double u, double v, double w,int x,int y)
        {
            if (u >= v && u >= w)
                return cU;
            if (v >= w)
                return cV;
            return cW;
        }
    }
}
