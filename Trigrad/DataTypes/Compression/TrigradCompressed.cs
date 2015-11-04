using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ICSharpCode.SharpZipLib.BZip2;
using PixelMapSharp;
using TriangleNet;

namespace Trigrad.DataTypes.Compression
{
    /// <summary> The TrigradCompressed form of a bitmap. </summary>
    public partial class TrigradCompressed
    {
        /// <summary> Constructs a TrigradCompressed without any initial data. </summary>
        public TrigradCompressed()
        {

        }
        /// <summary> A dictionary of sampled points to their corresponding colors. </summary>
        public Dictionary<Point, Pixel> SampleTable = new Dictionary<Point, Pixel>();

        public List<SampleTri> Mesh = new List<SampleTri>();
        /// <summary> The width of the bitmap. </summary>
        public int Width;
        /// <summary> The height of the bitmap. </summary>
        public int Height;
        /// <summary> Provides a visualisation of the SampleTable. </summary>
        public PixelMap DebugVisualisation()
        {
            PixelMap map = new PixelMap(Width, Height);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    map[x, y] = new Pixel(10, 10, 10);
                }
            }

            foreach (var value in SampleTable)
            {
                Point p = value.Key;

                map[p]= value.Value;
            }
            return map;
        }

        public PixelMap MeshOutput(PixelMap original)
        {
            return Mesh.DrawMesh(Width, Height);
        }
    }
}