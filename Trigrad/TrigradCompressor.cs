﻿using System;
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
        public static TrigradCompressed CompressBitmap(PixelMap pixelmap, TrigradOptions options)
        {
            TrigradCompressed compressed = new TrigradCompressed { Height = pixelmap.Height, Width = pixelmap.Width };
            List<Point> samplePoints = new List<Point>();

            samplePoints.Add(new Point(0, 0));
            samplePoints.Add(new Point(pixelmap.Width - 1, 0));
            samplePoints.Add(new Point(0, pixelmap.Height - 1));
            samplePoints.Add(new Point(pixelmap.Width - 1, pixelmap.Height - 1));

            double baseChance = options.SampleCount/options.FrequencyTable.Sum;

            for (int x = 0; x < pixelmap.Width; x++)
            {
                for (int y = 0; y < pixelmap.Height; y++)
                {
                    double chance = ((options.FrequencyTable != null) ? options.FrequencyTable.Table[x, y] : 1d) * baseChance;

                    if (options.Random.NextDouble() < chance)
                    {
                        samplePoints.Add(new Point(x, y));
                    }
                }
            }

            foreach (var sample in samplePoints)
            {
                List<Color> averageColors = new List<Color>();

                for (int x = sample.X - options.SampleRadius; x < sample.X + options.SampleRadius + 1; x++)
                {
                    for (int y = sample.Y - options.SampleRadius; y < sample.Y + options.SampleRadius + 1; y++)
                    {
                        if (y >= 0 && y < pixelmap.Height && x >= 0 && x < pixelmap.Width)
                        {
                            averageColors.Add(pixelmap[x,y]);
                        }
                    }
                }

                byte R = (byte)averageColors.Average(c => c.R);
                byte G = (byte)averageColors.Average(c => c.G);
                byte B = (byte)averageColors.Average(c => c.B);
                compressed.SampleTable[sample] = Color.FromArgb(R, G, B);
            }


            return compressed;
        }
    }
}
