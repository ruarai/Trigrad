using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trigrad.ColorGraders
{
    public class BottomGrader : IGrader
    {
        public Color Grade(Color cU, Color cV, Color cW, double u, double v, double w, int x, int y, Point pU, Point pV, Point pW)
        {
            int uSum = cU.R + cU.G + cU.B;
            int vSum = cV.R + cV.G + cV.B;
            int wSum = cW.R + cW.G + cW.B;

            if (uSum < vSum && uSum < wSum)
            {
                return cU;
            }
            else if (vSum < wSum)
                return cV;
            else
                return cW;

        }
    }
}
