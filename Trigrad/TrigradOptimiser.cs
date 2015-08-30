using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Geometry;
using Trigrad.ColorGraders;
using Trigrad.DataTypes;
using Trigrad.DataTypes.Compression;
using Point = System.Drawing.Point;

namespace Trigrad
{
    public static class TrigradOptimiser
    {

        private static List<SampleTri> Mesh;
        public static void OptimiseMesh(TrigradCompressed compressionData, PixelMap original, TrigradOptions options)
        {
            var mesh = MeshBuilder.BuildMesh(compressionData.SampleTable);


            Mesh = mesh;
            var samples = mesh.SelectMany(t => t.Samples).Distinct().ToList();

            for (int i = 0; i < options.Iterations; i++)
            {
                minimiseMesh(samples, options, original);


                Console.WriteLine("{0}/{1}",i,options.Iterations);
            }

            compressionData.Mesh = mesh;
        }

        private static int j = 0;

        static void minimiseMesh(List<Sample> samples, TrigradOptions options, PixelMap original)
        {
            int o = 0;
            foreach (var sample in samples)
            {
                minimiseSample(sample, options.Resamples, original,options.Grader);

                o++;

                if (o % 1000 == 0)
                    Console.WriteLine("{0}/{1}", o, samples.Count);

                if (o%200 == 0)
                {
                    //Mesh.DrawMesh(original.Width, original.Height).Bitmap.Save("tests\\dframes\\" + j + ".png");


                    j++;
                }
            }
        }


        private static void minimiseSample(Sample s, int resamples, PixelMap original,IGrader grader)
        {
            if (s.Point.X == 0 || s.Point.Y == 0)
                return;

            if (s.Point.X == original.Width - 1 || s.Point.Y == original.Height - 1)
                return;

            var curPoints = s.GetPoints();

            double minError = errorPolygon(s, original,grader);
            Point bestPoint = s.Point;


            int count = curPoints.Count;
            int skip = count / resamples;
            if (skip == 0)
                skip = 1;

            foreach (var drawPoint in curPoints.Where((x, i) => i % skip == 0))
            {
                s.Point = drawPoint.Point;

                s.Recalculate();

                double error = errorPolygon(s, original, grader);
                if (error < minError)
                {
                    bestPoint = drawPoint.Point;
                    minError = error;
                }


                //Console.WriteLine(curPoints.Count);
            }

            s.Point = bestPoint;
        }

        private static double errorPolygon(Sample s, PixelMap original,IGrader grader)
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

                    Color gradedColor = grader.Grade(t.U.Color, t.V.Color, t.W.Color, coords,
                        drawPoint.Point, t.U.Point, t.V.Point, t.W.Point);
                    Color originalColor = original[drawPoint.Point];


                    error += Math.Abs(gradedColor.R - originalColor.R);
                    error += Math.Abs(gradedColor.G - originalColor.G);
                    error += Math.Abs(gradedColor.B - originalColor.B);
                }
            });
            return error;
        }

    }
}
