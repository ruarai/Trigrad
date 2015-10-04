using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigrad.DataTypes;

namespace Trigrad.Filters
{
    public class NoiseFilter :ITriGradFilter
    {
        public NoiseFilter(int depth)
        {
            Depth = depth;
        }

        public int Depth;
        private static Random r = new Random();
        public void Run(List<SampleTri> mesh)
        {
            Parallel.ForEach(mesh.SelectMany(m => m.Samples).Distinct().ToList(), sample =>
            {
                var neighbours = sample.DepthNeighbours(Depth);

                var colors = neighbours.ToList();

                sample.Color = colors[r.Next(0, colors.Count)].Color;
            });
        }
    }
}
