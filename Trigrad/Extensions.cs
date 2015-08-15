using System.Collections.Generic;
using TriangleNet.Data;
using System.Drawing;
using TriangleNet;
using Trigrad.DataTypes;

namespace Trigrad
{
    internal static class Extensions
    {
        public static Point Point(this Vertex t)
        {
            return new Point((int)t.X, (int)t.Y);
        }

        public static PixelMap ToBitmap(this List<SampleTri> mesh, int width, int height)
        {
            PixelMap b = new PixelMap(width, height);
            foreach (var tri in mesh)
            {
                b.DrawLine(tri.U.Point, tri.V.Point, Color.Red);
                b.DrawLine(tri.V.Point, tri.W.Point, Color.Blue);
                b.DrawLine(tri.W.Point, tri.U.Point, Color.Green);
            }

            return b;
        }
    }
}
