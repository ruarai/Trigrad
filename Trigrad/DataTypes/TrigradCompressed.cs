using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using TriangleNet;

namespace Trigrad.DataTypes
{
    /// <summary> The TrigradCompressed form of a bitmap. </summary>
    public class TrigradCompressed
    {
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

        public TrigradCompressed(Stream s)
        {
            using (GZipStream dezipper = new GZipStream(s, CompressionMode.Decompress))
            using (BinaryReader reader = new BinaryReader(new MemoryStream()))
            {
                dezipper.CopyTo(reader.BaseStream);
                reader.BaseStream.Position = 0;


                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();

                List<Color> colorIndex = new List<Color>();
                Dictionary<Point, ushort> pointIndex = new Dictionary<Point, ushort>();

                ushort colors = reader.ReadUInt16();
                for (int i = 0; i < colors; i++)
                {
                    colorIndex.Add(Color.FromArgb(reader.ReadByte(), reader.ReadByte(), reader.ReadByte()));
                }
                uint points = reader.ReadUInt32();
                for (int i = 0; i < points; i++)
                {
                    Point p = new Point(reader.ReadUInt16(), reader.ReadUInt16());
                    ushort index = reader.ReadUInt16();
                    pointIndex.Add(p, index);
                }

                foreach (var pair in pointIndex)
                {
                    SampleTable.Add(pair.Key, colorIndex[pair.Value]);
                }

            }
        }

        public void Save(Stream s)
        {
            using(GZipStream zipper = new GZipStream(s,CompressionLevel.Optimal))
            using (BinaryWriter writer = new BinaryWriter(zipper))
            {
                writer.Write((ushort)Width);
                writer.Write((ushort)Height);

                List<Color> colorIndex = new List<Color>();
                Dictionary<Point,ushort> pointIndex = new Dictionary<Point, ushort>();

                foreach (var pair in SampleTable)
                {
                    if (colorIndex.Contains(pair.Value))
                    {
                        pointIndex.Add(pair.Key, (ushort)colorIndex.IndexOf(pair.Value));
                    }
                    else
                    {
                        colorIndex.Add(pair.Value);
                        pointIndex.Add(pair.Key,(ushort)(colorIndex.Count -1));
                    }
                }

                writer.Write((ushort)colorIndex.Count);

                foreach (var color in colorIndex)
                {
                    writer.Write(color.R);
                    writer.Write(color.G);
                    writer.Write(color.B);
                }

                writer.Write((uint)pointIndex.Count);

                foreach (var pair in pointIndex)
                {
                    writer.Write((ushort)pair.Key.X);
                    writer.Write((ushort)pair.Key.Y);

                    writer.Write(pair.Value);

                }


                writer.Flush();
            }
        }
    }
}
