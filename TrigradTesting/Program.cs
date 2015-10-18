using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Trigrad;
using Trigrad.ColorGraders;
using Trigrad.DataTypes;
using Trigrad.DataTypes.Compression;
using Trigrad.Filters;

namespace TrigradTesting
{
    class Program
    {
        private static string backup = @"C:\deepstorage";

        static void Main(string[] args)
        {
            string input = "tests\\input\\Art.jpg";

            PixelMap inputBitmap = PixelMap.SlowLoad(new Bitmap(input));
            FrequencyTable table = new FrequencyTable(inputBitmap, 1, 0.1);

            var options = new TrigradOptions { SampleCount =100000, FrequencyTable = table, ScaleFactor = 1, Resamples = 10, Iterations = 1, Grader = new BarycentricGrader() };

            var results = TrigradCompressor.CompressBitmap(inputBitmap, options);

            //var results = fauxResults(inputBitmap);

            results.DebugVisualisation().Save("tests\\points.png");

            results.Mesh = MeshBuilder.BuildMesh(results.SampleTable);

            //TrigradOptimiser.OptimiseMesh(results, inputBitmap, options);

            //new AreaFilter(2).Run(results.Mesh);
            //new GridFilter(4).Run(results.Mesh);
            //new MedianFilter(16).Run(results.Mesh);
            //results.Save(new FileStream("tests\\out.tri", FileMode.Create));

            results.MeshOutput(inputBitmap).Bitmap.Save("tests\\mesh.png");

            Console.WriteLine(results.SampleTable.Count);

            //var loaded = new TrigradCompressed(new FileStream("tests\\out.tri", FileMode.Open));

            GPUT.CalculateMesh(results.Mesh);

            var returned = TrigradDecompressor.DecompressBitmap(results, options);

            returned.Output.Bitmap.Save("tests\\output.png");
            returned.DebugOutput.Bitmap.Save("tests\\debug_output.png");

            int error = errorBitmap(inputBitmap, returned.Output);
            double avgError = Math.Round((double)error / (inputBitmap.Width * inputBitmap.Height * 3), 2);

            Console.WriteLine("{0} error", avgError);

            saveBackup(avgError, Path.GetFileNameWithoutExtension(input), options);

            Console.Beep();
            Console.ReadKey();
        }

        static void saveBackup(double error, string file, TrigradOptions options)
        {
            Random r = new Random();

            var path = Path.Combine(backup, string.Format("{0} Test #{1}, {2} err, {3} samples, {4} resamples, {5} iterations", file, r.Next(), error, options.SampleCount, options.Resamples, options.Iterations));
            Directory.CreateDirectory(path);

            File.Copy("tests\\points.png", Path.Combine(path, "points.png"));
            File.Copy("tests\\output.png", Path.Combine(path, "output.png"));
            //File.Copy("tests\\debug_output.png", Path.Combine(path, "debug_output.png"));
            File.Copy("tests\\mesh.png", Path.Combine(path, "mesh.png"));
            File.Copy("tests\\error.png", Path.Combine(path, "error.png"));

        }

        static int errorBitmap(PixelMap a, PixelMap b)
        {
            int error = 0;
            PixelMap output = new PixelMap(a.Width, a.Height);
            for (int x = 0; x < a.Width; x++)
            {
                for (int y = 0; y < a.Height; y++)
                {
                    Color cA = a[x, y];
                    Color cB = b[x, y];

                    error += Math.Abs(cA.R - cB.R);
                    error += Math.Abs(cA.G - cB.G);
                    error += Math.Abs(cA.B - cB.B);

                    output[x, y] = Color.FromArgb(Math.Abs(cA.R - cB.R), Math.Abs(cA.G - cB.G), Math.Abs(cA.B - cB.B));
                }
            }
            output.Bitmap.Save("tests\\error.png");

            return error;
        }

        static TrigradCompressed fauxResults(PixelMap input)
        {
            var results = new TrigradCompressed();

            results.Width = input.Width;
            results.Height = input.Height;

            List<Point> samplePoints = new List<Point>();

            samplePoints.Add(new Point(0, 0));
            samplePoints.Add(new Point(input.Width - 1, 0));
            samplePoints.Add(new Point(0, input.Height - 1));
            samplePoints.Add(new Point(input.Width - 1, input.Height - 1));

            int cellsize = 8;



            for (int x = 0; x < input.Width / cellsize; x++)
            {
                for (int y = 0; y < input.Height / cellsize; y++)
                {
                    samplePoints.Add(new Point(x * cellsize, y * cellsize));
                }
            }

            foreach (var samplePoint in samplePoints)
            {
                results.SampleTable[samplePoint] = input[samplePoint];
            }

            results.Mesh = MeshBuilder.BuildMesh(results.SampleTable);

            return results;
        }
    }

}
