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
            string zip = "tests\\out.tri";
            Compress(zip);
            Decompress(zip);
        }

        static void Compress(string file)
        {
            Bitmap inputBitmap = new Bitmap("tests\\Tulips.jpg");
            FrequencyTable table = new FrequencyTable(inputBitmap);

            var results = TrigradCompressor.CompressBitmap(inputBitmap, new TrigradOptions { SampleCount = 120000, SampleRadius = 0, FrequencyTable = table });

            results.DebugVisualisation().Save("tests\\visualisation.png");


            results.Save(new FileStream(file, FileMode.Create));
        }

        static void Decompress(string file)
        {
            var compressed = new TrigradCompressed(new FileStream(file, FileMode.Open));

            var returned = TrigradDecompressor.DecompressBitmap(compressed);

            returned.Output.Save("tests\\output.png");
            returned.DebugOutput.Save("tests\\debug_output.png");
        }
    }
}
