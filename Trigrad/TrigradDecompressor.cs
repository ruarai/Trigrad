using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using TriangleNet;
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
        static List<IGrader> graders = new List<IGrader>();
        static List<Color> graderLegend = new List<Color>();
        static TrigradDecompressor()
        {
            graders.Add(new BarycentricGrader());
            graderLegend.Add(Color.Black);
            graders.Add(new DitherGrader());
            graderLegend.Add(Color.HotPink);
            graders.Add(new TriGrader());
            graderLegend.Add(Color.OrangeRed);
            graders.Add(new FillGrader());
            graderLegend.Add(Color.DarkBlue);
            graders.Add(new AverageGrader());
            graderLegend.Add(Color.LawnGreen);
        }

        /// <summary> Decompresses a trigrad compressed bitmap. </summary>
        /// <param name="compressionData"> The TrigradCompressed data.</param>
        /// <param name="original"> The original image to determine the most effect fill mode.</param>
        /// <param name="debug"> Bool specifying whether a debug output will be produced.</param>
        public static TrigradDecompressed DecompressBitmap(TrigradCompressed compressionData, Bitmap original, bool debug = false)
        {
            TrigradDecompressed decompressed = new TrigradDecompressed(compressionData.Width, compressionData.Height);

            //build the triangle mesh
            decompressed.Mesh = buildMesh(compressionData.SampleTable);

            Parallel.ForEach(decompressed.Mesh.Triangles, triangle =>
            {
                var vU = triangle.GetVertex(0);
                var vV = triangle.GetVertex(1);
                var vW = triangle.GetVertex(2);

                //find sample points of triangle
                Color cU = compressionData.SampleTable[vU.Point()];
                Color cV = compressionData.SampleTable[vV.Point()];
                Color cW = compressionData.SampleTable[vW.Point()];

                //rasterize triangle to find points to fill
                var points = TriangleRasterization.PointsInTriangle(vU.Point(), vV.Point(), vW.Point());

                Dictionary<Point, Color> originalColors = new Dictionary<Point, Color>();

                foreach (var point in points)
                {
                    lock (original)
                        originalColors.Add(point, original.GetPixel(point.X, point.Y));
                }

                int minError = int.MaxValue;
                Dictionary<Point, Color> bestColors = new Dictionary<Point, Color>();

                int i = 0;
                int best = 0;
                foreach (var grader in graders)
                {
                    Dictionary<Point, Color> pixelMap = new Dictionary<Point, Color>();
                    foreach (var point in points)
                    {
                        var coords = Barycentric.GetCoordinates(point, vU, vV, vW);

                        Color gradedColor = grader.Grade(cU, cV, cW, coords.U, coords.V, coords.W, point.X, point.Y, vU.Point(), vV.Point(), vW.Point());

                        pixelMap[point] = gradedColor;
                    }

                    int error = pointError(points, originalColors, pixelMap);

                    if (error < minError)
                    {
                        minError = error;
                        bestColors = pixelMap;
                        best = i;
                    }
                    i++;
                }

                foreach (var point in points)
                {
                    lock (decompressed.Output)
                        decompressed.Output.SetPixel(point.X, point.Y, bestColors[point]);

                    lock (decompressed.DebugOutput)
                        decompressed.DebugOutput.SetPixel(point.X, point.Y, graderLegend[best]);
                }
            });

            return decompressed;
        }

        private static int pointError(IEnumerable<Point> points, Dictionary<Point, Color> a, Dictionary<Point, Color> b)
        {
            int error = 0;
            foreach (var point in points)
            {
                Color cA = a[point];
                Color cB = b[point];

                error += Math.Abs(cA.R - cB.R);
                error += Math.Abs(cA.G - cB.G);
                error += Math.Abs(cA.B - cB.B);
            }
            return error;
        }

        private static Mesh buildMesh(Dictionary<Point, Color> pointIndex)
        {
            InputGeometry g = new InputGeometry();
            foreach (var value in pointIndex)
            {
                g.AddPoint(value.Key.X, value.Key.Y);
            }

            Mesh m = new Mesh();
            m.Triangulate(g);
            return m;
        }
    }
}
