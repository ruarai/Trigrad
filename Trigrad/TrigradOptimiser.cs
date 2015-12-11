using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PixelMapSharp;
using TriangleNet;
using TriangleNet.Data;
using TriangleNet.Geometry;
using Trigrad.ColorGraders;
using Trigrad.DataTypes;
using Trigrad.DataTypes.Compression;
using Point = System.Drawing.Point;

namespace Trigrad
{
    public delegate void OptimiserProgressUpdate(double progress);

    public static class TrigradOptimiser
    {
        public static event OptimiserProgressUpdate OnUpdate;


        public static void OptimiseMesh(TrigradCompressed compressionData, PixelMap original, TrigradOptions options)
        {
            var mesh = compressionData.Mesh;
            GPUT.CalculateMesh(mesh);

            var samples = mesh.SelectMany(t => t.Samples).Distinct().ToList();

            for (int i = 0; i < options.Iterations; i++)
            {
                minimiseMesh(samples, options, original);

                Console.WriteLine("{0}/{1}", i, options.Iterations);
            }

            compressionData.Mesh = mesh;
        }


        static void minimiseMesh(List<Sample> samples, TrigradOptions options, PixelMap original)
        {
            tempMap = new PixelMap(original.Width,original.Height);

            int o = 0;
            int count = samples.Count;
            foreach (var sample in samples)
            {
                minimiseSample(sample, options.Resamples, original, options);

                o++;

                if (o%1000 == 0)
                    Console.WriteLine("{0}/{1}", o, samples.Count);

                if (o%100 == 0 && OnUpdate != null)
                    OnUpdate((double)o/count);
            }
        }


        private static void minimiseSample(Sample s, int resamples, PixelMap original, TrigradOptions options)
        {
            if (s.Point.X == 0 || s.Point.Y == 0)
                return;

            if (s.Point.X == original.Width - 1 || s.Point.Y == original.Height - 1)
                return;


            var curPoints = s.Points;

            double minError = errorPolygon(s, original, options);
            Point bestPoint = s.Point;

            if (polygonConvex(s))
                return;

            int count = curPoints.Count;
            int skip = count / resamples;
            if (skip == 0)
                skip = 1;

            foreach (var drawPoint in curPoints.Where((x, i) => i % skip == 0))
            {
                s.Point = drawPoint.Point;

                TriangleRasterization.CalculateMesh(s.Triangles);

                double error = errorPolygon(s, original, options);
                if (error < minError)
                {
                    bestPoint = drawPoint.Point;
                    minError = error;
                }
            }

            s.Point = bestPoint;
            TriangleRasterization.CalculateMesh(s.Triangles);
        }

        private static PixelMap tempMap;
        private static double errorPolygon(Sample s, PixelMap original, TrigradOptions options)
        {
            if(options.ResampleColors)
            foreach (var sample in s.Samples)
            {
                sample.Color = original[sample.Point];
            }

            double error = 0d;
            foreach (var t in s.Triangles)
            {
                t.CenterColor = original[t.CenterPoint];

                options.Renderer.Fill(t,tempMap);

                foreach (var drawPoint in t.Points)
                {
                    Pixel a = original[drawPoint.Point];
                    Pixel b = tempMap[drawPoint.Point];

                    Pixel diff = a - b;

                    error += diff.R;
                    error += diff.G;
                    error += diff.B;
                }
            }
            return error;
        }

        private static bool polygonConvex(Sample s)
        {
            List<Point> outerPolygonPoints = s.Samples.Except(new[]{s}).Select(sample=>sample.Point).ToList();

            bool got_negative = false;
            bool got_positive = false;
            int num_points = outerPolygonPoints.Count();
            int B, C;
            for (int A = 0; A < num_points; A++)
            {
                B = (A + 1) % num_points;
                C = (B + 1) % num_points;

                float cross_product =
                    crossProductMagnitude(
                        outerPolygonPoints[A].X, outerPolygonPoints[A].Y,
                        outerPolygonPoints[B].X, outerPolygonPoints[B].Y,
                        outerPolygonPoints[C].X, outerPolygonPoints[C].Y);
                if (cross_product < 0)
                {
                    got_negative = true;
                }
                else if (cross_product > 0)
                {
                    got_positive = true;
                }
                if (got_negative && got_positive) return false;
            }

            // If we got this far, the polygon is convex.
            return true;
        }
        private static float crossProductMagnitude(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }

    }
}
