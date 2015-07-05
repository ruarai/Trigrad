using System;
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
            Bitmap inputBitmap = new Bitmap("tests\\Tulips.jpg");
            FrequencyTable table = new FrequencyTable(inputBitmap);

            var results = TrigradCompressor.CompressBitmap(inputBitmap, new TrigradOptions { SampleCount = 150000, SampleRadius = 0, FrequencyTable = table });

            results.DebugVisualisation().Save("tests\\points.png");

            Console.WriteLine(results.SampleTable.Count);

            var returned = TrigradDecompressor.DecompressBitmap(results);

            returned.Output.Save("tests\\output.png");
            returned.DebugOutput.Save("tests\\debug_output.png");
            returned.MeshOutput.Save("tests\\mesh_output.png");
        }
    }
}
