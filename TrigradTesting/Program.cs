using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using PixelMapSharp;
using Trigrad;
using Trigrad.ColorGraders;
using Trigrad.DataTypes;
using Trigrad.DataTypes.Compression;
using Trigrad.Filters;
using Trigrad.Renderers;

namespace TrigradTesting
{
    class Program
    {
        private static string backup = @"E:\deepstorage";

        static void Main(string[] args)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            Console.WriteLine("Trigrad");
            string input = "tests\\input\\Rebecca.jpg";

            PixelMap inputBitmap = PixelMap.SlowLoad(new Bitmap(input));
            FrequencyTable table = new FrequencyTable(inputBitmap,1);

            int n = 6;
            var options = new TrigradOptions
            {
                SampleCount = 30000, 
                FrequencyTable = table,
                Resamples = 4,
                Iterations =1, 
                Random = new Random(0),
                ResampleColors = true,
                Renderer = new ShapeFill
                {
                    ShapeFunction = t => Math.Cos(Math.PI / n) / Math.Cos(t % ((2 * Math.PI) / n) - Math.PI / n)
                }
            };

            var results = TrigradCompressor.CompressBitmap(inputBitmap, options);

            results.DebugVisualisation().GetBitmap().Save("tests\\points.png");

            results.Mesh = MeshBuilder.BuildMesh(results.SampleTable);
            results.MeshOutput(inputBitmap).GetBitmap().Save("tests\\mesh_a.png");

            TrigradOptimiser.OptimiseMesh(results, inputBitmap, options);


            GPUT.CalculateMesh(results.Mesh);

            //results.Save(new FileStream("tests\\out.tri", FileMode.Create));

            results.MeshOutput(inputBitmap).GetBitmap().Save("tests\\mesh_b.png");

            Console.WriteLine(results.SampleTable.Count);

            //var loaded = new TrigradCompressed(new FileStream("tests\\out.tri", FileMode.Open));

            results.Mesh.Shuffle(options.Random);

            var returned = TrigradDecompressor.DecompressBitmap(results, options);

            returned.Output.GetBitmap().Save("tests\\output.png");
            returned.DebugOutput.GetBitmap().Save("tests\\debug_output.png");

            int error = errorBitmap(inputBitmap, returned.Output);
            double avgError = Math.Round((double)error / (inputBitmap.Width * inputBitmap.Height * 3), 2);

            Console.WriteLine("{0} error", avgError);
            Console.WriteLine("{0} s", Math.Round(s.ElapsedMilliseconds / 1000d, 2));

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
            File.Copy("tests\\mesh_a.png", Path.Combine(path, "mesh_a.png"));
            File.Copy("tests\\mesh_b.png", Path.Combine(path, "mesh_b.png"));
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
                    Pixel cA = a[x, y];
                    Pixel cB = b[x, y];

                    Pixel diff = cA - cB;

                    error += (diff.R + diff.G + diff.B);

                    output[x, y] = diff;
                }
            }
            output.GetBitmap().Save("tests\\error.png");

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
