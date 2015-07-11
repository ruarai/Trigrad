using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trigrad.ColorGraders
{
    public class AverageGrader : IGrader 
    {
        public Color Grade(Color cU, Color cV, Color cW, double u, double v, double w, int x, int y, Point pU, Point pV, Point pW)
        {
            int R = cU.R + cV.R + cW.R;
            int G = cU.G + cV.G + cW.G;
            int B = cU.B + cV.B + cW.B;

            return Color.FromArgb((byte) (R/3), (byte) (G/3), (byte) (B/3));
        }
    }
}
