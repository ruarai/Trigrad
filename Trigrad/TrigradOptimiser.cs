using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
            int o = 0;
            foreach (var sample in samples)
            {
                minimiseSample(sample, options.Resamples, original, options.Grader);

                o++;

                if (o % 1000 == 0)
                    Console.WriteLine("{0}/{1}", o, samples.Count);
            }
        }


        private static void minimiseSample(Sample s, int resamples, PixelMap original, IGrader grader)
        {
            if (s.Point.X == 0 || s.Point.Y == 0)
                return;

            if (s.Point.X == original.Width - 1 || s.Point.Y == original.Height - 1)
                return;

            var curPoints = s.Points;

            double minError = errorPolygon(s, original, grader);
            Point bestPoint = s.Point;


            int count = curPoints.Count;
            int skip = count / resamples;
            if (skip == 0)
                skip = 1;

            foreach (var drawPoint in curPoints.Where((x, i) => i % skip == 0))
            {
                s.Point = drawPoint.Point;

                TriangleRasterization.CalculateMesh(s.Triangles);

                double error = errorPolygon(s, original, grader);
                if (error < minError)
                {
                    bestPoint = drawPoint.Point;
                    minError = error;
                }
            }

            s.Point = bestPoint;
        }


        private static double errorPolygon(Sample s, PixelMap original, IGrader grader)
        {
            double error = 0d;
            foreach (var t in s.Triangles)
            {
                t.U.Color = original[t.U.Point];
                t.V.Color = original[t.V.Point];
                t.W.Color = original[t.W.Point];

                foreach (var drawPoint in t.Points)
                {
                    Pixel gradedColor = grader.Grade(t.U, t.V, t.W, drawPoint);
                    Pixel originalColor = original[drawPoint.Point];

                    error += Math.Abs(gradedColor.R - originalColor.R);
                    error += Math.Abs(gradedColor.G - originalColor.G);
                    error += Math.Abs(gradedColor.B - originalColor.B);
                }
            }
            return error;
        }

    }
}
