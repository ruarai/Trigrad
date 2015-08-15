using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Trigrad;
using Trigrad.ColorGraders;
using Trigrad.DataTypes;
using Trigrad.DataTypes.Compression;

namespace TrigradTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            PixelMap inputBitmap = new PixelMap(new Bitmap("tests\\input\\Lenna.png"));
            FrequencyTable table = new FrequencyTable(inputBitmap,1, 0.05);

            var results = TrigradCompressor.CompressBitmap(inputBitmap, new TrigradOptions { SampleCount = 10000, FrequencyTable = table, ScaleFactor = 1 });

            //var results = fauxResults(inputBitmap);

            results.DebugVisualisation().Save("tests\\points.png");
            results.Save(new FileStream("tests\\out.tri", FileMode.Create));

            Console.WriteLine(results.SampleTable.Count);

            var returned = TrigradDecompressor.DecompressBitmap(results, inputBitmap);

            returned.Output.Bitmap.Save("tests\\output.png");
            returned.DebugOutput.Bitmap.Save("tests\\debug_output.png");

            errorBitmap(inputBitmap,returned.Output).Bitmap.Save("tests\\error.png");
            Console.Beep();
            Console.ReadKey();
        }

        static PixelMap errorBitmap(PixelMap a, PixelMap b)
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

                    output[x,y]= Color.FromArgb(Math.Abs(cA.R - cB.R), Math.Abs(cA.G - cB.G), Math.Abs(cA.B - cB.B));
                }
            }
            Console.WriteLine("{0} error",Math.Round((double)error/(a.Width*a.Height*3),2));
            return output;
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
            samplePoints.Add(new Point(128, 128));
            samplePoints.Add(new Point(256, 256));
            samplePoints.Add(new Point(256 + 128, 256 + 128));

            foreach (var samplePoint in samplePoints)
            {
                results.SampleTable[samplePoint] = Color.Red;
            }

            return results;
        }
    }

}
