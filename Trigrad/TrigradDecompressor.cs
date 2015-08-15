using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Data;
using TriangleNet.Geometry;
using Trigrad.ColorGraders;
using Trigrad.DataTypes;
using Trigrad.DataTypes.Compression;
using Point = System.Drawing.Point;

namespace Trigrad
{
    /// <summary> Holds methods for decompressing trigrad compressed imagery. </summary>
    public static class TrigradDecompressor
    {
        /// <summary> Decompresses a trigrad compressed bitmap. </summary>
        /// <param name="compressionData"> The TrigradCompressed data.</param>
        /// <param name="original"> The original image to determine the most effect fill mode.</param>
        /// <param name="debug"> Bool specifying whether a debug output will be produced.</param>
        public static TrigradDecompressed DecompressBitmap(TrigradCompressed compressionData, PixelMap original, bool debug = false)
        {
            TrigradDecompressed decompressed = new TrigradDecompressed(compressionData.Width, compressionData.Height);

            //build the triangle mesh
            decompressed.Mesh = buildMesh(compressionData.SampleTable);
            decompressed.MeshOutput.Bitmap.Save("tests\\mesh_output1.png");

            int o = 0;
            int j = 0;
            foreach (var triangle in decompressed.Mesh)
            {
                if (!triangle.U.OnEdge(compressionData.Width, compressionData.Height))
                {
                    var curPoints = triangle.U.GetPoints();

                    double minError = errorPolygon(triangle.U, original);
                    Point bestPoint = triangle.U.Point;


                    int count = curPoints.Count;
                    int skip = count / 10;
                    if (skip == 0)
                        skip = 1;

                    //if (o == 2)
                    //    foreach (var drawPoint in curPoints)
                    //        decompressed.Output[drawPoint.Point] = Color.HotPink;

                    foreach (var drawPoint in curPoints.Where((x, i) => i % skip == 0))
                    {
                        //Console.WriteLine(curPoints.Count);

                        //var point = curPoints[i];
                        triangle.U.Point = drawPoint.Point;

                        triangle.U.Recalculate();

                        double error = errorPolygon(triangle.U, original);
                        if (error < minError)
                        {
                            bestPoint = drawPoint.Point;
                            minError = error;
                        }
                    }

                    //var diff = triangle.U.Point - new Size(bestPoint);
                    //if (diff.X != 0 && diff.Y != 0)
                    //    Console.WriteLine(diff);

                    //Console.WriteLine(bestPoint);

                    triangle.U.Point = bestPoint;
                }
                o++;

                if (o % 1000 == 0)
                    Console.WriteLine(o);

                if (o%100 == 0)
                {
                    decompressed.MeshOutput.Bitmap.Save("tests\\frames\\" + j + ".png");
                    j++;
                }
            }


            Parallel.ForEach(decompressed.Mesh, triangle =>
            {
                triangle.U.Color = original[triangle.U.Point];
                triangle.V.Color = original[triangle.V.Point];
                triangle.W.Color = original[triangle.W.Point];

                triangle.Recalculate();

                fillTriangle(triangle, decompressed.Output);
            });


            return decompressed;
        }


        static IGrader grader = new BarycentricGrader();
        private static void fillTriangle(SampleTri t, PixelMap map)
        {
            foreach (var drawPoint in t.Points)
            {
                var coords = drawPoint.BarycentricCoordinates;

                Color gradedColor = grader.Grade(t.U.Color, t.V.Color, t.W.Color, coords.U, coords.V, coords.W, drawPoint.Point.X, drawPoint.Point.Y, t.U.Point, t.V.Point, t.W.Point);

                //Color gradedColor = Color.FromArgb((byte)(coords.U * 255), (byte)(coords.V * 255), (byte)(coords.W * 255));

                map[drawPoint.Point] = gradedColor;
            }
        }

        private static double errorPolygon(Sample s, PixelMap original)
        {
            double error = 0d;
            foreach (var t in s.Triangles)
            {
                t.U.Color = original[t.U.Point];
                t.V.Color = original[t.V.Point];
                t.W.Color = original[t.W.Point];

                //Console.WriteLine(t.Points.Count());
                foreach (var drawPoint in t.Points)
                {
                    var coords = drawPoint.BarycentricCoordinates;
                    Color gradedColor = grader.Grade(t.U.Color, t.V.Color, t.W.Color, coords.U, coords.V, coords.W, drawPoint.Point.X, drawPoint.Point.Y, t.U.Point, t.V.Point, t.W.Point);
                    Color originalColor = original[drawPoint.Point];


                    error += Math.Abs(gradedColor.R - originalColor.R);
                    error += Math.Abs(gradedColor.G - originalColor.G);
                    error += Math.Abs(gradedColor.B - originalColor.B);

                }
            }
            return error;
        }

        private static List<SampleTri> buildMesh(Dictionary<Point, Color> pointIndex)
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

            int o = 0;
            Parallel.ForEach(sampleMesh, tri =>
            {
                IEnumerable<SampleTri> treeTris = breadthFirstTopDownTraversal(tri, n => n.SampleTriNeighbours, 100);

                lock (tri.U)
                    foreach (var tri2 in treeTris)
                    {
                        lock (tri2)
                            if (Equals(tri.U, tri2.U) || Equals(tri.U, tri2.V) || Equals(tri.U, tri2.W))
                            {
                                tri.U.Triangles.Add(tri2);
                            }
                            else if (Equals(tri.V, tri2.U) || Equals(tri.V, tri2.V) || Equals(tri.V, tri2.W))
                            {
                                tri.U.Triangles.Add(tri2);
                            }
                            else if (Equals(tri.W, tri2.U) || Equals(tri.W, tri2.V) || Equals(tri.W, tri2.W))
                            {
                                tri.U.Triangles.Add(tri2);
                            }
                    }
                if (o % 1000 == 0)
                    Console.WriteLine("{0}/{1}", o, sampleMesh.Count);
                o++;
            });

            return sampleMesh;

        }

        static IEnumerable<T> breadthFirstTopDownTraversal<T>(T root, Func<T, IEnumerable<T>> children,int count)
        {
            var q = new Queue<T>();
            q.Enqueue(root);
            while (q.Count > 0)
            {
                T current = q.Dequeue();
                yield return current;
                foreach (var child in children(current))
                {
                    q.Enqueue(child);

                    if (q.Count >= count)
                        yield break;
                }
            }
        }
    }
}
