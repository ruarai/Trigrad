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
        /// <summary> Saves a TrigradCompressed image to a stream. </summary>
        public void Save(Stream s)
        {
            using (BZip2OutputStream zipper = new BZip2OutputStream(s))
            using (BinaryWriter writer = new BinaryWriter(zipper))
            {
                writer.Write((ushort)Width);
                writer.Write((ushort)Height);

                var samples = Mesh.SelectMany(t => t.Samples).Distinct().ToList();

                writer.Write((uint)samples.Count);

                var sorted = samples.OrderBy(kvp => kvp.Point.X * ushort.MaxValue + kvp.Point.Y).ToList();
                foreach (var sample in sorted)
                    writer.Write((ushort)sample.Point.X);
                foreach (var sample in sorted)
                    writer.Write((ushort)sample.Point.Y);

                foreach (var sample in sorted)
                    writer.Write(sample.Color.R);
                foreach (var sample in sorted)
                    writer.Write(sample.Color.G);
                foreach (var sample in sorted)
                    writer.Write(sample.Color.B);

                writer.Write((uint)Mesh.Count);

                foreach (var tri in Mesh)
                    writer.Write(sorted.FindIndex(p => p.Point == tri.U.Point));
                foreach (var tri in Mesh)
                    writer.Write(sorted.FindIndex(p => p.Point == tri.V.Point));
                foreach (var tri in Mesh)
                    writer.Write(sorted.FindIndex(p => p.Point == tri.W.Point));


                writer.Flush();
            }
        }
    }
}
