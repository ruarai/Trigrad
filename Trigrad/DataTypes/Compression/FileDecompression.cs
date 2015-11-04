using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.BZip2;
using PixelMapSharp;

namespace Trigrad.DataTypes.Compression
{
    public partial class TrigradCompressed
    {
        /// <summary> Loads a TrigradCompressed image from a stream. </summary>
        public TrigradCompressed(Stream s)
        {
            using (BZip2InputStream dezipper = new BZip2InputStream(s))
            using (BinaryReader reader = new BinaryReader(dezipper))
            {
                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();

                uint count = reader.ReadUInt32();

                Point[] points = new Point[count];

                for (int i = 0; i < count; i++)
                    points[i].X = reader.ReadUInt16();
                for (int i = 0; i < count; i++)
                    points[i].Y = reader.ReadUInt16();

                ColorStruct[] colors = new ColorStruct[count];

                for (int i = 0; i < count; i++)
                    colors[i].R = reader.ReadByte();
                for (int i = 0; i < count; i++)
                    colors[i].G = reader.ReadByte();
                for (int i = 0; i < count; i++)
                    colors[i].B = reader.ReadByte();

                for (int i = 0; i < count; i++)
                {
                    SampleTable.Add(points[i],colors[i].Color);
                }

                uint meshCount = reader.ReadUInt32();

                SampleTri[] tris = new SampleTri[meshCount];


                for (int i = 0; i < meshCount; i++)
                    tris[i] = new SampleTri();

                for (int i = 0; i < meshCount; i++)
                    tris[i].U = new Sample(points[reader.ReadInt32()], new Pixel(Color.Black));

                for (int i = 0; i < meshCount; i++)
                    tris[i].V = new Sample(points[reader.ReadInt32()], new Pixel(Color.Black));

                for (int i = 0; i < meshCount; i++)
                    tris[i].W = new Sample(points[reader.ReadInt32()], new Pixel(Color.Black));

                foreach (var tri in tris)
                {
                    tri.U.Color = SampleTable[tri.U.Point];
                    tri.V.Color = SampleTable[tri.V.Point];
                    tri.W.Color = SampleTable[tri.W.Point];
                }

                Mesh = tris.ToList();
            }
        }

        internal struct ColorStruct
        {
            public byte R;
            public byte G;
            public byte B;

            public Pixel Color { get { return new Pixel(R, G, B); } }
        }
    }
}
