﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Data;
using TriangleNet.Geometry;
using Trigrad.ColorGraders;
using Trigrad.DataTypes;
using Trigrad.DataTypes.Compression;
using Point = System.Drawing.Point;

namespace Trigrad
{
    public delegate void DecompressorProgressUpdate(double progress);
    /// <summary> Holds methods for decompressing trigrad compressed imagery. </summary>
    public static class TrigradDecompressor
    {
        public static event DecompressorProgressUpdate OnUpdate;

        /// <summary> Decompresses a trigrad compressed bitmap. </summary>
        /// <param name="compressionData"> The TrigradCompressed data.</param>
        /// <param name="original"> The original image to determine the most effect fill mode.</param>
        /// <param name="options"> Options dictating the decompression.</param>
        public static TrigradDecompressed DecompressBitmap(TrigradCompressed compressionData, TrigradOptions options)
        {
            TrigradDecompressed decompressed = new TrigradDecompressed(compressionData.Width, compressionData.Height);

            for (int x = 0; x < compressionData.Width; x++)
                for (int y = 0; y < compressionData.Height; y++)
                    decompressed.Output[x, y] = Color.HotPink;

            drawMesh(compressionData.Mesh, decompressed.Output, options.Grader);


            fillGaps(decompressed.Output);

            return decompressed;
        }


        private static void drawMesh(List<SampleTri> mesh, PixelMap output, IGrader grader)
        {
            int i = 0;
            int count = mesh.Count;
            Parallel.ForEach(mesh, triangle =>
            {
                fillTriangle(triangle, output,grader);


                if (i % 50 == 0 && OnUpdate != null)
                    OnUpdate((double)i / count);

                i++;
            });
        }
        private static void fillTriangle(SampleTri t, PixelMap map, IGrader grader)
        {
            foreach (var drawPoint in t.Points)
            {
                Color gradedColor = grader.Grade(t.U,t.V,t.W,drawPoint);

                //var coords = drawPoint.BarycentricCoordinates;
                //Color gradedColor = Color.FromArgb((byte)(coords.U * 255), (byte)(coords.V * 255), (byte)(coords.W * 255));

                map[drawPoint.Point] = gradedColor;
            }
        }

        private static void fillGaps(PixelMap p)
        {
            Color lastColor = p[0];
            for (int x = 0; x < p.Width; x++)
            {
                for (int y = 0; y < p.Height; y++)
                {
                    if (p[x, y] == Color.HotPink)
                    {
                        p[x, y] = lastColor;
                    }
                    else
                    {
                        lastColor = p[x, y];
                    }
                }
            }
        }
    }
}
