using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelMapSharp;
using TriangleNet.Data;
using TriangleNet.Geometry;
using Point = System.Drawing.Point;

namespace Trigrad.DataTypes
{
    public class SampleTri
    {
        public SampleTri()
        {
            
        }

        public SampleTri(ITriangle t)
        {
            Vertex u = t.GetVertex(0);
            Vertex v = t.GetVertex(1);
            Vertex w = t.GetVertex(2);


            U = new Sample(u.Point(), new Pixel(Color.Black));
            V = new Sample(v.Point(), new Pixel(Color.Black));
            W = new Sample(w.Point(), new Pixel(Color.Black));
        }

        public IEnumerable<Sample> Samples
        {
            get { return new[] {U, V, W}.ToList(); }
        }

        public List<DrawPoint> Points = new List<DrawPoint>();

        public Sample U;
        public Sample V;
        public Sample W;

        public bool Busy;

        public Point CenterPoint
        {
            get { return new Point((int) Samples.Average(s => s.Point.X), (int) Samples.Average(s => s.Point.Y)); }
        }
        public Pixel CenterColor;

        public List<ITriangle> TriangleNeighbours = new List<ITriangle>();
        public List<SampleTri> SampleTriNeighbours = new List<SampleTri>();

        public override string ToString()
        {
            return string.Format("({0}),({1}),({2})", U.Point, V.Point, W.Point);
        }
    }
}
