using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Geometry;
using Trigrad.DataTypes;
using Point = System.Drawing.Point;

namespace Trigrad
{
    static class MeshBuilder
    {
        public static List<SampleTri> BuildMesh(Dictionary<Point, Color> pointIndex)
        {
            InputGeometry g = new InputGeometry();
            foreach (var value in pointIndex)
            {
                g.AddPoint(value.Key.X, value.Key.Y);
            }

            Mesh m = new Mesh();
            m.Triangulate(g);

            List<SampleTri> sampleMesh = new List<SampleTri>();

            Dictionary<ITriangle, SampleTri> table = new Dictionary<ITriangle, SampleTri>();

            Dictionary<Point, Sample> sampleTable = new Dictionary<Point, Sample>();

            foreach (var mTri in m.Triangles)
            {
                SampleTri tri = new SampleTri(mTri);

                for (int i = 0; i < 3; i++)
                    tri.TriangleNeighbours.Add(mTri.GetNeighbor(i));

                sampleMesh.Add(tri);
                table.Add(mTri, tri);

                if (sampleTable.ContainsKey(tri.U.Point))
                    tri.U = sampleTable[tri.U.Point];
                else
                    sampleTable[tri.U.Point] = tri.U;

                if (sampleTable.ContainsKey(tri.V.Point))
                    tri.V = sampleTable[tri.V.Point];
                else
                    sampleTable[tri.V.Point] = tri.V;

                if (sampleTable.ContainsKey(tri.W.Point))
                    tri.W = sampleTable[tri.W.Point];
                else
                    sampleTable[tri.W.Point] = tri.W;
            }

            foreach (var tri in sampleMesh)
            {
                foreach (var triangleNeighbour in tri.TriangleNeighbours)
                {
                    if (triangleNeighbour != null)
                        tri.SampleTriNeighbours.Add(table[triangleNeighbour]);
                }
            }
            foreach (var tri in sampleMesh)
            {
                tri.U.Triangles.Add(tri);
                tri.V.Triangles.Add(tri);
                tri.W.Triangles.Add(tri);

                tri.U.Color = pointIndex[tri.U.Point];
                tri.V.Color = pointIndex[tri.V.Point];
                tri.W.Color = pointIndex[tri.W.Point];
            }

            return sampleMesh;
        }
    }
}
