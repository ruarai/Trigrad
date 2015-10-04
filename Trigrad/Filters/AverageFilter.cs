using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigrad.DataTypes;

namespace Trigrad.Filters
{
    public class AverageFilter : ITriGradFilter
    {
        public AverageFilter(int depth)
        {
            Depth = depth;
        }

        public int Depth;

        public void Run(List<SampleTri> mesh)
        {
            Parallel.ForEach(mesh.SelectMany(m => m.Samples).ToList(), sample =>
            {
                var neighbours = sample.DepthNeighbours(Depth);

                var colors = neighbours.Select(s => s.Color);

                sample.Color = Color.FromArgb((int)colors.Average(c => c.R), (int)colors.Average(c => c.G),(int)colors.Average(c => c.B));
            });
        }
    }
}
