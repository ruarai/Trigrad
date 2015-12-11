using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelMapSharp;
using Trigrad.ColorGraders;
using Trigrad.DataTypes;

namespace Trigrad.Renderers
{
    public class GradeFill : IFill
    {
        public GradeFill(IGrader grader)
        {
            Grader = grader;
        }

        public IGrader Grader;

        public void Fill(SampleTri t, PixelMap map)
        {
            foreach (var drawPoint in t.Points)
            {
                Pixel gradedColor = Grader.Grade(t.U, t.V, t.W, drawPoint);

                map[drawPoint.Point] = gradedColor;
            }
        }
    }
}
