using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TriangleNet;
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
            GPUT.CalculateMesh(compressionData.Mesh);
            var samples = compressionData.Mesh.SelectMany(t => t.Samples).Distinct().ToList();

            for (int i = 0; i < options.Iterations; i++)
            {
                minimiseMesh(samples, options, original);

            }
        }

        private static int tasksRunning = 0;
        static void minimiseMesh(List<Sample> samples, TrigradOptions options, PixelMap original)
        {
            samples.ForEach(s => s.Optimised = sampleOnEdge(s,original.Width,original.Height));

            int o = 0;
            int count = samples.Count(s=>!s.Optimised);
            tasksRunning = 0;

            const int maxBusy = 20;

            //allow all non-edge samples to be optimised

            while (o < count)
            {
                var sample = samples.FirstOrDefault(s => !s.Optimised && !s.Triangles.Any(t => t.Busy));

                if (tasksRunning < maxBusy && sample != null)
                {
                    o++;
                    if (o % 50 == 0 && OnUpdate != null)
                        OnUpdate((double)o / (count));

                    if (o % 100 == 0)
                        Console.WriteLine("{0}/{1} - {2} threads", o, samples.Count,tasksRunning);

                    lock (sample.Triangles)
                        sample.Triangles.ForEach(t => t.Busy = true);

                    Thread thread = new Thread(() => minimiseSample(sample, options.Resamples, original, options.Grader));
                    thread.Start();
                    tasksRunning++;
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }


        private static void minimiseSample(Sample s, int resamples, PixelMap original, IGrader grader)
        {
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


                //Console.WriteLine(curPoints.Count);
            }

            s.Point = bestPoint;
            s.Optimised = true;

            lock (s.Triangles)
                s.Triangles.ForEach(t => t.Busy = false);
            tasksRunning--;
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
                    Color gradedColor = grader.Grade(t.U, t.V, t.W, drawPoint);
                    Color originalColor = original[drawPoint.Point];

                    error += Math.Abs(gradedColor.R - originalColor.R);
                    error += Math.Abs(gradedColor.G - originalColor.G);
                    error += Math.Abs(gradedColor.B - originalColor.B);
                }
            }
            return error;
        }

        static bool sampleOnEdge(Sample sample,int width,int height)
        {
            if (sample.Point.X == 0 || sample.Point.Y == 0)
                return true;
            if (sample.Point.X == width - 1 || sample.Point.Y == height - 1)
                return true;
            return false;
        }

    }
}
