using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigrad.DataTypes;

namespace Trigrad.Filters
{
    public class GridFilter : ITriGradFilter
    {
        public GridFilter(int size)
        {
            Size = size;
        }

        public int Size;
        public void Run(List<SampleTri> mesh)
        {
            Parallel.ForEach(mesh.SelectMany(m => m.Samples).Distinct().ToList(), sample =>
            {
                sample.Point = new Point(sample.Point.X - sample.Point.X % Size, sample.Point.Y - sample.Point.Y % Size);
                //sample.Point = new Point(sample.Point.X + (int)(Math.Sin(sample.Point.X + sample.Point.Y) * (Size / 2)), sample.Point.Y + (int)(Math.Sin(sample.Point.X + sample.Point.Y) * (Size / 2)));
            });
        }
    }
}
