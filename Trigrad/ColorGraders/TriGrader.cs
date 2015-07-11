using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trigrad.ColorGraders
{
    public class TriGrader : IGrader
    {
        Color IGrader.Grade(Color cU, Color cV, Color cW, double u, double v, double w, int x, int y, Point pU, Point pV, Point pW)
        {
            int uSum = pU.X + pU.Y;
            int vSum = pV.X + pV.Y;
            int wSum = pW.X + pW.Y;


            if (uSum > vSum && uSum > wSum)
            {
                return cU;
            }
            else if (vSum >wSum)
                return cV;
            else
                return cW;
        }
    }
}
