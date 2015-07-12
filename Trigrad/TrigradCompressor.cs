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
    /// <summary> Holds methods for compressing trigrad compressed imagery. </summary>
    public static class TrigradCompressor
    {
        /// <summary> Compresses a bitmap using TrigradCompression. </summary>
        /// <param name="bitmap"> The input bitmap.</param>
        /// <param name="options"> TrigradOptions specifying how the image will be compressed.</param>
        public static TrigradCompressed CompressBitmap(Bitmap bitmap, TrigradOptions options)
        {
            bitmap = resizeImage(bitmap, options.ScaleFactor);

            TrigradCompressed compressed = new TrigradCompressed { Height = bitmap.Height, Width = bitmap.Width };
            List<Point> samplePoints = new List<Point>();

            samplePoints.Add(new Point(0, 0));
            samplePoints.Add(new Point(bitmap.Width - 1, 0));
            samplePoints.Add(new Point(0, bitmap.Height - 1));
            samplePoints.Add(new Point(bitmap.Width - 1, bitmap.Height - 1));

            double baseChance = options.SampleCount / (options.FrequencyTable.Sum * options.ScaleFactor * options.ScaleFactor);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Point original = new Point(x / options.ScaleFactor, y / options.ScaleFactor);

                    double chance = ((options.FrequencyTable != null) ? options.FrequencyTable.Table[original.X, original.Y] : 1d) * baseChance;

                    if (options.Random.NextDouble() < chance)
                    {
                        samplePoints.Add(new Point(x, y));
                    }
                }
            }

            bitmap.Save("scaled.png");

            foreach (var sample in samplePoints)
            {
                compressed.SampleTable[sample] = bitmap.GetPixel(sample.X, sample.Y);
            }


            return compressed;
        }
        private static Bitmap resizeImage(Bitmap img, int factor)
        {
            Bitmap b = new Bitmap((img.Width-1) * factor, (img.Height-1) * factor);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(img, 0, 0, img.Width * factor, img.Height * factor);
            }
            return b;
        }
    }
}
