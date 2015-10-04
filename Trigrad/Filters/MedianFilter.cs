using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigrad.DataTypes;
using System.Drawing;

namespace Trigrad.Filters
{
    public class MedianFilter : ITriGradFilter
    {
        public MedianFilter(int depth)
        {
            Depth = depth;
        }

        public int Depth;
        public void Run(List<SampleTri> mesh)
        {
            Parallel.ForEach(mesh.SelectMany(m => m.Samples).Distinct().ToList(), sample =>
            {
                var neighbours = sample.DepthNeighbours(Depth);

                var colors = neighbours.Select(s => s.Color).OrderByDescending(c => c.GetSaturation()).ToList();

                sample.NewColor = colors.Last();
            });
            Parallel.ForEach(mesh.SelectMany(m => m.Samples).Distinct().ToList(), sample =>
            {
                sample.Color = sample.NewColor;
            });
        }
    }
}
