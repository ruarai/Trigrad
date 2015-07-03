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
            Bitmap inputBitmap = new Bitmap("tests\\Tulips.jpg");
            FrequencyTable table = new FrequencyTable(inputBitmap);

            var results = TrigradCompressor.CompressBitmap(inputBitmap, new TrigradOptions { SampleCount = 200000, SampleRadius = 0, FrequencyTable = table });

            results.DebugVisualisation().Save("tests\\visualisation.png");

            if(File.Exists("tests\\out.tri"))
                File.Delete("tests\\out.tri");
            if (File.Exists("tests\\out.json"))
                File.Delete("tests\\out.json");

            GZipStream zip = new GZipStream(new FileStream("tests\\out.tri", FileMode.CreateNew), CompressionLevel.Optimal);

            JsonSerializer s = new JsonSerializer();
            s.Serialize(new BsonWriter(new BinaryWriter(zip)), results);
            s.Serialize(new StreamWriter(new FileStream("tests\\out.json", FileMode.CreateNew)), results);

            var returned = TrigradDecompressor.DecompressBitmap(results);

            returned.Output.Save("tests\\output.png");
            returned.DebugOutput.Save("tests\\debug_output.png");
        }
    }
}
