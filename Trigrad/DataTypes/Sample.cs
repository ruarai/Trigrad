using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trigrad.DataTypes
{

    public class Sample
    {
        public Sample(Point p, Color c)
        {
            Point = p;
            Color = c;
        }

        public Point Point;
        public Color Color;
        public Color NewColor;

        public bool Optimised = true;

        public List<SampleTri> Triangles = new List<SampleTri>();

        public IEnumerable<Sample> Samples
        {
            get { return Triangles.SelectMany(t => t.Samples); }
        }

        public List<DrawPoint> Points { get { return Triangles.SelectMany(t => t.Points).ToList(); } } 

        public List<Sample> DepthNeighbours(int depth)
        {
            return depthSelect(this, depth);
        }

        private static List<Sample> depthSelect(Sample sample, int depth, List<Sample> parents = null)
        {
            if (depth <= 0)
                return new List<Sample>();

            List<Sample> samples = new List<Sample>();
            foreach (var child in sample.Samples)
            {
                if (parents != null && parents.Contains(child))
                    continue;

                samples.Add(child);
                var result = depthSelect(child, depth - 1, samples);

                samples.AddRange(result);
            }
            return samples;
        }


        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Sample s = obj as Sample;
            if (s == null)
                return false;

            // Return true if the fields match:
            return (Point == s.Point);
        }
        public override int GetHashCode()
        {
            return Point.GetHashCode();
        }

        public bool OnEdge(int width, int height)
        {
            return (Point.X == 0 || Point.Y == 0 ||
                Point.Y == width - 1 || Point.Y == height - 1);
        }
    }
}
