using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet.Data;
using TriangleNet.Geometry;
using Point = System.Drawing.Point;

namespace Trigrad.DataTypes
{
    class SampleTri
    {
        public SampleTri(ITriangle t)
        {
            Vertex u = t.GetVertex(0);
            Vertex v = t.GetVertex(1);
            Vertex w = t.GetVertex(2);


            U = new Sample(u.Point(), Color.Black);
            V = new Sample(v.Point(), Color.Black);
            W = new Sample(w.Point(), Color.Black);

            Recalculate();
        }

        public void Recalculate()
        {
            Points = TriangleRasterization.PointsInTriangle(U.Point, V.Point, W.Point);
        }

        public IEnumerable<DrawPoint> Points;

        public Sample U;
        public Sample V;
        public Sample W;

        public List<ITriangle> TriangleNeighbours = new List<ITriangle>();
        public List<SampleTri> SampleTriNeighbours = new List<SampleTri>();

        public override string ToString()
        {
            return string.Format("({0}),({1}),({2})", U.Point, V.Point, W.Point);
        }
    }

    class Sample
    {
        public Sample(Point p, Color c)
        {
            Point = p;
            Color = c;
        }

        public Point Point;
        public Color Color;

        public List<SampleTri> Triangles = new List<SampleTri>();

        public List<DrawPoint> GetPoints()
        {
            Recalculate();

            return Triangles.SelectMany(p => p.Points).ToList();
        }

        public void Recalculate()
        {
            foreach (var sampleTri in Triangles)
                sampleTri.Recalculate();
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

        public bool OnEdge(int width,int height)
        {
            return (Point.X == 0 || Point.Y == 0 ||
                Point.Y == width - 1 || Point.Y == height - 1) ;
        }
    }
}
