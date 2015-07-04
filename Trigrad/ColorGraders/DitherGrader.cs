using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trigrad.ColorGraders
{
    /// <summary> Color grader that fills a triangle with a dithered output. </summary>
    public class DitherGrader : IGrader
    {
        /// <summary> Produces a color from the specified coordinates and colors. </summary>
        public Color Grade(Color cU, Color cV, Color cW, double u, double v, double w, int x, int y)
        {
            int val = (x + y) % 4;

            if (u >= v && u >= w)
                return ditherFurther(cU, cV, cW, val);
            if (v >= w)
                return ditherFurther(cV, cU, cW, val);

            return ditherFurther(cW, cV, cU, val);
        }
        private static Color ditherFurther(Color a, Color b, Color c, int val)
        {
            switch (val)
            {
                case 0:
                    return a;
                case 1:
                    return b;
                case 2:
                    return a;
                case 3:
                    return c;
                default:
                    return a;
            }
        }
    }
}
