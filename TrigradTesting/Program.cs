using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Trigrad;
using Trigrad.DataTypes;

namespace TrigradTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap inputBitmap = new Bitmap("tests\\Penguins.jpg");
            FrequencyTable table = new FrequencyTable(inputBitmap);

            var results = TrigradCompressor.CompressBitmap(inputBitmap, new TrigradOptions { SampleCount = 50000, SampleRadius = 0, FrequencyTable = table });

            results.DebugVisualisation().Save("tests\\visualisation.png");


            var returned = TrigradDecompressor.DecompressBitmap(results,TrigradDecompressor.ColorMode.BlindDither);

            returned.Output.Save("tests\\output.png");
            returned.DebugOutput.Save("tests\\debug_output.png");
            returned.MeshOutput.Save("tests\\mesh_output.png");
        }
    }
}
