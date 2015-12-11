using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelMapSharp;
using Trigrad.DataTypes;

namespace Trigrad.Renderers
{
    public class CenterFill : IFill
    {
        public void Fill(SampleTri t, PixelMap map)
        {
            foreach (var drawPoint in t.Points)
            {
                Pixel gradedColor = t.CenterColor;

                map[drawPoint.Point] = gradedColor;
            }
        }
    }
}
