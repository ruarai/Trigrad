using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.BZip2;

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
            }
        }

        internal struct ColorStruct
        {
            public byte R;
            public byte G;
            public byte B;

            public Color Color { get { return Color.FromArgb(R, G, B); } }
        }
    }
}
