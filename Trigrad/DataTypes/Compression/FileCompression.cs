using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.BZip2;

namespace Trigrad.DataTypes.Compression
{
    public partial class TrigradCompressed
    {
        /// <summary> Saves a TrigradCompressed image to a stream. </summary>
        public void Save(Stream s)
        {
            using (BZip2OutputStream zipper = new BZip2OutputStream(s))
            using (BinaryWriter writer = new BinaryWriter(zipper))
            {
                writer.Write((ushort)Width);
                writer.Write((ushort)Height);

                writer.Write((uint)SampleTable.Count);

                var sorted = SampleTable.OrderBy(kvp => kvp.Key.X * ushort.MaxValue + kvp.Key.Y).ToArray();
                foreach (var pair in sorted)
                    writer.Write((ushort)pair.Key.X);
                foreach (var pair in sorted)
                    writer.Write((ushort)pair.Key.Y);

                foreach (var pair in sorted)
                    writer.Write(pair.Value.R);
                foreach (var pair in sorted)
                    writer.Write(pair.Value.G);
                foreach (var pair in sorted)
                    writer.Write(pair.Value.B);

                writer.Flush();
            }
        }
    }
}
