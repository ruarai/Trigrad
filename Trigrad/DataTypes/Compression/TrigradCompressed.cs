using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ICSharpCode.SharpZipLib.BZip2;
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
        public Dictionary<Point, Color> SampleTable = new Dictionary<Point, Color>();

        public List<SampleTri> Mesh = new List<SampleTri>();
        /// <summary> The width of the bitmap. </summary>
        public int Width;
        /// <summary> The height of the bitmap. </summary>
        public int Height;
        /// <summary> Provides a visualisation of the SampleTable. </summary>
        public Bitmap DebugVisualisation()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(10, 10, 10)), new Rectangle(0, 0, Width, Height));
            }

            foreach (var value in SampleTable)
            {
                Point p = value.Key;

                bitmap.SetPixel(p.X, p.Y, value.Value);
            }
            return bitmap;
        }

        public PixelMap MeshOutput(PixelMap original)
        {
            return Mesh.DrawMesh(Width, Height,original);
        }
    }
}