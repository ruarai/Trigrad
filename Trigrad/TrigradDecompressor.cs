using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using PixelMapSharp;
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
                    decompressed.Output[x, y] = new Pixel(Color.HotPink);

            drawMesh(compressionData.Mesh, decompressed.Output, options);


            fillGaps(decompressed.Output);

            return decompressed;
        }


        private static void drawMesh(List<SampleTri> mesh, PixelMap output, TrigradOptions options)
        {
            int i = 0;
            int count = mesh.Count;
            foreach (var triangle in mesh)
            {
                options.Renderer.Fill(triangle,output);


                if (i % 50 == 0 && OnUpdate != null)
                    OnUpdate((double)i / count);

                i++;
            }
        }

        private static void fillGaps(PixelMap p)
        {
            Pixel lastColor = p[0];
            Pixel pink = new Pixel(Color.HotPink);

            for (int x = 0; x < p.Width; x++)
            {
                for (int y = 0; y < p.Height; y++)
                {
                    if (alike(p[x, y],pink))
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

        static bool alike(Pixel a, Pixel b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B;
        }
    }
}
