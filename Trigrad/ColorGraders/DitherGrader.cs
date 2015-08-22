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
        public Color Grade(Color cU, Color cV, Color cW, double u, double v, double w, int x, int y, Point pU, Point pV, Point pW)
        {
            if (u >= 0.9)
                return cU;
            if (v >= 0.9)
                return cV;
            if (w >= 0.9)
                return cW;


            int valU = (int)(x + y + u * 79) % 4;
            int valV = (int)(x + y + v * 79) % 4;
            int valW = (int)(x + y + w * 79) % 4;

            if (u >= v && u >= w)
                return ditherFurther(cU, cV, cW, valU);
            if (v >= w)
                return ditherFurther(cV, cU, cW, valV);

            return ditherFurther(cW, cV, cU, valW);
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
