using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigrad.DataTypes;

namespace Trigrad.Filters
{
    public class AreaFilter : ITriGradFilter
    {
        public AreaFilter(int depth)
        {
            Depth = depth;
        }

        public int Depth;

        public void Run(List<SampleTri> mesh)
        {
            double s = 2;
            Parallel.ForEach(mesh.SelectMany(m => m.Samples).ToList(), sample =>
            {
                Point p = sample.Point;

                sample.Point = new Point(p.X + (int)(Math.Sin(p.X / s) * s), p.Y);
            });
        }
    }
}
