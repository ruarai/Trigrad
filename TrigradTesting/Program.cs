using System;
using System.Diagnostics;
using System.Drawing;
using Trigrad;
using Trigrad.ColorGraders;
using Trigrad.DataTypes;

namespace TrigradTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            s.Start();

            PixelMap inputPixelmap = new PixelMap(new Bitmap("tests\\input\\Tulips.jpg"));

            Mark("loaded a");

            FrequencyTable table = new FrequencyTable(inputPixelmap);

            Mark("edge");

            var results = TrigradCompressor.CompressBitmap(inputPixelmap, new TrigradOptions { SampleCount = 150000, SampleRadius = 0, FrequencyTable = table });

            Mark("compressed");

            //results.DebugVisualisation().Save("tests\\points.png");
            //Mark("debug");

            var returned = TrigradDecompressor.DecompressBitmap(results);

            Mark("decompress");

            returned.Output.Bitmap.Save("tests\\output.png");
            //returned.DebugOutput.Save("tests\\debug_output.png");
            //returned.MeshOutput.Save("tests\\mesh_output.png");
        }
        static Stopwatch s = new Stopwatch();

        static void Mark(string msg)
        {
            Console.WriteLine("{0}ms for {1}", s.ElapsedMilliseconds, msg);
            s.Restart();
        }
    }
}
