using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Data;
using TriangleNet.Geometry;
using Trigrad.DataTypes;
using Trigrad.DataTypes.Compression;
using Point = System.Drawing.Point;

namespace Trigrad
{
    public delegate void CompressorProgressUpdate(double progress);

    /// <summary> Holds methods for compressing trigrad compressed imagery. </summary>
    public static class TrigradCompressor
    {
        public static event CompressorProgressUpdate OnUpdate;

        /// <summary> Compresses a bitmap using TrigradCompression. </summary>
        /// <param name="pixelmap"> The input bitmap.</param>
        /// <param name="options"> TrigradOptions specifying how the image will be compressed.</param>
        public static TrigradCompressed CompressBitmap(PixelMap pixelmap, TrigradOptions options)
        {
            TrigradCompressed compressed = new TrigradCompressed { Height = pixelmap.Height, Width = pixelmap.Width };

            double baseChance = options.SampleCount / (options.FrequencyTable.Sum);

            int i = 0;
            int count = pixelmap.Width * pixelmap.Height;

            Parallel.For(0, pixelmap.Width, x =>
            {
                for (int y = 0; y < pixelmap.Height; y++)
                {
                    if ((x == 0 && y == 0) || 
                        (x == pixelmap.Width - 1 && y == 0) ||
                        (x == 0 && y == pixelmap.Height - 1) ||
                        (x == pixelmap.Width - 1 && y == pixelmap.Height - 1))
                    {
                        compressed.SampleTable[new Point(x, y)] = pixelmap[new Point(x, y)];
                        continue;
                    }

                    double chance = ((options.FrequencyTable != null)
                        ? options.FrequencyTable.Table[x, y]
                        : 1d) * baseChance;

                    lock(options.Random)
                    if (options.Random.NextDouble() < chance)
                    {
                        lock (compressed.SampleTable)
                            compressed.SampleTable[new Point(x, y)] = pixelmap[new Point(x, y)];
                    }


                    if (i % 50 == 0 && OnUpdate != null)
                        OnUpdate((double)i / count);

                    i++;
                }
            });


            return compressed;
        }
    }
}
