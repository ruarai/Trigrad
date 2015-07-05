using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ICSharpCode.SharpZipLib.BZip2;
using TriangleNet;

namespace Trigrad.DataTypes
{
    /// <summary> The TrigradCompressed form of a bitmap. </summary>
    public class TrigradCompressed
    {
        /// <summary> Constructs an empty TrigradCompressed. </summary>
        public TrigradCompressed()
        {

        }

        /// <summary> A dictionary of sampled points to their corresponding colors. </summary>
        public Dictionary<Point, Color> SampleTable = new Dictionary<Point, Color>();
        /// <summary> The width of the bitmap. </summary>
        public int Width;
        /// <summary> The height of the bitmap. </summary>
        public int Height;
        /// <summary> Provides a visualisation of the SampleTable. </summary>
        public Bitmap DebugVisualisation()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            foreach (var value in SampleTable)
            {
                Point p = value.Key;

                bitmap.SetPixel(p.X, p.Y, value.Value);
            }
            return bitmap;
        }

        /// <summary> Loads a TrigradCompressed image from a stream. </summary>
        public TrigradCompressed(Stream s)
        {
            using (BZip2InputStream dezipper = new BZip2InputStream(s))
            using (BinaryReader reader = new BinaryReader(dezipper))
            {
                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();

                List<Color> colorIndex = new List<Color>();
                Dictionary<Point, ushort> pointIndex = new Dictionary<Point, ushort>();

                ushort colors = reader.ReadUInt16();
                List<byte> r = new List<byte>();
                List<byte> g = new List<byte>();
                List<byte> b = new List<byte>();
                for (int i = 0; i < colors; i++) r.Add(reader.ReadByte());
                for (int i = 0; i < colors; i++) g.Add(reader.ReadByte());
                for (int i = 0; i < colors; i++) b.Add(reader.ReadByte());

                for (int i = 0; i < colors; i++)
                {
                    colorIndex.Add(Color.FromArgb(r[i], g[i], b[i]));
                }


                uint points = reader.ReadUInt32();
                List<ushort> x = new List<ushort>();
                List<ushort> y = new List<ushort>();
                List<ushort> c = new List<ushort>();

                for (int i = 0; i < points; i++) x.Add(reader.ReadUInt16());
                for (int i = 0; i < points; i++) y.Add(reader.ReadUInt16());
                for (int i = 0; i < points; i++) c.Add(reader.ReadUInt16());

                for (int i = 0; i < points; i++)
                {
                    Point p = new Point(x[i], y[i]);
                    ushort index = c[i];
                    pointIndex.Add(p, index);
                }

                foreach (var pair in pointIndex)
                {
                    SampleTable.Add(pair.Key, colorIndex[pair.Value]);
                }

            }
        }

        /// <summary> Saves a TrigradCompressed image to a stream. </summary>
        public void Save(Stream s)
        {
            using (BZip2OutputStream zipper = new BZip2OutputStream(s))
            using (BinaryWriter writer = new BinaryWriter(zipper))
            {
                writer.Write((ushort)Width);
                writer.Write((ushort)Height);

                List<Color> colorIndex = new List<Color>();
                Dictionary<Point, ushort> pointIndex = new Dictionary<Point, ushort>();

                foreach (var pair in SampleTable.OrderBy(kvp => kvp.Value.ToArgb()))
                {
                    int index = colorIndex.IndexOf(pair.Value);

                    if (index != -1)
                    {
                        pointIndex.Add(pair.Key, (ushort)index);
                    }
                    else
                    {
                        colorIndex.Add(pair.Value);
                        pointIndex.Add(pair.Key, (ushort)(colorIndex.Count - 1));
                    }
                }

                writer.Write((ushort)colorIndex.Count);

                foreach (var color in colorIndex)
                    writer.Write(color.R);
                foreach (var color in colorIndex)
                    writer.Write(color.G);
                foreach (var color in colorIndex)
                    writer.Write(color.B);

                writer.Write((uint)pointIndex.Count);

                var sorted = pointIndex.OrderBy(kvp => kvp.Key.X * ushort.MaxValue + kvp.Key.Y).ToArray();
                foreach (var pair in sorted)
                    writer.Write((ushort)pair.Key.X);
                foreach (var pair in sorted)
                    writer.Write((ushort)pair.Key.Y);

                foreach (var pair in sorted)
                    writer.Write(pair.Value);

                writer.Flush();
            }
        }
    }
}