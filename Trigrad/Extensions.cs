using System.Collections.Generic;
using TriangleNet.Data;
using System.Drawing;
using TriangleNet;
using Trigrad.DataTypes;

namespace Trigrad
{
    public static class Extensions
    {
        public static Point Point(this Vertex t)
        {
            return new Point((int)t.X, (int)t.Y);
        }

        public static PixelMap DrawMesh(this List<SampleTri> m,int width,int height)
        {
            PixelMap map = new PixelMap(width,height);

            foreach (var sampleTri in m)
            {
                map.DrawLine(sampleTri.U.Point, sampleTri.V.Point, sampleTri.U.Color);
                map.DrawLine(sampleTri.V.Point, sampleTri.W.Point, sampleTri.V.Color);
                map.DrawLine(sampleTri.W.Point, sampleTri.U.Point, sampleTri.W.Color);
            }

            return map;
        }
    }
}
