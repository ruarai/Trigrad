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
        /// <param name="options"> Options dictating the decompression.</param>
        public static TrigradDecompressed DecompressBitmap(TrigradCompressed compressionData, PixelMap original, TrigradOptions options)
        {
            TrigradDecompressed decompressed = new TrigradDecompressed(compressionData.Width, compressionData.Height);

            //build the triangle mesh
            decompressed.Mesh = buildMesh(compressionData.SampleTable);

            Console.WriteLine("Built mesh.");
            decompressed.MeshOutput.Bitmap.Save("tests\\mesh_output1.png");


            var samples = decompressed.Mesh.SelectMany(t => t.Samples).Distinct().ToList();

            for (int i = 0; i < options.Iterations; i++)
            {
                minimiseMesh(samples, options, original);
                decompressed.MeshOutput.Bitmap.Save("tests\\mesh_output_"+i+".png");
            }

            drawMesh(decompressed.Mesh, decompressed.Output, original);

            decompressed.MeshOutput.Bitmap.Save("tests\\mesh_output2.png");

            return decompressed;
        }

        static void minimiseMesh(List<Sample> samples,TrigradOptions options,PixelMap original)
        {
            int o = 0;
            int j = 0;
            foreach (var sample in samples)
            {
                minimiseSample(sample, options.Resamples, original);

                o++;

                if (o % 1000 == 0)
                    Console.WriteLine("{0}/{1}", o, samples.Count);

                if (o % 20 == 0)
                {
                    //drawMesh(decompressed.Mesh, decompressed.Output, original);
                    //decompressed.Output.Bitmap.Save("tests\\frames\\" + j + ".png");
                    //decompressed.MeshOutput.Bitmap.Save("tests\\dframes\\" + j + ".png");
                    j++;
                }
            }
        }

        private static void drawMesh(List<SampleTri> mesh, PixelMap output, PixelMap original)
        {
            Parallel.ForEach(mesh, triangle =>
            {
                triangle.U.Color = original[triangle.U.Point];
                triangle.V.Color = original[triangle.V.Point];
                triangle.W.Color = original[triangle.W.Point];

                triangle.Recalculate();

                fillTriangle(triangle, output);
            });
        }

        private static void minimiseSample(Sample s, int resamples, PixelMap original)
        {
            if (s.Point.X == 0 || s.Point.Y == 0)
                return;

            if (s.Point.X == original.Width - 1 || s.Point.Y == original.Height - 1)
                return;

            var curPoints = s.GetPoints();

            double minError = errorPolygon(s, original);
            Point bestPoint = s.Point;


            int count = curPoints.Count;
            int skip = count / resamples;
            if (skip == 0)
                skip = 1;

            foreach (var drawPoint in curPoints.Where((x, i) => i % skip == 0))
            {
                s.Point = drawPoint.Point;

                s.Recalculate();

                double error = errorPolygon(s, original);
                if (error < minError)
                {
                    bestPoint = drawPoint.Point;
                    minError = error;
                }


                //Console.WriteLine(curPoints.Count);
            }

            s.Point = bestPoint;
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
            Parallel.ForEach(s.Triangles, t =>
            {
                t.U.Color = original[t.U.Point];
                t.V.Color = original[t.V.Point];
                t.W.Color = original[t.W.Point];

                //Console.WriteLine(t.Points.Count());
                foreach (var drawPoint in t.Points)
                {
                    var coords = drawPoint.BarycentricCoordinates;

                    Color gradedColor = grader.Grade(t.U.Color, t.V.Color, t.W.Color, coords.U, coords.V, coords.W,
                        drawPoint.Point.X, drawPoint.Point.Y, t.U.Point, t.V.Point, t.W.Point);
                    Color originalColor = original[drawPoint.Point];


                    error += Math.Abs(gradedColor.R - originalColor.R);
                    error += Math.Abs(gradedColor.G - originalColor.G);
                    error += Math.Abs(gradedColor.B - originalColor.B);
                }
            });
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
            foreach (var tri in sampleMesh)
            {
                tri.U.Triangles.Add(tri);
                tri.V.Triangles.Add(tri);
                tri.W.Triangles.Add(tri);
            }

            return sampleMesh;

        }
    }
}
